using System;

namespace Strawberry.Sound.Midi
{
    /// <summary>
    /// MIDI channel voice message types (high nibble of status byte).
    /// </summary>
    public enum MidiEventType
    {
        NoteOff              = 0x80,
        NoteOn               = 0x90,
        PolyphonicPressure   = 0xA0,
        ControlChange        = 0xB0,
        ProgramChange        = 0xC0,
        ChannelPressure      = 0xD0,
        PitchWheel           = 0xE0,
        Meta                 = 0xFF,
        SysEx                = 0xF0,
        SysExContinuation    = 0xF7,
    }

    /// <summary>
    /// MIDI meta event types (second byte after 0xFF).
    /// </summary>
    public enum MidiMetaType
    {
        SequenceNumber   = 0x00,
        Text             = 0x01,
        Copyright        = 0x02,
        TrackName        = 0x03,
        InstrumentName   = 0x04,
        Lyrics           = 0x05,
        Marker           = 0x06,
        CuePoint         = 0x07,
        ChannelPrefix    = 0x20,
        MidiPort         = 0x21,
        EndOfTrack       = 0x2F,
        Tempo            = 0x51,
        SmpteOffset      = 0x54,
        TimeSignature    = 0x58,
        KeySignature     = 0x59,
        Proprietary      = 0x7F,
    }

    /// <summary>
    /// Standard MIDI Control Change controller numbers (CC).
    /// </summary>
    public static class MidiController
    {
        public const int BankSelectMSB       = 0;
        public const int ModulationWheelMSB  = 1;
        public const int BreathControllerMSB = 2;
        public const int FootControllerMSB   = 4;
        public const int PortamentoTimeMSB   = 5;
        public const int DataEntryMSB        = 6;
        public const int ChannelVolumeMSB    = 7;
        public const int BalanceMSB          = 8;
        public const int PanMSB              = 10;
        public const int ExpressionControllerMSB = 11;
        public const int EffectControl1MSB   = 12;
        public const int EffectControl2MSB   = 13;
        public const int BankSelectLSB       = 32;
        public const int ModulationWheelLSB  = 33;
        public const int BreathControllerLSB = 34;
        public const int FootControllerLSB   = 36;
        public const int PortamentoTimeLSB   = 37;
        public const int DataEntryLSB        = 38;
        public const int ChannelVolumeLSB    = 39;
        public const int BalanceLSB          = 40;
        public const int PanLSB              = 42;
        public const int ExpressionControllerLSB = 43;
        public const int SustainPedal        = 64;
        public const int PortamentoOnOff     = 65;
        public const int Sostenuto           = 66;
        public const int SoftPedal           = 67;
        public const int LegatoFootswitch    = 68;
        public const int Hold2               = 69;
        public const int SoundVariation      = 70;
        public const int SoundTimbre         = 71;
        public const int SoundReleaseTime    = 72;
        public const int SoundAttackTime     = 73;
        public const int SoundBrightness     = 74;
        public const int SoundDecayTime      = 75;
        public const int VibratoRate         = 76;
        public const int VibratoDepth        = 77;
        public const int VibratoDelay        = 78;
        public const int ReverbSendLevel     = 91;
        public const int ChorusSendLevel     = 93;
        public const int AllSoundOff         = 120;
        public const int ResetAllControllers = 121;
        public const int AllNotesOff         = 123;
        public const int OmniModeOff         = 124;
        public const int OmniModeOn          = 125;
        public const int MonoModeOn          = 126;
        public const int PolyModeOn          = 127;
    }

    /// <summary>
    /// Represents a single parsed MIDI event with absolute timing.
    /// Events are sorted by AbsoluteTicks after track merging.
    /// TimeSeconds is computed from the tempo map after parsing.
    /// </summary>
    public sealed class MidiEvent
    {
        /// <summary>Absolute tick position from the start of the file.</summary>
        public long AbsoluteTicks { get; set; }

        /// <summary>Time in seconds from the start of the file (computed from tempo map).</summary>
        public double TimeSeconds { get; set; }

        /// <summary>Type of the MIDI event (channel voice, meta, sysex).</summary>
        public MidiEventType Type { get; set; }

        /// <summary>MIDI channel (0-15) for channel voice messages.</summary>
        public int Channel { get; set; }

        /// <summary>
        /// First data byte: note number, controller number, program number,
        /// or pitch wheel LSB depending on event type.
        /// </summary>
        public int Data1 { get; set; }

        /// <summary>
        /// Second data byte: velocity, controller value, or pitch wheel MSB
        /// depending on event type. Zero for single-byte messages.
        /// </summary>
        public int Data2 { get; set; }

        /// <summary>Meta event subtype (only valid when Type == Meta).</summary>
        public MidiMetaType MetaType { get; set; }

        /// <summary>Raw bytes of meta event data (only valid when Type == Meta).</summary>
        public byte[] MetaData { get; set; }

        /// <summary>Raw bytes of SysEx data (only valid when Type == SysEx or SysExContinuation).</summary>
        public byte[] SysExData { get; set; }

        public override string ToString()
        {
            return Type switch
            {
                MidiEventType.Meta => $"Meta({MetaType}) @{TimeSeconds:F3}s tick={AbsoluteTicks}",
                MidiEventType.SysEx or MidiEventType.SysExContinuation =>
                    $"{Type} @{TimeSeconds:F3}s tick={AbsoluteTicks} len={SysExData?.Length ?? 0}",
                _ => $"{Type} ch={Channel} d1={Data1} d2={Data2} @{TimeSeconds:F3}s tick={AbsoluteTicks}",
            };
        }
    }
}
