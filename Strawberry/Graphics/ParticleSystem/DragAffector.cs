namespace Strawberry.Graphics.ParticleSystem
{
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
