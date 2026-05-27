namespace Strawberry.Graphics.ParticleSystem
{
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
