/*
 * Strawberry Game Engine
 * File: DragAffector.cs
 * Author: Koosha Aabedini Nassab
 *
 * Affector applying drag/air resistance to particle velocity.
 */

namespace Strawberry.Graphics.ParticleSystem
{
    /// <summary>
    /// An affector applying drag/air resistance to particle velocity.
    /// </summary>
    public class DragAffector : IParticleAffector
    {
        public float Drag = 0.98f;

        public DragAffector() { }

        public DragAffector(float drag)
        {
            Drag = drag;
        }

        public void Update(ref Particle particle, float dt)
        {
            particle.Velocity *= Drag;
        }
    }
}
