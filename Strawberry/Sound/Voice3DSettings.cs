/*
 * Strawberry Game Engine
 * File: Voice3DSettings.cs
 * Author: Koosha Aabedini Nassab
 *
 * Settings container for 3D voice playback.
 */

using Strawberry.Math;

namespace Strawberry.Sound
{
    public struct Voice3DSettings
    {
        /// <summary>
        /// Gets or sets position of the source sound.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Ges or sets direction to which the sound is projecting
        /// </summary>
        public Vector3 Direction { get; set; }

        /// <summary>
        /// Gets or sets velocity of the sound projection
        /// </summary>
        public Vector3 Velocity { get; set; }
    }
}
