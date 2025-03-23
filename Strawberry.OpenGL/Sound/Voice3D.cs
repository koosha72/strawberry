using OpenTK.Audio.OpenAL;
using Strawberry.Math;
using Strawberry.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D;

namespace Strawberry.OpenGL.Sound
{
    internal class Voice3D : Voice, IVoice3D
    {
        private Vector3 position;

        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                AL.Source(SourceInd, ALSource3f.Position, value.X, value.Y, value.Z);
            }
        }
        private Vector3 direction;

        public Vector3 Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                AL.Source(SourceInd, ALSource3f.Direction, value.X, value.Y, value.Z);
            }
        }

        private Vector3 velocity;

        public Vector3 Velocity
        {
            get { return velocity; }
            set
            {
                velocity = value;
                AL.Source(SourceInd, ALSource3f.Velocity, value.X, value.Y, value.Z);
            }
        }

        public float MaxDistance { get; set; }
        public float RefrenceDistance { get; set; }

        public Voice3D(SoundBuffer soundBuffer,Voice3DSettings settings,int ind)
            : base(soundBuffer, ind)
        {
            Position = settings.Position;
            Direction = settings.Direction;
            Velocity = settings.Velocity;
        }
    }
}
