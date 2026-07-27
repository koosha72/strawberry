using Strawberry.Sound;

namespace Strawberry.OpenAL
{
    public class Voice : Strawberry.Sound.Voice, IVoice
    {
        SoundBuffer buffer;
        public override Strawberry.Sound.SoundBuffer Buffer => buffer;

        internal int SourceInd { get; set; }

        private float cachedVolume = 1.0f;
        float cachedFrequencyRatio = 1.0f;

        public bool IsVirtual => SourceInd == -1;

        public override float CurrentPlayTime
        {
            get
            {
                if (IsVirtual || buffer == null)
                    return 0;

                if (AL.IsSource(SourceInd))
                {
                    var pos = AL.GetSourcei(SourceInd, ALGetSourcei.ByteOffset);
                    var size = AL.GetBufferi(buffer.ID, ALGetBufferi.Size);

                    if (size == 0) return 0;
                    return pos / (float)size * buffer.Seconds;
                }
                return 0;
            }
            set
            {
                if (IsVirtual || buffer == null)
                    return;

                if (AL.IsSource(SourceInd))
                {
                    var size = AL.GetBufferi(buffer.ID, ALGetBufferi.Size);
                    if (buffer.Seconds > 0 && size > 0)
                    {
                        var pos = value / buffer.Seconds;
                        AL.Sourcei(SourceInd, ALSourcei.ByteOffset, (int)(pos * size));
                    }
                }
            }
        }

        public override float Volume
        {
            get => cachedVolume;
            set
            {
                cachedVolume = value;
                if (!IsVirtual && AL.IsSource(SourceInd))
                {
                    AL.Sourcef(SourceInd, ALSourcef.Gain, value);
                }
            }
        }

        public override float FrequencyRatio
        {
            get => cachedFrequencyRatio;
            set
            {
                cachedFrequencyRatio = value;
                if (!IsVirtual && AL.IsSource(SourceInd))
                {
                    AL.Sourcef(SourceInd, ALSourcef.Pitch, value);
                }
            }
        }


        int IVoice.SourceInd { get => SourceInd; set => SourceInd = value; }
        int priority = 0;
        public override int Priority => priority;

        public Voice(SoundBuffer buffer, int priority = 0)
        {
            this.buffer = buffer;
            SourceInd = -1;
            this.priority = priority;
        }

        public override bool IsPlaying()
        {
            if (IsVirtual) return false;

            ALSourceState state = (ALSourceState)AL.GetSourcei(SourceInd, ALGetSourcei.SourceState);
            return state == ALSourceState.Playing;
        }

        public override void Stop()
        {
            // The SoundManager handles returning the source to the pool
            (buffer.SoundManager as SoundManager)?.Stop(this);
        }

        public override void Pause()
        {
            if (IsVirtual) return;
            if (AL.IsSource(SourceInd))
                AL.SourcePause(SourceInd);
        }

        public override void Resume()
        {
            if (IsVirtual) return;
            if (AL.IsSource(SourceInd))
                AL.SourcePlay(SourceInd);
        }

        public override bool IsPaused()
        {
            if (IsVirtual) return false;

            ALSourceState state = (ALSourceState)AL.GetSourcei(SourceInd, ALGetSourcei.SourceState);
            return state == ALSourceState.Paused;
        }

        public void ApplyCachedState()
        {
            if (IsVirtual) return;

            AL.Sourcef(SourceInd, ALSourcef.Gain, cachedVolume);
            AL.Sourcef(SourceInd, ALSourcef.Pitch, cachedFrequencyRatio);
        }

        protected override void CleanUnmanaged()
        {
            SourceInd = -1;
            buffer = null;
            base.CleanUnmanaged();
        }
    }
}