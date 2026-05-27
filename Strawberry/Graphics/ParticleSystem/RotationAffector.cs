namespace Strawberry.Graphics.ParticleSystem
{
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
