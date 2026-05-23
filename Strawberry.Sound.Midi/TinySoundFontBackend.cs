using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Strawberry.Sound.Midi
{
    using Math = System.Math;

    /// <summary>
    /// High-quality MIDI synthesis backend using TinySoundFont v0.9 via P/Invoke.
    /// Requires a compiled libtsf native library on the system library path.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This backend uses the <b>channel-based</b> TSF API (tsf_channel_note_on,
    /// tsf_channel_note_off, tsf_channel_set_presetnumber, etc.) which correctly
    /// routes notes to per-channel presets — matching how MIDI playback works.
    /// </para>
    /// <para>
    /// The low-level tsf_note_on/tsf_note_off functions take a <c>preset_index</c>,
    /// NOT a channel number. Using those would cause wrong instruments and crashes.
    /// </para>
    /// </remarks>
    public sealed class TinySoundFontBackend : ISynthBackend
    {
        IntPtr tsfHandle;
        bool disposed;
        GCHandle dataHandle;

        // Reusable float buffer to avoid per-call GC allocation
        float[]? renderBuffer;
        int renderBufferCapacity;

        public int SampleRate { get; private set; }
        public int Channels { get; private set; }

        /// <summary>
        /// Load a SoundFont from a file path and initialise the synth.
        /// </summary>
        /// <param name="soundFontPath">Path to a .sf2 SoundFont file.</param>
        /// <param name="sampleRate">Output sample rate (default 44100).</param>
        /// <param name="channels">Output channels: 1 = mono, 2 = stereo (default 2).</param>
        /// <param name="globalGainDB">Global gain in dB (default -6 to prevent clipping).</param>
        public TinySoundFontBackend(string soundFontPath, int sampleRate = 44100,
                                     int channels = 2, float globalGainDB = -6f)
        {
            if (!File.Exists(soundFontPath))
                throw new FileNotFoundException("SoundFont file not found.", soundFontPath);

            byte[] data = File.ReadAllBytes(soundFontPath);
            tsfHandle = LoadFromMemory(data);
            Initialize(sampleRate, channels, globalGainDB);
        }

        /// <summary>
        /// Load a SoundFont from a stream and initialise the synth.
        /// </summary>
        public TinySoundFontBackend(Stream soundFontStream, int sampleRate = 44100,
                                     int channels = 2, float globalGainDB = -6f)
        {
            byte[] data;
            if (soundFontStream is MemoryStream ms && ms.TryGetBuffer(out _))
            {
                data = ms.ToArray();
            }
            else
            {
                using var temp = new MemoryStream();
                soundFontStream.CopyTo(temp);
                data = temp.ToArray();
            }

            tsfHandle = LoadFromMemory(data);
            Initialize(sampleRate, channels, globalGainDB);
        }

        // ---------------------------------------------------------
        //  ISynthBackend implementation
        // ---------------------------------------------------------

        public void NoteOn(int channel, int key, float velocity)
        {
            ThrowIfDisposed();
            // Use the CHANNEL-BASED note_on, not tsf_note_on (which takes preset_index!)
            TinySoundFontNative.tsf_channel_note_on(tsfHandle, channel, key, velocity);
        }

        public void NoteOff(int channel, int key)
        {
            ThrowIfDisposed();
            // Use the CHANNEL-BASED note_off
            TinySoundFontNative.tsf_channel_note_off(tsfHandle, channel, key);
        }

        public void NoteOffAll()
        {
            ThrowIfDisposed();
            TinySoundFontNative.tsf_note_off_all(tsfHandle);
        }

        public void ProgramChange(int channel, int program)
        {
            ThrowIfDisposed();
            // flag_mididrums: 0 = normal channel, 1 = MIDI drum channel
            int flagMididrums = (channel == 9) ? 1 : 0;
            TinySoundFontNative.tsf_channel_set_presetnumber(
                tsfHandle, channel, program, flagMididrums);
        }

        public void ControlChange(int channel, int controller, int value)
        {
            ThrowIfDisposed();
            TinySoundFontNative.tsf_channel_midi_control(tsfHandle, channel, controller, value);
        }

        public void PitchWheel(int channel, int value)
        {
            ThrowIfDisposed();
            // tsf.h v0.9 uses tsf_channel_set_pitchwheel (not tsf_channel_pitch_wheel)
            TinySoundFontNative.tsf_channel_set_pitchwheel(tsfHandle, channel, value);
        }

        public void Reset()
        {
            ThrowIfDisposed();
            TinySoundFontNative.tsf_reset(tsfHandle);

            // Re-apply default program assignments for all 16 channels
            for (int ch = 0; ch < 16; ch++)
            {
                int flagMididrums = (ch == 9) ? 1 : 0;
                TinySoundFontNative.tsf_channel_set_presetnumber(
                    tsfHandle, ch, 0, flagMididrums);
            }
        }

        public int RenderShort(short[] buffer, int offset, int sampleFrames)
        {
            ThrowIfDisposed();

            if (sampleFrames <= 0) return 0;

            int floatCount = sampleFrames * Channels;

            // Ensure reusable buffer is large enough
            if (renderBuffer == null || renderBufferCapacity < floatCount)
            {
                renderBufferCapacity = Math.Max(floatCount, 4096);
                renderBuffer = new float[renderBufferCapacity];
            }

            // Render audio from TSF
            TinySoundFontNative.tsf_render_float(tsfHandle, renderBuffer, sampleFrames, 0);

            // Peak-limit: find maximum amplitude and scale down if it would clip
            float peak = 0.0001f;
            for (int i = 0; i < floatCount; i++)
            {
                float abs = Math.Abs(renderBuffer[i]);
                if (abs > peak) peak = abs;
            }

            float scale = peak > 0.9f ? 0.9f / peak : 1f;

            // Convert float → short
            for (int i = 0; i < floatCount; i++)
            {
                float v = renderBuffer[i] * scale;
                if (v > 1.0f) v = 1.0f;
                else if (v < -1.0f) v = -1.0f;
                buffer[offset + i] = (short)(v * 32767f);
            }

            return sampleFrames;
        }

        // ---------------------------------------------------------
        //  Internal helpers
        // ---------------------------------------------------------

        IntPtr LoadFromMemory(byte[] data)
        {
            dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);

            IntPtr dataPtr = dataHandle.AddrOfPinnedObject();
            IntPtr handle = TinySoundFontNative.tsf_load_memory(dataPtr, data.Length);

            if (handle == IntPtr.Zero)
            {
                dataHandle.Free();
                throw new InvalidDataException(
                    "TinySoundFont failed to load the SoundFont data. " +
                    "Ensure the file is a valid .sf2 SoundFont.");
            }

            return handle;
        }

        void Initialize(int sampleRate, int channels, float globalGainDB)
        {
            SampleRate = sampleRate;
            Channels = channels;

            // tsf.h v0.9 enum values:
            //   TSF_STEREO_INTERLEAVED = 0  ← we want this for stereo
            //   TSF_STEREO_UNWEAVED    = 1
            //   TSF_MONO               = 2
            int outputMode = (channels == 1) ? 2 : 0;

            TinySoundFontNative.tsf_set_output_raw(tsfHandle, outputMode, sampleRate, globalGainDB);

            // Assign General MIDI default programs for all 16 channels
            for (int ch = 0; ch < 16; ch++)
            {
                int flagMididrums = (ch == 9) ? 1 : 0;
                TinySoundFontNative.tsf_channel_set_presetnumber(
                    tsfHandle, ch, 0, flagMididrums);
            }
        }

        void ThrowIfDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(TinySoundFontBackend));
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            if (tsfHandle != IntPtr.Zero)
            {
                TinySoundFontNative.tsf_close(tsfHandle);
                tsfHandle = IntPtr.Zero;
            }

            if (dataHandle.IsAllocated)
                dataHandle.Free();

            renderBuffer = null;
        }
    }
}
