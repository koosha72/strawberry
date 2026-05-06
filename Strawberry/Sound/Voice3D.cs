using Strawberry.Math;

namespace Strawberry.Sound
{
    public abstract class Voice3D : Voice
    {
        /// <summary>
        /// The position of the source sound.
        /// </summary>
        public abstract Vector3 Position { get; set; }

        /// <summary>
        /// The direction to which the sound is projecting
        /// </summary>
        public abstract Vector3 Direction { get; set; }

        /// <summary>
        /// The velocity of the sound projection
        /// </summary>
        public abstract Vector3 Velocity { get; set; }

        /// <summary>
        /// The maximum distance the sound can be heared from.
        /// </summary>
        public abstract float MaxDistance { get; set; }

        public abstract float ReferenceDistance { get; set; }
    }
}
