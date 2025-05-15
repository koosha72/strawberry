using Strawberry.Math;

namespace Strawberry.Sound
{
    public abstract class Sound3DListener : DisposableReferenceObject
    {
        /// <summary>
        /// The position of the sound listener
        /// </summary>
        public abstract Vector3 Position { get; set; }

        /// <summary>
        /// The direction of the listenr
        /// </summary>
        public abstract Vector3 LookAt { get; set; }

        /// <summary>
        /// The up direction of the game. the default value is {0.0f,0.0f,1.0f}
        /// </summary>
        public abstract Vector3 Up { get; set; }

        /// <summary>
        /// The velocity of the listenr
        /// </summary>
        public abstract Vector3 Velocity { get; set; }

        /// <summary>
        /// The calculation mode.
        /// </summary>
        public abstract FallOffMode FallOffMode { get; set; }

        /// <summary>
        /// Returns whether the listener is active or not.
        /// NOTE: You can only have 1 active listener.
        /// </summary>
        public abstract bool IsActive { get; }

        public abstract ISoundManager SoundManager { get; }
    }
}
