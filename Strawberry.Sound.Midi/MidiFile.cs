using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Strawberry.Sound.Midi
{
    /// <summary>
    /// Parses a standard MIDI file (SMF) into a sorted list of <see cref="MidiEvent"/>s
    /// with absolute tick positions and computed time-in-seconds.
    /// </summary>
    /// <remarks>
    /// Supports Format 0 (single track) and Format 1 (multiple concurrent tracks).
    /// Format 2 (sequential tracks) is not supported and will throw.
    /// The parser is 100% managed C# with zero native dependencies, making it
    /// safe for WebAssembly / AOT environments.
    /// </remarks>
    public sealed class MidiFile
    {
        /// <summary>MIDI file format (0 = single track, 1 = multi track).</summary>
        public int Format { get; private set; }

        /// <summary>Number of tracks in the file.</summary>
        public int TrackCount { get; private set; }

        /// <summary>Ticks per quarter note (PPQN resolution).</summary>
        public int TicksPerQuarterNote { get; private set; }

        /// <summary>
        /// All MIDI events merged from all tracks, sorted by absolute ticks
        /// (and in original order for same-tick events).
        /// </summary>
        public List<MidiEvent> Events { get; private set; }

        /// <summary>Total duration of the MIDI file in seconds.</summary>
        public double DurationSeconds { get; private set; }

        // Internal tempo map: sorted list of (tick, microsecondsPerQuarterNote).
        readonly List<(long Tick, int UsPerQn)> tempoMap;

        public MidiFile(Stream stream)
        {
            Events = new List<MidiEvent>();
            tempoMap = new List<(long, int)>();
            Parse(stream);
        }

        // -----------------------------------------------------------------
        //  Public helpers
        // -----------------------------------------------------------------

        /// <summary>
        /// Converts a time in seconds back to an absolute tick position,
        /// respecting the tempo map.
        /// </summary>
        public long SecondsToTicks(double seconds)
        {
            if (seconds <= 0) return 0;

            double remaining = seconds;
            long currentTick = 0;

            for (int i = 0; i < tempoMap.Count; i++)
            {
                double timeToNext;
                if (i < tempoMap.Count - 1)
                {
                    long ticksToNext = tempoMap[i + 1].Tick - currentTick;
                    timeToNext = TicksToSeconds(ticksToNext, tempoMap[i].UsPerQn);
                }
                else
                {
                    timeToNext = double.MaxValue;
                }

                if (remaining <= timeToNext)
                {
                    return currentTick + SecondsToTicks(remaining, tempoMap[i].UsPerQn);
                }

                remaining -= timeToNext;
                currentTick = tempoMap[i + 1].Tick;
            }

            // Exhausted tempo map — use the last entry.
            return currentTick + SecondsToTicks(remaining, tempoMap[tempoMap.Count - 1].UsPerQn);
        }

        // -----------------------------------------------------------------
        //  Parsing
        // -----------------------------------------------------------------

        void Parse(Stream stream)
        {
            using var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true);

            // ---- MThd header ----
            string headerId = ReadAscii(reader, 4);
            if (headerId != "MThd")
                throw new InvalidDataException(
                    $"Invalid MIDI file: expected MThd header, got \"{headerId}\".");

            int headerLength = ReadInt32BE(reader);
            Format = ReadInt16BE(reader);
            TrackCount = ReadInt16BE(reader);
            int division = ReadInt16BE(reader);

            if (Format == 2)
                throw new NotSupportedException(
                    "MIDI Format 2 (sequential tracks) is not supported.");

            if ((division & 0x8000) != 0)
                throw new NotSupportedException(
                    "SMPTE-based time division is not supported; only ticks-per-quarter-note is supported.");

            TicksPerQuarterNote = division & 0x7FFF;

            // Default tempo: 120 BPM = 500 000 us/qn
            tempoMap.Add((0, 500_000));

            // ---- Track chunks ----
            // Read each track's raw bytes first, then parse from an isolated MemoryStream.
            // This avoids BinaryReader internal buffering causing stream position misalignment
            // between tracks (BinaryReader reads ahead into its buffer, making
            // BaseStream.Position unreliable for boundary tracking).
            var trackEvents = new List<List<MidiEvent>>(TrackCount);
            for (int t = 0; t < TrackCount; t++)
            {
                string trackId = ReadAscii(reader, 4);
                if (trackId != "MTrk")
                    throw new InvalidDataException(
                        $"Invalid MIDI file: expected MTrk header for track {t}, got \"{trackId}\".");

                int trackLength = ReadInt32BE(reader);
                byte[] trackData = reader.ReadBytes(trackLength);

                if (trackData.Length < trackLength)
                    throw new InvalidDataException(
                        $"Unexpected end of stream in track {t}: expected {trackLength} bytes, got {trackData.Length}.");

                trackEvents.Add(ParseTrack(trackData, t));
            }

            // ---- Merge & sort ----
            Events = MergeTracks(trackEvents);

            // ---- Tick → second conversion ----
            ConvertTicksToSeconds();

            // ---- Duration ----
            CalculateDuration();
        }

        List<MidiEvent> ParseTrack(byte[] trackData, int trackIndex)
        {
            using var trackStream = new MemoryStream(trackData, writable: false);
            using var trackReader = new BinaryReader(trackStream);

            var events = new List<MidiEvent>();
            long absoluteTicks = 0;
            int runningStatus = 0;

            while (trackStream.Position < trackStream.Length)
            {
                long delta = ReadVarLen(trackReader);
                absoluteTicks += delta;

                // Read the next byte directly (do NOT use PeekChar — ASCII encoding
                // corrupts bytes >= 0x80, which includes ALL MIDI status bytes).
                int nextByte = trackReader.ReadByte();

                int data1PreRead; // set when running status consumes a data byte
                if ((nextByte & 0x80) != 0)
                {
                    // New status byte
                    runningStatus = nextByte;
                    data1PreRead = -1; // no data byte consumed yet
                }
                else
                {
                    // Running status — this byte is the first data byte
                    data1PreRead = nextByte;
                }

                var evt = new MidiEvent
                {
                    AbsoluteTicks = absoluteTicks,
                };

                if (runningStatus == 0xFF)
                {
                    // ---- Meta event ----
                    evt.Type = MidiEventType.Meta;
                    evt.MetaType = (MidiMetaType)trackReader.ReadByte();
                    int length = (int)ReadVarLen(trackReader);
                    evt.MetaData = trackReader.ReadBytes(length);

                    if (evt.MetaType == MidiMetaType.Tempo && evt.MetaData.Length == 3)
                    {
                        int us = (evt.MetaData[0] << 16) | (evt.MetaData[1] << 8) | evt.MetaData[2];
                        tempoMap.Add((absoluteTicks, us));
                    }

                    if (evt.MetaType == MidiMetaType.EndOfTrack)
                        break; // end of this track
                }
                else if (runningStatus == 0xF0 || runningStatus == 0xF7)
                {
                    // ---- SysEx ----
                    evt.Type = runningStatus == 0xF0
                        ? MidiEventType.SysEx
                        : MidiEventType.SysExContinuation;
                    int length = (int)ReadVarLen(trackReader);
                    evt.SysExData = trackReader.ReadBytes(length);
                }
                else
                {
                    // ---- Channel voice / mode message ----
                    int highNibble = runningStatus & 0xF0;
                    evt.Type = (MidiEventType)highNibble;
                    evt.Channel = runningStatus & 0x0F;

                    switch (highNibble)
                    {
                        case 0x80: // Note Off — 2 data bytes
                        case 0x90: // Note On
                        case 0xA0: // Polyphonic Key Pressure
                        case 0xB0: // Control Change
                        case 0xE0: // Pitch Wheel
                            evt.Data1 = data1PreRead >= 0 ? data1PreRead : trackReader.ReadByte();
                            evt.Data2 = trackReader.ReadByte();
                            // Note On with velocity 0 is equivalent to Note Off
                            if (highNibble == 0x90 && evt.Data2 == 0)
                                evt.Type = MidiEventType.NoteOff;
                            break;

                        case 0xC0: // Program Change — 1 data byte
                        case 0xD0: // Channel Key Pressure — 1 data byte
                            evt.Data1 = data1PreRead >= 0 ? data1PreRead : trackReader.ReadByte();
                            evt.Data2 = 0;
                            break;

                        default:
                            // Unknown status byte — skip
                            break;
                    }
                }

                events.Add(evt);
            }

            return events;
        }

        /// <summary>
        /// Merge all tracks into a single event list, sorted by absolute ticks.
        /// Stable sort preserves original order for same-tick events.
        /// </summary>
        static List<MidiEvent> MergeTracks(List<List<MidiEvent>> trackEvents)
        {
            var merged = new List<MidiEvent>();
            foreach (var track in trackEvents)
                merged.AddRange(track);

            merged.Sort((a, b) => a.AbsoluteTicks.CompareTo(b.AbsoluteTicks));
            return merged;
        }

        /// <summary>
        /// Walk through the sorted events, advancing the tempo map cursor,
        /// and compute <see cref="MidiEvent.TimeSeconds"/> for every event.
        /// </summary>
        void ConvertTicksToSeconds()
        {
            tempoMap.Sort((a, b) => a.Tick.CompareTo(b.Tick));

            double currentTime = 0;
            long lastTick = 0;
            int tempoIdx = 0;

            for (int i = 0; i < Events.Count; i++)
            {
                var evt = Events[i];

                // Advance through tempo changes that fall before this event
                while (tempoIdx < tempoMap.Count - 1 &&
                       tempoMap[tempoIdx + 1].Tick <= evt.AbsoluteTicks)
                {
                    currentTime += TicksToSeconds(
                        tempoMap[tempoIdx + 1].Tick - lastTick,
                        tempoMap[tempoIdx].UsPerQn);
                    lastTick = tempoMap[tempoIdx + 1].Tick;
                    tempoIdx++;
                }

                // Add the remaining segment from lastTick to this event's tick
                currentTime += TicksToSeconds(
                    evt.AbsoluteTicks - lastTick,
                    tempoMap[tempoIdx].UsPerQn);
                lastTick = evt.AbsoluteTicks;

                evt.TimeSeconds = currentTime;
            }
        }

        void CalculateDuration()
        {
            if (Events.Count == 0)
            {
                DurationSeconds = 0;
                return;
            }

            double maxTime = 0;
            foreach (var evt in Events)
            {
                if (evt.TimeSeconds > maxTime)
                    maxTime = evt.TimeSeconds;
            }

            DurationSeconds = maxTime;
        }

        // -----------------------------------------------------------------
        //  Conversion helpers
        // -----------------------------------------------------------------

        double TicksToSeconds(long ticks, int usPerQn)
        {
            return (double)ticks * usPerQn / (TicksPerQuarterNote * 1_000_000.0);
        }

        long SecondsToTicks(double seconds, int usPerQn)
        {
            return (long)(seconds * TicksPerQuarterNote * 1_000_000.0 / usPerQn);
        }

        // -----------------------------------------------------------------
        //  Binary reader helpers (big-endian MIDI format)
        // -----------------------------------------------------------------

        static string ReadAscii(BinaryReader reader, int count)
        {
            byte[] bytes = reader.ReadBytes(count);
            return Encoding.ASCII.GetString(bytes);
        }

        static short ReadInt16BE(BinaryReader reader)
        {
            byte[] b = reader.ReadBytes(2);
            return (short)((b[0] << 8) | b[1]);
        }

        static int ReadInt32BE(BinaryReader reader)
        {
            byte[] b = reader.ReadBytes(4);
            return (b[0] << 24) | (b[1] << 16) | (b[2] << 8) | b[3];
        }

        static long ReadVarLen(BinaryReader reader)
        {
            long value = 0;
            int b;
            int iterations = 0;
            do
            {
                b = reader.ReadByte();
                value = (value << 7) | (b & 0x7F);
                if (++iterations > 4)
                    throw new InvalidDataException(
                        "Malformed MIDI variable-length value: exceeds 4 bytes (28-bit maximum).");
            } while ((b & 0x80) != 0);
            return value;
        }
    }
}
