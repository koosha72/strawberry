/*
 * Strawberry Game Engine
 * File: GravityAffector.cs
 * Author: Koosha Aabedini Nassab
 *
 * Particle affector applying constant gravity to particle velocity.
 */

using Strawberry.Math;

namespace Strawberry.Graphics.ParticleSystem
{
    /// <summary>
    /// Particle affector applying constant gravity to particle velocity.
    /// </summary>
    public class GravityAffector : IParticleAffector
    {
        public Vector2 Gravity = new Vector2(0f, 200f);

        public GravityAffector() { }

        public GravityAffector(float gravityX, float gravityY)
        {
            Gravity = new Vector2(gravityX, gravityY);
        }

        public GravityAffector(Vector2 gravity)
        {
            Gravity = gravity;
        }

        public void Update(ref Particle particle, float dt)
        {
            particle.Velocity += Gravity * dt;
        }
    }
}
