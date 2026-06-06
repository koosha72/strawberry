/*
 * Strawberry Game Engine
 * File: Particle.cs
 * Author: Koosha Aabedini Nassab
 *
 * Particle structure containing position, velocity, color and lifecycle info.
 */

using Strawberry.Math;

namespace Strawberry.Graphics.ParticleSystem
{
    /// <summary>
    /// Represents the state of a single particle managed by the particle system.
    /// </summary>
    public struct Particle
    {
        /// <summary>
        /// Gets or sets the particle position.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// Gets or sets the particle velocity.
        /// </summary>
        public Vector2 Velocity;

        /// <summary>
        /// Gets or sets the particle acceleration.
        /// </summary>
        public Vector2 Acceleration;

        /// <summary>
        /// Gets or sets the particle color.
        /// </summary>
        public Color Color;

        /// <summary>
        /// Gets or sets the particle scale.
        /// </summary>
        public float Scale;

        /// <summary>
        /// Gets or sets the particle rotation in degrees.
        /// </summary>
        public float Rotation;

        /// <summary>
        /// Gets or sets the angular velocity of the particle.
        /// </summary>
        public float AngularVelocity;

        /// <summary>
        /// Gets or sets the total lifetime of the particle.
        /// </summary>
        public float Lifetime;

        /// <summary>
        /// Gets or sets the current age of the particle.
        /// </summary>
        public float Age;

        /// <summary>
        /// Gets or sets a value indicating whether the particle is alive.
        /// </summary>
        public bool Alive;

        /// <summary>
        /// Gets or sets the sprite image index for this particle.
        /// </summary>
        public int ImageIndex;

        /// <summary>
        /// Gets or sets the index of the emitter that owns this particle.
        /// </summary>
        public int EmitterIndex;

        /// <summary>
        /// Gets the normalized age of the particle between 0 and 1.
        /// </summary>
        public float NormalizedAge => Lifetime > 0f ? Age / Lifetime : 0f;
    }
}
