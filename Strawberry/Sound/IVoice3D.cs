using Strawberry.Math;

namespace Strawberry.Sound
{
    public interface IVoice3D : IVoice
    {
        /// <summary>
        /// The position of the source sound.
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// The direction to which the sound is projecting
        /// </summary>
        Vector3 Direction { get; set; }

        /// <summary>
        /// The velocity of the sound projection
        /// </summary>
        Vector3 Velocity { get; set; }

        /// <summary>
        /// The maximum distance the sound can be heared from.
        /// </summary>
        float MaxDistance { get; set; }

        float RefrenceDistance { get; set; }
    }
}
