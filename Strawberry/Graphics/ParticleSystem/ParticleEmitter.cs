using Strawberry.Math;

namespace Strawberry.Graphics.ParticleSystem
{
    public class ParticleEmitter
    {
        // Identity
        public string Name = "";
        public Vector2 Position = Vector2.Zero;
        public float Rotation = 0f;
        public bool Enabled = true;

        // Sprite
        public Sprite Sprite;

        // Configuration
        public ParticleInitiator Initiator { get; set; }
        public ParticleTimeline Timeline { get; set; }

        // Rate-based emission (particles per second)
        public float EmitRate = 10f;
        float emitAccumulator = 0f;

        // Burst emission (one-shot particles)
        public int BurstCount = 0;
        public bool HasBursted = false;

        // Per-emitter particle limit
        public int MaxParticles = 500;
        int aliveCount = 0;

        // Should this emitter auto-destroy when all particles are dead after a burst?
        public bool AutoDestroy = false;

        // Internal: index of this emitter in the layer's list
        internal int EmitterIndex = -1;

        public ParticleEmitter() { }

        /// <summary>
        /// Creates a new emitter with the given sprite and timeline.
        /// </summary>
        /// <param name="sprite">The sprite to use for particles.</param>
        /// <param name="initiator">The particle initiator to use for particles.</param>
        /// <param name="timeline">The timeline used to update and control particles.</param>
        public ParticleEmitter(Sprite sprite, ParticleInitiator initiator, ParticleTimeline timeline)
        {
            Sprite = sprite;
            Initiator = initiator;
            Timeline = timeline;
        }

        /// <summary>
        /// Emits particles into the pool. Called by ParticleLayer each frame.
        /// </summary>
        internal void Emit(Particle[] pool, int poolSize)
        {
            if (!Enabled || Initiator == null)
                return;

            float dt = FrameInfo.Information.DeltaTime;

            // Rate-based emission
            emitAccumulator += EmitRate * dt;
            while (emitAccumulator >= 1f && aliveCount < MaxParticles)
            {
                SpawnParticle(pool, poolSize);
                emitAccumulator -= 1f;
            }

            // Burst emission
            if (BurstCount > 0 && !HasBursted)
            {
                int toEmit = BurstCount;
                for (int i = 0; i < toEmit && aliveCount < MaxParticles; i++)
                    SpawnParticle(pool, poolSize);
                HasBursted = true;
            }
        }

        /// <summary>
        /// Updates a single particle using this emitter's timeline.
        /// Called by ParticleLayer during the update loop.
        /// </summary>
        internal void UpdateParticle(ref Particle particle, float dt)
        {
            if (Timeline != null)
                Timeline.Update(ref particle, dt);

            // Integrate motion
            particle.Position += particle.Velocity * dt;
            particle.Velocity += particle.Acceleration * dt;
            particle.Rotation += particle.AngularVelocity * dt;
        }

        /// <summary>
        /// Called when a particle owned by this emitter dies.
        /// </summary>
        internal void OnParticleDied()
        {
            aliveCount--;
        }

        /// <summary>
        /// Called when a particle owned by this emitter is spawned.
        /// </summary>
        internal void OnParticleSpawned()
        {
            aliveCount++;
        }

        /// <summary>
        /// Resets burst state so the emitter can burst again.
        /// </summary>
        public void ResetBurst()
        {
            HasBursted = false;
        }

        /// <summary>
        /// Whether this emitter is finished (burst completed and all particles dead).
        /// </summary>
        public bool IsFinished => HasBursted && aliveCount <= 0 && EmitRate <= 0f;

        void SpawnParticle(Particle[] pool, int poolSize)
        {
            for (int i = 0; i < poolSize; i++)
            {
                if (!pool[i].Alive)
                {
                    Initiator.Initialize(ref pool[i], Position, Sprite);
                    pool[i].EmitterIndex = EmitterIndex;
                    aliveCount++;
                    return;
                }
            }
        }
    }
}
