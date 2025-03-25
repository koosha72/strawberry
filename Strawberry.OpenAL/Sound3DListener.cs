using Strawberry.Math;
using Strawberry.Sound;

namespace Strawberry.OpenAL
{
    public class Sound3DListener : Base, ISound3DListener
    {
        private SoundManager soundManager;

        public ISoundManager SoundManager
        {
            get { return soundManager; }
        }


        private Vector3 position;

        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                AL.Listener3f(ALListener3f.Position, value.X, value.Y, value.Z);
            }
        }

        private Vector3 lookAt;

        public Vector3 LookAt
        {
            get { return lookAt; }
            set
            {
                lookAt = value;
                AL.Listenerfv(ALListenerfv.Orientation, ref lookAt, ref up);
            }
        }
        private Vector3 up;

        public Vector3 Up
        {
            get { return up; }
            set
            {
                up = value;
                AL.Listenerfv(ALListenerfv.Orientation, ref lookAt, ref up);
            }
        }

        private Vector3 velocity;

        public Vector3 Velocity
        {
            get { return velocity; }
            set
            {
                velocity = value;
                AL.Listener3f(ALListener3f.Velocity, value.X, value.Y, value.Z);
            }
        }

        private FallOffMode fallOffMode;

        public FallOffMode FallOffMode
        {
            get
            {
                return fallOffMode;
            }
            set
            {
                fallOffMode = value;
                AL.DistanceModel(GetDistanceModel(value));
            }
        }

        public bool IsActive
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
    }
}
