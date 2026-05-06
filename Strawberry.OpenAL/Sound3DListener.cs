using Strawberry.Math;
using Strawberry.Sound;

namespace Strawberry.OpenAL
{
    public class Sound3DListener : Strawberry.Sound.Sound3DListener
    {
        private ISoundManager soundManager;

        public override ISoundManager SoundManager
        {
            get { return soundManager; }
        }


        private Vector3 position;

        public override Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                if (IsActive)
                {
                    AL.Listener3f(ALListener3f.Position, position.X, position.Y, position.Z);
                }
            }
        }

        private Vector3 lookAt;

        public override Vector3 LookAt
        {
            get { return lookAt; }
            set
            {
                lookAt = value;
                if (IsActive)
                {
                    AL.Listenerfv(ALListenerfv.Orientation, ref lookAt, ref up);
                }
            }
        }
        private Vector3 up;

        public override Vector3 Up
        {
            get { return up; }
            set
            {
                up = value;
                if (IsActive)
                {
                    AL.Listenerfv(ALListenerfv.Orientation, ref lookAt, ref up);
                }
            }
        }

        private Vector3 velocity;

        public override Vector3 Velocity
        {
            get { return velocity; }
            set
            {
                velocity = value;
                if (IsActive)
                {
                    AL.Listener3f(ALListener3f.Velocity, velocity.X, velocity.Y, velocity.Z);
                }
            }
        }

        private FallOffMode fallOffMode;

        public override FallOffMode FallOffMode
        {
            get
            {
                return fallOffMode;
            }
            set
            {
                fallOffMode = value;
                if (IsActive)
                {
                    AL.DistanceModel(GetDistanceModel(fallOffMode));
                }
            }
        }

        public override bool IsActive
        {
            get { return soundManager.ActiveListener == this; }
        }

        public Sound3DListener(SoundManager soundManager)
        {
            this.soundManager = soundManager;
        }

        ALDistanceModel GetDistanceModel(FallOffMode fallOffMode)
        {
            switch (fallOffMode)
            {
                case FallOffMode.None:
                    return ALDistanceModel.None;
                case FallOffMode.ExponentDistance:
                    return ALDistanceModel.ExponentDistance;
                case FallOffMode.ExponentDistanceClamped:
                    return ALDistanceModel.ExponentDistanceClamped;
                case FallOffMode.InverseDistance:
                    return ALDistanceModel.InverseDistance;
                case FallOffMode.InverseDistanceClamped:
                    return ALDistanceModel.InverseDistanceClamped;
                case FallOffMode.LinearDistance:
                    return ALDistanceModel.LinearDistance;
                case FallOffMode.LinearDistanceClamped:
                    return ALDistanceModel.LinearDistanceClamped;
                default:
                    return ALDistanceModel.None;
            }
        }

        public override void Activate()
        {
            AL.Listener3f(ALListener3f.Position, position.X, position.Y, position.Z);
            AL.Listenerfv(ALListenerfv.Orientation, ref lookAt, ref up);
            AL.Listener3f(ALListener3f.Velocity, velocity.X, velocity.Y, velocity.Z);
            AL.DistanceModel(GetDistanceModel(fallOffMode));
        }
    }
}
