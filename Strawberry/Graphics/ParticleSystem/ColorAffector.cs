/*
 * Strawberry Game Engine
 * File: ColorAffector.cs
 * Author: Koosha Aabedini Nassab
 *
 * Affector that modifies particle color using gradients over lifetime.
 */

namespace Strawberry.Graphics.ParticleSystem
{
    /// <summary>
    /// An affector that modifies particle color using gradients over lifetime.
    /// </summary>
    public class ColorAffector : IParticleAffector
    {
        public ColorGradient Gradient = new ColorGradient();

        public ColorAffector() { }

        public ColorAffector(Color start, Color end)
        {
            Gradient.AddKeyframe(0f, start);
            Gradient.AddKeyframe(1f, end);
        }

        public void Update(ref Particle particle, float dt)
        {
            particle.Color = Gradient.Evaluate(particle.NormalizedAge);
        }
    }
}
