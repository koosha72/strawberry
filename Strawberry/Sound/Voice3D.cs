/*
 * Strawberry Game Engine
 * File: Voice3D.cs
 * Author: Koosha Aabedini Nassab
 *
 * 3D voice subclass with spatial properties.
 */

using Strawberry.Math;

namespace Strawberry.Sound
{
    public abstract class Voice3D : Voice
    {
        /// <summary>
        /// Gets or sets position of the source sound.
        /// </summary>
        public abstract Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets direction to which the sound is projecting
        /// </summary>
        public abstract Vector3 Direction { get; set; }

        /// <summary>
        /// Gets or sets velocity of the sound projection
        /// </summary>
        public abstract Vector3 Velocity { get; set; }

        /// <summary>
        /// Gets or sets maximum distance the sound can be heard from.
        /// </summary>
        public abstract float MaxDistance { get; set; }
        
        public abstract float ReferenceDistance { get; set; }
    }
}
