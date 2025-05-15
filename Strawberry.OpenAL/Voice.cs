using Strawberry.Sound;

namespace Strawberry.OpenAL
{
    public class Voice : Strawberry.Sound.Voice, IVoice
    {
        SoundBuffer buffer;
        public override Strawberry.Sound.SoundBuffer Buffer { get { return buffer; } }

        internal int SourceInd { get; set; }

        public override float CurrentPlayTime
        {
            get
            {
                if (AL.IsSource(SourceInd))
                {
                    var pos = AL.GetSourcei(SourceInd, ALGetSourcei.ByteOffset);
                    var size = AL.GetBufferi(buffer.ID, ALGetBufferi.Size);

                    if (size == 0)
                        return 0;
                    return pos / (float)size * buffer.Seconds;
                }
                return 0;
            }
            set
            {
                var size = AL.GetBufferi(buffer.ID, ALGetBufferi.Size);
                var pos = value / buffer.Seconds;
                AL.Sourcei(SourceInd, ALSourcei.ByteOffset, (int)(pos * size));
            }
        }

        public override float Volume
        {
            get
            {
                return AL.GetSourcef(SourceInd, ALSourcef.Gain);
            }
            set
            {
                AL.Sourcef(SourceInd, ALSourcef.Gain, value);
            }
        }

        int IVoice.SourceInd { get => SourceInd; set => SourceInd = value; }

        public Voice(SoundBuffer buffer, int ind)
        {
            this.buffer = buffer;
            this.SourceInd = ind;
        }

        public override bool IsPlaying()
        {
            if (AL.IsSource(SourceInd))
            {
                ALSourceState state = (ALSourceState)AL.GetSourcei(SourceInd, ALGetSourcei.SourceState);
                return state == ALSourceState.Playing;
            }
            return false;
        }

        public override void Stop()
        {
            (buffer.SoundManager as SoundManager).Stop(this);
        }

        public override void Pause()
        {
            if (AL.IsSource(SourceInd))
                AL.SourcePause(SourceInd);
        }

        public override void Resume()
        {
            if (AL.IsSource(SourceInd))
                AL.SourcePlay(SourceInd);
        }

        public override bool IsPaused()
        {
            if (AL.IsSource(SourceInd))
            {
                ALSourceState state = (ALSourceState)AL.GetSourcei(SourceInd, ALGetSourcei.SourceState);
                return state == ALSourceState.Paused;
            }
            return false;
        }

        protected override void CleanUnmanaged()
        {
            if (AL.IsSource(SourceInd))
            {
                AL.SourceStop(SourceInd);
                AL.DeleteSource(SourceInd);
                SourceInd = 0;
                buffer = null;
            }
            base.CleanUnmanaged();
        }

        public void SetBuffer(SoundBuffer soundBuffer)
        {
            buffer = soundBuffer as SoundBuffer;
        }
    }
}
