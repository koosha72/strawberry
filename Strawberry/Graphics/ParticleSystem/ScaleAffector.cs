/*
 * Strawberry Game Engine
 * File: ScaleAffector.cs
 * Author: Koosha Aabedini Nassab
 *
 * Affector that scales particle size over their lifetime.
 */

namespace Strawberry.Graphics.ParticleSystem
{
    /// <summary>
    /// Affector that scales particle size over their lifetime
    /// </summary>
    public class ScaleAffector : IParticleAffector
    {
        public InterpolationCurve ScaleCurve = new InterpolationCurve();

        public ScaleAffector() { }

        public ScaleAffector(float startScale, float endScale)
        {
            ScaleCurve.AddKeyframe(0f, startScale);
            ScaleCurve.AddKeyframe(1f, endScale);
        }

        public void Update(ref Particle particle, float dt)
        {
            particle.Scale = ScaleCurve.Evaluate(particle.NormalizedAge);
        }
    }
}
