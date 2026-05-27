using Strawberry.Math;

namespace Strawberry.Graphics.ParticleSystem
{
    public struct Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public Color Color;
        public float Scale;
        public float Rotation;
        public float AngularVelocity;
        public float Lifetime;
        public float Age;
        public bool Alive;
        public int ImageIndex;
        public int EmitterIndex;

        public float NormalizedAge => Lifetime > 0f ? Age / Lifetime : 0f;
    }
}
