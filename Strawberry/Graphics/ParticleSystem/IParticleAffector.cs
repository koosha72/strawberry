namespace Strawberry.Graphics.ParticleSystem
{
    public interface IParticleAffector
    {
        void Update(ref Particle particle, float dt);
    }
}
