using OpenTK.Audio.OpenAL;
using Strawberry.Sound;

namespace Strawberry.OpenGL.Sound
{
    public class Voice : IVoice
    {
        SoundBuffer buffer;
        public virtual ISoundBuffer Buffer { get { return buffer; } internal set { buffer = value as SoundBuffer; } }

        internal int SourceInd { get; set; }

        public float CurrentPlayTime
        {
            get
            {
                if (AL.IsSource(SourceInd))
                {
                    var pos = AL.GetSource(SourceInd, ALGetSourcei.ByteOffset);
                    var size = AL.GetBuffer(buffer.ID, ALGetBufferi.Size);

                    if (size == 0)
                        return 0;
                    return pos / (float)size * buffer.Seconds;
                }
                return 0;
            }
            set
            {
                var size = AL.GetBuffer(buffer.ID, ALGetBufferi.Size);
                var pos = value / buffer.Seconds;
                AL.Source(SourceInd, ALSourcei.ByteOffset, (int)(pos * size));
            }
        }

        public Voice(SoundBuffer buffer, int ind)
        {
            this.buffer = buffer;
            this.SourceInd = ind;
        }

        public bool IsPlaying()
        {
            if (AL.IsSource(SourceInd))
            {
                ALSourceState state = (ALSourceState)AL.GetSource(SourceInd, ALGetSourcei.SourceState);
                return state == ALSourceState.Playing;
            }
            return false;
        }

        public void Stop()
        {
            (buffer.SoundManager as SoundManager).Stop(this);
        }

        public void Pause()
        {
            if (AL.IsSource(SourceInd))
                AL.SourcePause(SourceInd);
        }

        public void Resume()
        {
            if (AL.IsSource(SourceInd))
                AL.SourcePlay(SourceInd);
        }

        public bool IsPaused()
        {
            if (AL.IsSource(SourceInd))
            {
                ALSourceState state = (ALSourceState)AL.GetSource(SourceInd, ALGetSourcei.SourceState);
                return state == ALSourceState.Paused;
            }
            return false;
        }
    }
}
