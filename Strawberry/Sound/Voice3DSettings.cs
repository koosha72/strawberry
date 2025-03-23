using Strawberry.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strawberry.Sound
{
    public struct Voice3DSettings
    {
        /// <summary>
        /// The position of the source sound.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The direction to which the sound is projecting
        /// </summary>
        public Vector3 Direction { get; set; }

        /// <summary>
        /// The velocity of the sound projection
        /// </summary>
        public Vector3 Velocity { get; set; }
    }
}
