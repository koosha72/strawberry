namespace Strawberry.Sound.Midi
{
    using Math = System.Math;

    /// <summary>
    /// An <see cref="ISoundReader"/> that synthesises PCM audio from a MIDI file,
    /// compatible with the Strawberry engine's streaming system.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Usage patterns:
    /// <list type="number">
    ///   <item>With a native TinySoundFont backend (recommended for desktop):
    ///     <code>var reader = new MidiReader(midiStream, new TinySoundFontBackend("GeneralUser.sf2"));</code>
    ///   </item>
    ///   <item>With the managed fallback (WASM / AOT safe):
    ///     <code>var reader = new MidiReader(midiStream, new ManagedSynthBackend());</code>
    ///   </item>
    ///   <item>Auto-detect backend (tries TSF native, falls back to managed):
    ///     <code>var reader = new MidiReader(midiStream, "GeneralUser.sf2");</code>
    ///   </item>
    /// </list>
    /// </para>
    /// <para>
    /// The reader renders audio on-demand as <see cref="Read"/> is called by the
    /// streaming system. <see cref="Seek"/> rebuilds synth state by replaying MIDI
    /// events from the beginning up to the target position (without rendering audio),
    /// then resumes normal rendering from that point.
    /// </para>
    /// </remarks>
    public sealed class MidiReader : ISoundReader
    {
        readonly MidiFile midiFile;
        readonly ISynthBackend synth;
        readonly bool ownsSynth;

        // Current playback state
        int eventIndex;
        long currentSamplePos;
        long totalSamples;

        bool disposed;

        // ─────────────────────────────────────────────
        //  Properties (ISoundReader)
        // ─────────────────────────────────────────────

        /// <summary>The original MIDI file stream (read-only reference).</summary>
        public Stream Stream { get; }

        /// <summary>Always 16 (16-bit PCM output for OpenAL compatibility).</summary>
        public int BitsPerSample => 16;

        /// <summary>Output sample rate (matches the synth backend).</summary>
        public int SampleRate => synth.SampleRate;

        /// <summary>Output channel count (1 = mono, 2 = stereo).</summary>
        public int Channels => synth.Channels;

        /// <summary>Total PCM data size in bytes.</summary>
        public int DataSize { get; }

        // ─────────────────────────────────────────────
        //  Constructors
        // ─────────────────────────────────────────────

        /// <summary>
        /// Create a MidiReader with an explicitly provided synth backend.
        /// The caller is responsible for the backend's lifetime (not disposed by this reader).
        /// </summary>
        /// <param name="midiStream">Stream containing MIDI file data.</param>
        /// <param name="synthBackend">Pre-configured synthesizer backend.</param>
        public MidiReader(Stream midiStream, ISynthBackend synthBackend)
        {
            if (midiStream == null) throw new ArgumentNullException(nameof(midiStream));
            if (synthBackend == null) throw new ArgumentNullException(nameof(synthBackend));

            Stream = midiStream;
            synth = synthBackend;
            ownsSynth = false;

            midiFile = new MidiFile(midiStream);
            totalSamples = (long)(midiFile.DurationSeconds * synth.SampleRate);
            long dataSizeLong = totalSamples * synth.Channels * (BitsPerSample / 8);
            DataSize = dataSizeLong > int.MaxValue ? int.MaxValue : (int)dataSizeLong;
        }

        /// <summary>
        /// Create a MidiReader that auto-selects a synth backend.
        /// Attempts to load TinySoundFont native; falls back to <see cref="ManagedSynthBackend"/>.
        /// The synth backend is owned and disposed by this reader.
        /// </summary>
        /// <param name="midiStream">Stream containing MIDI file data.</param>
        /// <param name="soundFontPath">
        /// Path to a .sf2 SoundFont file (required for TinySoundFont).
        /// If null or the native library is unavailable, the managed fallback is used.
        /// </param>
        /// <param name="sampleRate">Output sample rate (default 44100).</param>
        /// <param name="channels">Output channels: 1 or 2 (default 2).</param>
        public MidiReader(Stream midiStream, Stream soundFont = null,
                          int sampleRate = 44100, int channels = 2)
        {
            if (midiStream == null) throw new ArgumentNullException(nameof(midiStream));

            Stream = midiStream;
            midiFile = new MidiFile(midiStream);

            ISynthBackend? backend = null;

            // Try TinySoundFont native if a SoundFont path is provided
            if (soundFont != null)
            {
                try
                {
                    backend = new TinySoundFontBackend(soundFont, sampleRate, channels, -10);
                }
                catch (DllNotFoundException)
                {
                    // libtsf not available — fall back to managed synth
                    System.Diagnostics.Debug.WriteLine(
                        "[MidiReader] libtsf not found, falling back to managed synthesizer. " +
                        "For high-quality audio, compile TinySoundFont and place the native library on the search path.");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[MidiReader] TinySoundFont failed to load: {ex.Message}. " +
                        "Falling back to managed synthesizer.");
                }
            }

            backend ??= new ManagedSynthBackend(sampleRate, channels);

            synth = backend;
            ownsSynth = true;

            totalSamples = (long)(midiFile.DurationSeconds * synth.SampleRate);
            long dataSizeLong = totalSamples * synth.Channels * (BitsPerSample / 8);
            DataSize = dataSizeLong > int.MaxValue ? int.MaxValue : (int)dataSizeLong;
        }

        // ─────────────────────────────────────────────
        //  ISoundReader — Read
        // ─────────────────────────────────────────────

        /// <summary>
        /// Read synthesised PCM audio into the buffer. Audio is rendered on-demand
        /// by advancing through the MIDI event list and feeding the synth backend.
        /// </summary>
        /// <returns>Number of bytes written into the buffer, or 0 if at end of data.</returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            ThrowIfDisposed();

            if (count <= 0) return 0;

            int bytesPerSampleFrame = Channels * (BitsPerSample / 8);
            long currentBytePos = currentSamplePos * bytesPerSampleFrame;

            if (currentBytePos >= DataSize)
                return 0;

            // Clamp to available data
            int bytesToRead = Math.Min(count, DataSize - (int)currentBytePos);
            int totalSampleFrames = bytesToRead / bytesPerSampleFrame;

            // Round down to frame boundary to avoid buffer overrun
            bytesToRead = totalSampleFrames * bytesPerSampleFrame;

            var shortBuffer = new short[totalSampleFrames * Channels];
            int framesRendered = 0;

            double currentTime = (double)currentSamplePos / SampleRate;
            double bufferEndTime = currentTime + (double)totalSampleFrames / SampleRate;

            // Event-synchronized rendering: process events at the correct sample position
            // to avoid quantising all events to the buffer boundary.
            while (framesRendered < totalSampleFrames)
            {
                // Process any overdue events (events at or before current time)
                while (eventIndex < midiFile.Events.Count &&
                       midiFile.Events[eventIndex].TimeSeconds <= currentTime)
                {
                    ProcessEvent(midiFile.Events[eventIndex]);
                    eventIndex++;
                }

                // Find time of next event within this buffer
                double nextEventTime = bufferEndTime;
                if (eventIndex < midiFile.Events.Count)
                {
                    double evtTime = midiFile.Events[eventIndex].TimeSeconds;
                    if (evtTime > currentTime && evtTime < nextEventTime)
                        nextEventTime = evtTime;
                }

                // Render audio up to the next event or end of buffer
                // Use Ceiling to ensure we always make forward progress,
                // then clamp to remaining buffer space.
                int framesToRender = (int)Math.Ceiling((nextEventTime - currentTime) * SampleRate);
                framesToRender = Math.Clamp(framesToRender, 1, totalSampleFrames - framesRendered);

                synth.RenderShort(shortBuffer, framesRendered * Channels, framesToRender);
                framesRendered += framesToRender;
                currentTime += (double)framesToRender / SampleRate;
                currentSamplePos += framesToRender;
            }

            // Copy short[] → byte[]
            Buffer.BlockCopy(shortBuffer, 0, buffer, offset, bytesToRead);

            return bytesToRead;
        }

        // ─────────────────────────────────────────────
        //  ISoundReader — Seek
        // ─────────────────────────────────────────────

        /// <summary>
        /// Seek to a byte offset in the decoded PCM data.
        /// Rebuilds synth state by replaying all MIDI events from the start
        /// up to the target position (without rendering audio), then positions
        /// the render cursor at the target.
        /// </summary>
        public void Seek(long offset)
        {
            ThrowIfDisposed();

            int bytesPerSampleFrame = Channels * (BitsPerSample / 8);
            long targetSample = offset / bytesPerSampleFrame;
            targetSample = Math.Clamp(targetSample, 0, totalSamples);

            double targetTime = (double)targetSample / SampleRate;

            // Reset synth to clean state
            synth.Reset();

            // Replay all events up to target time (state-only, no audio rendering)
            eventIndex = 0;
            while (eventIndex < midiFile.Events.Count &&
                   midiFile.Events[eventIndex].TimeSeconds < targetTime)
            {
                ProcessEvent(midiFile.Events[eventIndex]);
                eventIndex++;
            }

            currentSamplePos = targetSample;
        }

        // ─────────────────────────────────────────────
        //  ISoundReader — ReadAll
        // ─────────────────────────────────────────────

        /// <summary>
        /// Read the entire MIDI file as decoded PCM data.
        /// Resets the reader to the beginning first, then renders all audio.
        /// </summary>
        public byte[] ReadAll()
        {
            ThrowIfDisposed();

            Seek(0);
            var buffer = new byte[DataSize];
            int totalRead = 0;
            while (totalRead < DataSize)
            {
                int bytesRead = Read(buffer, totalRead, DataSize - totalRead);
                if (bytesRead <= 0) break;
                totalRead += bytesRead;
            }
            return buffer;
        }

        // ─────────────────────────────────────────────
        //  IDisposable
        // ─────────────────────────────────────────────

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            if (ownsSynth)
                synth.Dispose();
        }

        // ─────────────────────────────────────────────
        //  Internal
        // ─────────────────────────────────────────────

        void ProcessEvent(MidiEvent evt)
        {
            switch (evt.Type)
            {
                case MidiEventType.NoteOn:
                    // velocity is 0-127, normalise to 0.0-1.0
                    synth.NoteOn(evt.Channel, evt.Data1, evt.Data2 / 127f);
                    break;

                case MidiEventType.NoteOff:
                    synth.NoteOff(evt.Channel, evt.Data1);
                    break;

                case MidiEventType.ProgramChange:
                    synth.ProgramChange(evt.Channel, evt.Data1);
                    break;

                case MidiEventType.ControlChange:
                    synth.ControlChange(evt.Channel, evt.Data1, evt.Data2);
                    break;

                case MidiEventType.PitchWheel:
                    // Combine 7-bit values into 14-bit value
                    int pitchWheelValue = ((evt.Data2 & 0x7F) << 7) | (evt.Data1 & 0x7F);
                    synth.PitchWheel(evt.Channel, pitchWheelValue);
                    break;

                case MidiEventType.ChannelPressure:
                    // Channel aftertouch — not supported by most synth backends.
                    // Could be implemented as a volume/modulation modifier if needed.
                    break;

                case MidiEventType.PolyphonicPressure:
                    // Polyphonic aftertouch — rarely used, not implemented.
                    break;

                // Meta and SysEx events are parsed but do not affect synthesis
                // (tempo changes are already baked into the tick→second conversion).
                case MidiEventType.Meta:
                case MidiEventType.SysEx:
                case MidiEventType.SysExContinuation:
                    break;
            }
        }

        void ThrowIfDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(MidiReader));
        }
    }
}
