using System;

namespace Strawberry.Sound.Midi
{
    /// <summary>
    /// Abstraction over a MIDI synthesis engine.
    /// Two implementations are provided:
    /// <list type="bullet">
    ///   <item><see cref="TinySoundFontBackend"/> — native TinySoundFont via P/Invoke (desktop, high quality)</item>
    ///   <item><see cref="ManagedSynthBackend"/> — pure managed C# synth (WASM / AOT safe fallback)</item>
    /// </list>
    /// </summary>
    public interface ISynthBackend : IDisposable
    {
        /// <summary>Output sample rate in Hz.</summary>
        int SampleRate { get; }

        /// <summary>Number of output audio channels (1 = mono, 2 = stereo).</summary>
        int Channels { get; }

        /// <summary>Start a note on the given channel.</summary>
        /// <param name="channel">MIDI channel (0-15).</param>
        /// <param name="key">MIDI note number (0-127).</param>
        /// <param name="velocity">Velocity 0.0-1.0 (normalized from 0-127).</param>
        void NoteOn(int channel, int key, float velocity);

        /// <summary>Release a note on the given channel.</summary>
        void NoteOff(int channel, int key);

        /// <summary>Release all active notes across all channels.</summary>
        void NoteOffAll();

        /// <summary>Change the program (instrument) on a channel.</summary>
        void ProgramChange(int channel, int program);

        /// <summary>Set a MIDI controller value on a channel.</summary>
        void ControlChange(int channel, int controller, int value);

        /// <summary>Set the pitch wheel position on a channel.</summary>
        /// <param name="channel">MIDI channel.</param>
        /// <param name="value">Combined 14-bit value (0-16383, center = 8192).</param>
        void PitchWheel(int channel, int value);

        /// <summary>
        /// Reset the synthesizer to a clean state: all notes off,
        /// all controllers reset to defaults, all programs reset.
        /// </summary>
        void Reset();

        /// <summary>
        /// Render <paramref name="sampleFrames"/> frames of audio into <paramref name="buffer"/>
        /// starting at <paramref name="offset"/> (in shorts, not frames).
        /// </summary>
        /// <param name="buffer">Target buffer (interleaved 16-bit PCM).</param>
        /// <param name="offset">Starting index in the buffer (in shorts).</param>
        /// <param name="sampleFrames">
        /// Number of sample frames to render. One frame = <see cref="Channels"/> shorts.
        /// </param>
        /// <returns>Number of sample frames actually rendered.</returns>
        int RenderShort(short[] buffer, int offset, int sampleFrames);
    }
}
