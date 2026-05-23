namespace Strawberry.Sound.Midi
{
    using Math = System.Math;
    
    /// <summary>
    /// Pure managed C# MIDI synthesizer backend. Produces basic waveform output
    /// (sine, square, sawtooth, triangle, noise) with simple ADSR envelopes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This backend has ZERO native dependencies and is fully compatible with
    /// WebAssembly, AOT compilation, and any platform where .NET runs.
    /// It produces recognisable but not SoundFont-quality audio.
    /// For high-quality rendering on desktop, use <see cref="TinySoundFontBackend"/>.
    /// </para>
    /// <para>
    /// Waveform selection is mapped loosely from General MIDI program numbers:
    /// program ranges 0-7 → sine/triangle (piano-like), 8-15 → triangle (bell-like),
    /// 16-31 → square/saw (organ/guitar-like), etc.
    /// </para>
    /// </remarks>
    public sealed class ManagedSynthBackend : ISynthBackend
    {
        const int MaxVoices = 48;
        const int MidiChannels = 16;
        const double TwoPi = 2.0 * Math.PI;

        enum Waveform : byte
        {
            Sine = 0,
            Square = 1,
            Sawtooth = 2,
            Triangle = 3,
            Noise = 4,
        }

        enum EnvPhase : byte
        {
            Attack = 0,
            Decay = 1,
            Sustain = 2,
            Release = 3,
            Off = 4,
        }

        struct Voice
        {
            public bool Active;
            public int Channel;
            public int Note;
            public float Velocity;
            public double Frequency;
            public Waveform Waveform;
            public double Phase;

            // Envelope
            public EnvPhase EnvPhase;
            public double EnvValue;          // 0..1
            public double EnvTime;           // seconds in current phase
            public double ReleaseStartValue; // EnvValue at the moment release begins

            // Per-voice ADSR (copied from program settings)
            public double Attack;
            public double Decay;
            public double Sustain;
            public double Release;
        }

        struct ChannelState
        {
            public int Program;
            public float Volume;       // CC7  (0..1)
            public float Expression;   // CC11 (0..1)
            public float Pan;          // CC10 (0..1, 0.5 = center)
            public double PitchBend;   // multiplier (1.0 = no bend)
            public bool SustainPedal;  // CC64
            public Waveform Waveform;
            public double Attack;
            public double Decay;
            public double Sustain;
            public double Release;
        }

        readonly Voice[] voices;
        readonly ChannelState[] channels;

        // Simple deterministic pseudo-random for noise waveform (avoids allocation)
        uint noiseSeed = 12345;

        public int SampleRate { get; }
        public int Channels { get; }

        public ManagedSynthBackend(int sampleRate = 44100, int channels = 2)
        {
            SampleRate = sampleRate;
            Channels = channels;
            voices = new Voice[MaxVoices];
            this.channels = new ChannelState[MidiChannels];

            for (int i = 0; i < MidiChannels; i++)
            {
                this.channels[i] = CreateDefaultChannelState(i);
            }
        }

        // ---------------------------------------------------------
        //  ISynthBackend
        // ---------------------------------------------------------

        public void NoteOn(int channel, int key, float velocity)
        {
            if (velocity <= 0f) { NoteOff(channel, key); return; }

            // Find a free voice, or steal the voice closest to silence in release
            int voiceIdx = -1;
            int stealCandidate = -1;
            double maxReleaseTime = -1;

            for (int i = 0; i < MaxVoices; i++)
            {
                if (!voices[i].Active)
                {
                    voiceIdx = i;
                    break;
                }
                // Steal the voice furthest into release (closest to silence)
                if (voices[i].EnvPhase == EnvPhase.Release && voices[i].EnvTime > maxReleaseTime)
                {
                    maxReleaseTime = voices[i].EnvTime;
                    stealCandidate = i;
                }
            }

            if (voiceIdx < 0) voiceIdx = stealCandidate >= 0 ? stealCandidate : 0;

            ref var ch = ref channels[channel];

            voices[voiceIdx] = new Voice
            {
                Active = true,
                Channel = channel,
                Note = key,
                Velocity = velocity,
                Frequency = MidiNoteToFrequency(key) * ch.PitchBend,
                Waveform = ch.Waveform,
                Phase = 0,
                EnvPhase = EnvPhase.Attack,
                EnvValue = 0,
                EnvTime = 0,
                Attack = ch.Attack,
                Decay = ch.Decay,
                Sustain = ch.Sustain,
                Release = ch.Release,
            };
        }

        public void NoteOff(int channel, int key)
        {
            for (int i = 0; i < MaxVoices; i++)
            {
                if (voices[i].Active && voices[i].Channel == channel && voices[i].Note == key)
                {
                    // If sustain pedal is held, don't enter release yet
                    if (channels[channel].SustainPedal)
                        continue;

                    voices[i].ReleaseStartValue = voices[i].EnvValue;
                    voices[i].EnvPhase = EnvPhase.Release;
                    voices[i].EnvTime = 0;
                }
            }
        }

        public void NoteOffAll()
        {
            for (int i = 0; i < MaxVoices; i++)
            {
                if (voices[i].Active)
                {
                    voices[i].ReleaseStartValue = voices[i].EnvValue;
                    voices[i].EnvPhase = EnvPhase.Release;
                    voices[i].EnvTime = 0;
                }
            }
        }

        public void ProgramChange(int channel, int program)
        {
            channels[channel].Program = program;
            ApplyProgramWaveform(channel, program);
        }

        public void ControlChange(int channel, int controller, int value)
        {
            float normalized = value / 127f;
            switch (controller)
            {
                case MidiController.ChannelVolumeMSB:
                    channels[channel].Volume = normalized;
                    break;
                case MidiController.ExpressionControllerMSB:
                    channels[channel].Expression = normalized;
                    break;
                case MidiController.PanMSB:
                    channels[channel].Pan = normalized;
                    break;
                case MidiController.SustainPedal:
                    bool pedalOn = value >= 64;
                    channels[channel].SustainPedal = pedalOn;
                    // Release sustained notes when pedal is lifted
                    if (!pedalOn)
                    {
                        for (int i = 0; i < MaxVoices; i++)
                        {
                            if (voices[i].Active && voices[i].Channel == channel &&
                                voices[i].EnvPhase != EnvPhase.Release)
                            {
                                voices[i].ReleaseStartValue = voices[i].EnvValue;
                                voices[i].EnvPhase = EnvPhase.Release;
                                voices[i].EnvTime = 0;
                            }
                        }
                    }
                    break;
                case MidiController.AllNotesOff:
                case MidiController.AllSoundOff:
                    for (int i = 0; i < MaxVoices; i++)
                    {
                        if (voices[i].Active && voices[i].Channel == channel)
                        {
                            voices[i].EnvPhase = EnvPhase.Off;
                            voices[i].Active = false;
                        }
                    }
                    break;
                case MidiController.ResetAllControllers:
                    channels[channel].Volume = 1f;
                    channels[channel].Expression = 1f;
                    channels[channel].Pan = 0.5f;
                    channels[channel].PitchBend = 1.0;
                    break;
            }
        }

        public void PitchWheel(int channel, int value)
        {
            // 14-bit value: 0-16383, centre = 8192
            // Default pitch bend range = 2 semitones
            const double semitoneRange = 2.0;
            double semitones = ((value - 8192) / 8192.0) * semitoneRange;
            channels[channel].PitchBend = Math.Pow(2.0, semitones / 12.0);

            // Update active voices on this channel
            for (int i = 0; i < MaxVoices; i++)
            {
                if (voices[i].Active && voices[i].Channel == channel)
                {
                    voices[i].Frequency = MidiNoteToFrequency(voices[i].Note) * channels[channel].PitchBend;
                }
            }
        }

        public void Reset()
        {
            for (int i = 0; i < MaxVoices; i++)
            {
                voices[i].Active = false;
                voices[i].EnvPhase = EnvPhase.Off;
            }
            for (int ch = 0; ch < MidiChannels; ch++)
            {
                channels[ch] = CreateDefaultChannelState(ch);
            }
        }

        public int RenderShort(short[] buffer, int offset, int sampleFrames)
        {
            if (sampleFrames <= 0) return 0;

            double sampleDuration = 1.0 / SampleRate;

            for (int f = 0; f < sampleFrames; f++)
            {
                float sampleL = 0f, sampleR = 0f;

                for (int v = 0; v < MaxVoices; v++)
                {
                    if (!voices[v].Active) continue;

                    ref var voice = ref voices[v];

                    // Advance envelope
                    AdvanceEnvelope(ref voice, sampleDuration);
                    if (voice.EnvPhase == EnvPhase.Off)
                    {
                        voice.Active = false;
                        continue;
                    }

                    // Generate waveform sample
                    float raw = GenerateSample(ref voice);
                    float amp = (float)voice.EnvValue * voice.Velocity *
                                channels[voice.Channel].Volume *
                                channels[voice.Channel].Expression;

                    float sample = raw * amp * 0.25f; // master volume scaling

                    // Pan: 0 = left, 0.5 = center, 1 = right
                    float pan = channels[voice.Channel].Pan;
                    float leftGain = 1f - pan;
                    float rightGain = pan;

                    sampleL += sample * (leftGain * 2f);   // compensate for panning
                    sampleR += sample * (rightGain * 2f);

                    // Advance phase
                    voice.Phase += voice.Frequency * sampleDuration;
                    if (voice.Phase >= 1.0) voice.Phase -= 1.0;
                }

                // Clamp
                sampleL = Math.Clamp(sampleL, -1f, 1f);
                sampleR = Math.Clamp(sampleR, -1f, 1f);

                // Convert to 16-bit
                if (Channels == 1)
                {
                    float mono = (sampleL + sampleR) * 0.5f;
                    buffer[offset + f] = (short)(mono * 32767f);
                }
                else
                {
                    buffer[offset + f * 2] = (short)(sampleL * 32767f);
                    buffer[offset + f * 2 + 1] = (short)(sampleR * 32767f);
                }
            }

            return sampleFrames;
        }

        public void Dispose()
        {
            // Nothing to dispose — pure managed
        }

        // ---------------------------------------------------------
        //  Waveform generation
        // ---------------------------------------------------------

        float GenerateSample(ref Voice voice)
        {
            double phase = voice.Phase;
            return voice.Waveform switch
            {
                Waveform.Sine => (float)Math.Sin(phase * TwoPi),
                Waveform.Square => phase < 0.5 ? 1f : -1f,
                Waveform.Sawtooth => (float)(2.0 * phase - 1.0),
                Waveform.Triangle => (float)(Math.Abs(4.0 * phase - 2.0) - 1.0),
                Waveform.Noise => GenerateNoise(),
                _ => 0f,
            };
        }

        float GenerateNoise()
        {
            // xorshift32 — fast, deterministic, no allocation
            noiseSeed ^= noiseSeed << 13;
            noiseSeed ^= noiseSeed >> 17;
            noiseSeed ^= noiseSeed << 5;
            return (float)(noiseSeed / (double)uint.MaxValue) * 2f - 1f;
        }

        // ---------------------------------------------------------
        //  Envelope
        // ---------------------------------------------------------

        static void AdvanceEnvelope(ref Voice voice, double dt)
        {
            voice.EnvTime += dt;

            switch (voice.EnvPhase)
            {
                case EnvPhase.Attack:
                    voice.EnvValue = voice.Attack > 0
                        ? (float)(voice.EnvTime / voice.Attack)
                        : 1f;
                    if (voice.EnvValue >= 1f)
                    {
                        voice.EnvValue = 1f;
                        voice.EnvPhase = EnvPhase.Decay;
                        voice.EnvTime = 0;
                    }
                    break;

                case EnvPhase.Decay:
                    voice.EnvValue = voice.Decay > 0
                        ? (float)(1.0 - (1.0 - voice.Sustain) * (voice.EnvTime / voice.Decay))
                        : (float)voice.Sustain;
                    if (voice.EnvValue <= voice.Sustain)
                    {
                        voice.EnvValue = (float)voice.Sustain;
                        voice.EnvPhase = EnvPhase.Sustain;
                        voice.EnvTime = 0;
                    }
                    break;

                case EnvPhase.Sustain:
                    voice.EnvValue = (float)voice.Sustain;
                    break;

                case EnvPhase.Release:
                    voice.EnvValue = voice.Release > 0
                        ? (float)(voice.ReleaseStartValue * (1.0 - voice.EnvTime / voice.Release))
                        : 0f;
                    if (voice.EnvValue <= 0f)
                    {
                        voice.EnvValue = 0f;
                        voice.EnvPhase = EnvPhase.Off;
                    }
                    break;
            }
        }

        // ---------------------------------------------------------
        //  Program → waveform mapping (loose GM grouping)
        // ---------------------------------------------------------

        void ApplyProgramWaveform(int channel, int program)
        {
            ref var ch = ref channels[channel];
            int group = program / 8;

            (ch.Waveform, ch.Attack, ch.Decay, ch.Sustain, ch.Release) = group switch
            {
                // Pianos (0-7): sine/triangle, fast attack, medium release
                0 => (Waveform.Sine, 0.005, 0.3, 0.4, 0.4),
                // Chromatic percussion (8-15): triangle, percussive
                1 => (Waveform.Triangle, 0.001, 0.2, 0.3, 0.3),
                // Organs (16-23): sine, slow attack, sustained
                2 => (Waveform.Sine, 0.05, 0.1, 0.8, 0.1),
                // Guitars (24-31): sawtooth, plucky
                3 => (Waveform.Sawtooth, 0.002, 0.2, 0.5, 0.3),
                // Bass (32-39): square/saw, deep
                4 => (Waveform.Square, 0.005, 0.2, 0.6, 0.2),
                // Strings (40-47): sawtooth, slow attack
                5 => (Waveform.Sawtooth, 0.08, 0.1, 0.7, 0.3),
                // Ensemble (48-55): sawtooth, lush
                6 => (Waveform.Sawtooth, 0.06, 0.2, 0.6, 0.4),
                // Brass (56-63): square, bright
                7 => (Waveform.Square, 0.02, 0.1, 0.7, 0.15),
                // Reed (64-71): square, nasal
                8 => (Waveform.Square, 0.01, 0.15, 0.6, 0.2),
                // Pipe (72-79): sine, breathy
                9 => (Waveform.Sine, 0.03, 0.1, 0.5, 0.3),
                // Synth Lead (80-87): saw/square, sharp
                10 => (Waveform.Sawtooth, 0.005, 0.1, 0.7, 0.2),
                // Synth Pad (88-95): sine/triangle, slow
                11 => (Waveform.Sine, 0.1, 0.3, 0.6, 0.5),
                // Synth Effects (96-103): various
                12 => (Waveform.Noise, 0.001, 0.2, 0.3, 0.3),
                // Ethnic (104-111): triangle
                13 => (Waveform.Triangle, 0.005, 0.2, 0.5, 0.3),
                // Percussive (112-119): noise
                14 => (Waveform.Noise, 0.001, 0.1, 0.2, 0.15),
                // Sound effects (120-127): noise
                _ => (Waveform.Noise, 0.001, 0.15, 0.3, 0.2),
            };

            // Channel 9 (10 in 1-indexed) is standard GM drum channel
            if (channel == 9)
            {
                ch.Waveform = Waveform.Noise;
                ch.Attack = 0.001;
                ch.Decay = 0.15;
                ch.Sustain = 0.1;
                ch.Release = 0.1;
            }
        }

        // ---------------------------------------------------------
        //  Helpers
        // ---------------------------------------------------------

        static double MidiNoteToFrequency(int note)
        {
            // A4 = MIDI 69 = 440 Hz
            return 440.0 * Math.Pow(2.0, (note - 69) / 12.0);
        }

        static ChannelState CreateDefaultChannelState(int channel)
        {
            // General MIDI standard: all channels default to program 0 (Acoustic Grand Piano)
            // except channel 10 (index 9) which is the drum channel.
            var ch = new ChannelState
            {
                Program = 0,
                Volume = 1f,
                Expression = 1f,
                Pan = 0.5f,
                PitchBend = 1.0,
                SustainPedal = false,
                Waveform = Waveform.Sine,
                Attack = 0.005,
                Decay = 0.3,
                Sustain = 0.4,
                Release = 0.4,
            };

            // Channel 10 (index 9) is the standard GM drum/percussion channel
            if (channel == 9)
            {
                ch.Waveform = Waveform.Noise;
                ch.Attack = 0.001;
                ch.Decay = 0.15;
                ch.Sustain = 0.1;
                ch.Release = 0.1;
            }

            return ch;
        }
    }
}
