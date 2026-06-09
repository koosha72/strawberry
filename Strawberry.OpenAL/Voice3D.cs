using Strawberry.Math;
using Strawberry.Sound;

namespace Strawberry.OpenAL
{
    public class Voice3D : Strawberry.Sound.Voice3D, IVoice
    {
        private Vector3 position;
        public override Vector3 Position
        {
            get => position;
            set
            {
                position = value;
                if (!IsVirtual && AL.IsSource(SourceInd))
                    AL.Source3f(SourceInd, ALSource3f.Position, value.X, value.Y, value.Z);
            }
        }

        private Vector3 direction;
        public override Vector3 Direction
        {
            get => direction;
            set
            {
                direction = value;
                if (!IsVirtual && AL.IsSource(SourceInd))
                    AL.Source3f(SourceInd, ALSource3f.Direction, value.X, value.Y, value.Z);
            }
        }

        private Vector3 velocity;
        public override Vector3 Velocity
        {
            get => velocity;
            set
            {
                velocity = value;
                if (!IsVirtual && AL.IsSource(SourceInd))
                    AL.Source3f(SourceInd, ALSource3f.Velocity, value.X, value.Y, value.Z);
            }
        }

        SoundBuffer buffer;
        public override Strawberry.Sound.SoundBuffer Buffer => buffer;

        public int SourceInd { get; set; }

        public bool IsVirtual => SourceInd == -1;

        // Cache Volume!
        private float cachedVolume = 1.0f;
        public override float Volume
        {
            get => cachedVolume; // Always return the cache
            set
            {
                cachedVolume = value; // Always update the cache
                if (!IsVirtual && AL.IsSource(SourceInd))
                    AL.Sourcef(SourceInd, ALSourcef.Gain, value);
            }
        }

        private float cachedFrequencyRatio = 1.0f;

        public override float FrequencyRatio
        {
            get => cachedFrequencyRatio;
            set
            {
                cachedFrequencyRatio = value;
                if (!IsVirtual && AL.IsSource(SourceInd))
                {
                    AL.Sourcef(SourceInd, ALSourcef.Gain, value);
                }
            }
        }

        private float maxDistance = 10000.0f;
        public override float MaxDistance
        {
            get => maxDistance;
            set
            {
                maxDistance = value;
                if (!IsVirtual && AL.IsSource(SourceInd))
                    AL.Sourcef(SourceInd, ALSourcef.MaxDistance, value);
            }
        }

        // Cache and apply ReferenceDistance
        private float referenceDistance = 100.0f;
        public override float ReferenceDistance
        {
            get => referenceDistance;
            set
            {
                referenceDistance = value;
                if (!IsVirtual && AL.IsSource(SourceInd))
                    AL.Sourcef(SourceInd, ALSourcef.ReferenceDistance, value);
            }
        }

        public override float CurrentPlayTime
        {
            get
            {
                if (IsVirtual || buffer == null) return 0;

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
                if (IsVirtual || buffer == null) return;

                if (AL.IsSource(SourceInd) && buffer.Seconds > 0)
                {
                    var size = AL.GetBufferi(buffer.ID, ALGetBufferi.Size);
                    var pos = value / buffer.Seconds;
                    AL.Sourcei(SourceInd, ALSourcei.ByteOffset, (int)(pos * size));
                }
            }
        }

        int priority = 0;
        public override int Priority => priority;

        public Voice3D(SoundBuffer soundBuffer, Voice3DSettings settings, int priority = 0)
        {
            SourceInd = -1;
            Position = settings.Position;
            Direction = settings.Direction;
            Velocity = settings.Velocity;

            buffer = soundBuffer;
            this.priority = priority;
        }

        public override bool IsPlaying()
        {
            if (IsVirtual) return false;

            if (AL.IsSource(SourceInd))
            {
                ALSourceState state = (ALSourceState)AL.GetSourcei(SourceInd, ALGetSourcei.SourceState);
                return state == ALSourceState.Playing;
            }
            return false;
        }

        public override void Stop()
        {
            (buffer.SoundManager as SoundManager)?.Stop(this);
        }

        public override void Pause()
        {
            if (!IsVirtual && AL.IsSource(SourceInd))
                AL.SourcePause(SourceInd);
        }

        public override void Resume()
        {
            if (!IsVirtual && AL.IsSource(SourceInd))
                AL.SourcePlay(SourceInd);
        }

        public override bool IsPaused()
        {
            if (IsVirtual) return false;

            if (AL.IsSource(SourceInd))
            {
                ALSourceState state = (ALSourceState)AL.GetSourcei(SourceInd, ALGetSourcei.SourceState);
                return state == ALSourceState.Paused;
            }
            return false;
        }


        public void ApplyCachedState()
        {
            if (IsVirtual) return;

            AL.Sourcef(SourceInd, ALSourcef.Gain, cachedVolume);
            AL.Sourcef(SourceInd, ALSourcef.Pitch, cachedFrequencyRatio);
            AL.Source3f(SourceInd, ALSource3f.Position, position.X, position.Y, position.Z);
            AL.Source3f(SourceInd, ALSource3f.Direction, direction.X, direction.Y, direction.Z);
            AL.Source3f(SourceInd, ALSource3f.Velocity, velocity.X, velocity.Y, velocity.Z);
            AL.Sourcef(SourceInd, ALSourcef.ReferenceDistance, referenceDistance);
            AL.Sourcef(SourceInd, ALSourcef.MaxDistance, maxDistance);
        }

        protected override void CleanUnmanaged()
        {
            SourceInd = -1;
            buffer = null;
            base.CleanUnmanaged();
        }
    }
}