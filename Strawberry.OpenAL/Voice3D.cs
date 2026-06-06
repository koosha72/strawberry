using Strawberry.Math;
using Strawberry.Sound;

namespace Strawberry.OpenAL
{
    public class Voice3D : Strawberry.Sound.Voice3D, IVoice
    {
        private Vector3 position;

        public override Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                AL.Source3f(SourceInd, ALSource3f.Position, value.X, value.Y, value.Z);
            }
        }
        private Vector3 direction;

        public override Vector3 Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                AL.Source3f(SourceInd, ALSource3f.Direction, value.X, value.Y, value.Z);
            }
        }

        private Vector3 velocity;

        public override Vector3 Velocity
        {
            get { return velocity; }
            set
            {
                velocity = value;
                AL.Source3f(SourceInd, ALSource3f.Velocity, value.X, value.Y, value.Z);
            }
        }

        SoundBuffer buffer;
        public override Strawberry.Sound.SoundBuffer Buffer { get { return buffer; } }

        private bool isRecycled = false;
        public void MarkRecycled() { isRecycled = true; }

        public int SourceInd { get; set; }

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

        public override float MaxDistance { get; set; }
        public override float ReferenceDistance { get; set; }

        public Voice3D(SoundBuffer soundBuffer, Voice3DSettings settings, int ind)
        {
            SourceInd = ind;
            Position = settings.Position;
            Direction = settings.Direction;
            Velocity = settings.Velocity;
            buffer = soundBuffer;
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
                if (!isRecycled)
                {
                    AL.SourceStop(SourceInd);
                    AL.DeleteSource(SourceInd);
                }
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
