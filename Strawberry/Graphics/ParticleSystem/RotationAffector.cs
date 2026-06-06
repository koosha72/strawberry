/*
 * Strawberry Game Engine
 * File: RotationAffector.cs
 * Author: Koosha Aabedini Nassab
 *
 * Affector that modifies particle rotation over time.
 */

namespace Strawberry.Graphics.ParticleSystem
{
    /// <summary>
    /// Affector that modifies particle rotation over time
    /// </summary>
    public class RotationAffector : IParticleAffector
    {
        public InterpolationCurve AngularVelocityCurve = new InterpolationCurve();

        public RotationAffector() { }

        public RotationAffector(float startAngularVelocity, float endAngularVelocity)
        {
            AngularVelocityCurve.AddKeyframe(0f, startAngularVelocity);
            AngularVelocityCurve.AddKeyframe(1f, endAngularVelocity);
        }

        public void Update(ref Particle particle, float dt)
        {
            particle.AngularVelocity = AngularVelocityCurve.Evaluate(particle.NormalizedAge);
        }
    }
}
