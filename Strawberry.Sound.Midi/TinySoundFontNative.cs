using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Strawberry.Sound.Midi
{
    /// <summary>
    /// P/Invoke bindings for the TinySoundFont C library (libtsf) v0.9.
    /// Based on the exact function signatures from tsf.h.
    /// </summary>
    internal static class TinySoundFontNative
    {
        const string DllName = "tsf";

        // ---- Output mode enum ----
        // From tsf.h v0.9: auto-assigned starting from 0
        //   TSF_STEREO_INTERLEAVED = 0
        //   TSF_STEREO_UNWEAVED    = 1
        //   TSF_MONO               = 2
        public enum TSFOutputMode
        {
            StereoInterleaved  = 0,
            StereoUnweaved     = 1,
            Mono               = 2,
        }

        // ---- Stream struct for tsf_load ----
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int StreamReadCallback(IntPtr data, IntPtr ptr, uint size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int StreamSkipCallback(IntPtr data, uint count);

        [StructLayout(LayoutKind.Sequential)]
        public struct TsfStream
        {
            public IntPtr data;
            public StreamReadCallback read;
            public StreamSkipCallback skip;
        }

        // ---- API functions ----
        // All signatures taken directly from tsf.h v0.9

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr tsf_load(ref TsfStream stream);

        // tsf_load_memory(const void* buffer, int size)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr tsf_load_memory(IntPtr buffer, int size);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr tsf_copy(IntPtr f);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tsf_close(IntPtr f);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tsf_reset(IntPtr f);

        // int tsf_get_presetindex(const tsf* f, int bank, int preset_number)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_get_presetindex(IntPtr f, int bank, int presetNumber);

        // int tsf_get_presetcount(const tsf* f)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_get_presetcount(IntPtr f);

        // const char* tsf_get_presetname(const tsf* f, int preset_index)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr tsf_get_presetname(IntPtr f, int presetIndex);

        // const char* tsf_bank_get_presetname(const tsf* f, int bank, int preset_number)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr tsf_bank_get_presetname(IntPtr f, int bank, int presetNumber);

        // void tsf_set_output(tsf* f, enum TSFOutputMode outputmode, int samplerate, float global_gain_db)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tsf_set_output(IntPtr f, TSFOutputMode outputMode,
                                                  int sampleRate, float globalGainDB);

        /// <summary>tsf_set_output with raw int output mode.</summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tsf_set_output")]
        public static extern void tsf_set_output_raw(IntPtr f, int outputMode,
                                                      int sampleRate, float globalGainDB);

        // void tsf_set_volume(tsf* f, float global_gain)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tsf_set_volume(IntPtr f, float globalGain);

        // int tsf_set_max_voices(tsf* f, int max_voices)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_set_max_voices(IntPtr f, int maxVoices);

        // int tsf_note_on(tsf* f, int preset_index, int key, float vel)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_note_on(IntPtr f, int presetIndex, int key, float vel);

        // int tsf_bank_note_on(tsf* f, int bank, int preset_number, int key, float vel)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_bank_note_on(IntPtr f, int bank, int presetNumber, int key, float vel);

        // void tsf_note_off(tsf* f, int preset_index, int key)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tsf_note_off(IntPtr f, int presetIndex, int key);

        // int tsf_bank_note_off(tsf* f, int bank, int preset_number, int key)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_bank_note_off(IntPtr f, int bank, int presetNumber, int key);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tsf_note_off_all(IntPtr f);

        // int tsf_active_voice_count(tsf* f)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_active_voice_count(IntPtr f);

        // void tsf_render_short(tsf* f, short* buffer, int samples, int flag_mixing)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tsf_render_short(IntPtr f, short[] buffer,
                                                    int samples, int flagMixing);

        // void tsf_render_float(tsf* f, float* buffer, int samples, int flag_mixing)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tsf_render_float(IntPtr f, float[] buffer,
                                                    int samples, int flagMixing);

        // ---- Channel functions ----

        // int tsf_channel_set_presetindex(tsf* f, int channel, int preset_index)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_channel_set_presetindex(IntPtr f, int channel, int presetIndex);

        // int tsf_channel_set_presetnumber(tsf* f, int channel, int preset_number, int flag_mididrums)
        // In C++ the 4th param has a default value of 0, but in C it's always 4 params.
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_channel_set_presetnumber(IntPtr f, int channel,
                                                                int presetNumber, int flagMididrums);

        // int tsf_channel_set_bank(tsf* f, int channel, int bank)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_channel_set_bank(IntPtr f, int channel, int bank);

        // int tsf_channel_set_bank_preset(tsf* f, int channel, int bank, int preset_number)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_channel_set_bank_preset(IntPtr f, int channel,
                                                               int bank, int presetNumber);

        // int tsf_channel_set_pan(tsf* f, int channel, float pan)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_channel_set_pan(IntPtr f, int channel, float pan);

        // int tsf_channel_set_volume(tsf* f, int channel, float volume)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_channel_set_volume(IntPtr f, int channel, float volume);

        // int tsf_channel_set_pitchwheel(tsf* f, int channel, int pitch_wheel)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_channel_set_pitchwheel(IntPtr f, int channel, int pitchWheel);

        // int tsf_channel_set_pitchrange(tsf* f, int channel, float pitch_range)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_channel_set_pitchrange(IntPtr f, int channel, float pitchRange);

        // int tsf_channel_set_tuning(tsf* f, int channel, float tuning)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_channel_set_tuning(IntPtr f, int channel, float tuning);

        // int tsf_channel_set_sustain(tsf* f, int channel, int flag_sustain)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_channel_set_sustain(IntPtr f, int channel, int flagSustain);

        // int tsf_channel_note_on(tsf* f, int channel, int key, float vel)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_channel_note_on(IntPtr f, int channel, int key, float vel);

        // void tsf_channel_note_off(tsf* f, int channel, int key)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tsf_channel_note_off(IntPtr f, int channel, int key);

        // void tsf_channel_note_off_all(tsf* f, int channel)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tsf_channel_note_off_all(IntPtr f, int channel);

        // void tsf_channel_sounds_off_all(tsf* f, int channel)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tsf_channel_sounds_off_all(IntPtr f, int channel);

        // int tsf_channel_midi_control(tsf* f, int channel, int controller, int control_value)
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tsf_channel_midi_control(IntPtr f, int channel,
                                                            int controller, int controlValue);

        // ---- Helper: get preset name as managed string ----
        public static string GetPresetNameManaged(IntPtr f, int presetIndex)
        {
            IntPtr ptr = tsf_get_presetname(f, presetIndex);
            return ptr != IntPtr.Zero ? Marshal.PtrToStringUTF8(ptr) ?? "" : "";
        }
    }
}
