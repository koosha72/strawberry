/*
 * Strawberry Game Engine
 * File: ParticleEmitter.cs
 * Author: Koosha Aabedini Nassab
 *
 * Responsible for emitting particles according to initiation and timeline settings.
 */

using Strawberry.Math;

namespace Strawberry.Graphics.ParticleSystem
{
    /// <summary>
    /// Emits particles over time and controls per-particle behavior through a timeline.
    /// </summary>
    public class ParticleEmitter
    {
        // Identity
        /// <summary>
        /// Gets or sets the emitter name.
        /// </summary>
        public string Name = "";

        /// <summary>
        /// Gets or sets the emitter position.
        /// </summary>
        public Vector2 Position = Vector2.Zero;

        /// <summary>
        /// Gets or sets the emitter rotation in degrees.
        /// </summary>
        public float Rotation = 0f;

        /// <summary>
        /// Gets or sets a value indicating whether the emitter is enabled.
        /// </summary>
        public bool Enabled = true;

        // Sprite
        /// <summary>
        /// Gets or sets the sprite used by emitted particles.
        /// </summary>
        public Sprite Sprite;

        // Configuration
        /// <summary>
        /// Gets or sets the initiator responsible for initializing new particles.
        /// </summary>
        public ParticleInitiator Initiator { get; set; }

        /// <summary>
        /// Gets or sets the timeline used to update particle behavior.
        /// </summary>
        public ParticleTimeline Timeline { get; set; }

        // Rate-based emission (particles per second)
        /// <summary>
        /// Gets or sets the number of particles emitted per second.
        /// </summary>
        public float EmitRate = 10f;
        float emitAccumulator = 0f;

        // Burst emission (one-shot particles)
        /// <summary>
        /// Gets or sets the number of particles to emit in a burst.
        /// </summary>
        public int BurstCount = 0;

        /// <summary>
        /// Gets or sets a value indicating whether the burst has already occurred.
        /// </summary>
        public bool HasBursted = false;

        // Per-emitter particle limit
        /// <summary>
        /// Gets or sets the maximum number of active particles for this emitter.
        /// </summary>
        public int MaxParticles = 500;
        int aliveCount = 0;

        // Should this emitter auto-destroy when all particles are dead after a burst?
        /// <summary>
        /// Gets or sets a value indicating whether the emitter should auto-destroy when finished.
        /// </summary>
        public bool AutoDestroy = false;

        // Internal: index of this emitter in the layer's list
        internal int EmitterIndex = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleEmitter"/> class.
        /// </summary>
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
        /// Emits particles into the pool. Called by <see cref="ParticleLayer"/> each frame.
        /// </summary>
        /// <param name="pool">The particle pool to allocate from.</param>
        /// <param name="poolSize">The size of the particle pool.</param>
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
        /// Called by <see cref="ParticleLayer"/> during the update loop.
        /// </summary>
        /// <param name="particle">The particle to update.</param>
        /// <param name="dt">The elapsed time in seconds.</param>
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
        /// Resets the burst state so the emitter can burst again.
        /// </summary>
        public void ResetBurst()
        {
            HasBursted = false;
        }

        /// <summary>
        /// Gets a value indicating whether this emitter is finished.
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
