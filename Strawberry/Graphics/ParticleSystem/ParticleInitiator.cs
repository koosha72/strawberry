/*
 * Strawberry Game Engine
 * File: ParticleInitiator.cs
 * Author: Koosha Aabedini Nassab
 *
 * Configuration that initializes particle properties when spawned.
 */

using Strawberry.Math;

namespace Strawberry.Graphics.ParticleSystem
{
    /// <summary>
    /// Describes the emission shape used when spawning particles.
    /// </summary>
    public enum EmitShape
    {
        /// <summary>
        /// Emit particles from a single point.
        /// </summary>
        Point,

        /// <summary>
        /// Emit particles from a circle.
        /// </summary>
        Circle,

        /// <summary>
        /// Emit particles from a rectangle.
        /// </summary>
        Rectangle
    }

    /// <summary>
    /// Configures how particles are initialized when they are spawned.
    /// </summary>
    public class ParticleInitiator
    {
        /// <summary>
        /// Gets or sets the shape used for particle emission.
        /// </summary>
        public EmitShape Shape = EmitShape.Point;

        /// <summary>
        /// Gets or sets the radius used for circle emission.
        /// </summary>
        public float ShapeRadius = 0f;

        /// <summary>
        /// Gets or sets the width used for rectangle emission.
        /// </summary>
        public float ShapeWidth = 0f;

        /// <summary>
        /// Gets or sets the height used for rectangle emission.
        /// </summary>
        public float ShapeHeight = 0f;

        // Velocity
        /// <summary>
        /// Gets or sets the minimum initial particle speed.
        /// </summary>
        public float SpeedMin = 50f;

        /// <summary>
        /// Gets or sets the maximum initial particle speed.
        /// </summary>
        public float SpeedMax = 150f;

        /// <summary>
        /// Gets or sets the minimum emission direction in degrees.
        /// </summary>
        public float DirectionMin = 0f;          // degrees

        /// <summary>
        /// Gets or sets the maximum emission direction in degrees.
        /// </summary>
        public float DirectionMax = 360f;         // degrees

        // Lifetime
        /// <summary>
        /// Gets or sets the minimum lifetime of spawned particles.
        /// </summary>
        public float LifetimeMin = 1f;

        /// <summary>
        /// Gets or sets the maximum lifetime of spawned particles.
        /// </summary>
        public float LifetimeMax = 3f;

        // Initial visuals
        /// <summary>
        /// Gets or sets the starting color of spawned particles.
        /// </summary>
        public Color ColorStart = Color.White;

        /// <summary>
        /// Gets or sets the minimum scale applied to spawned particles.
        /// </summary>
        public float ScaleMin = 1f;

        /// <summary>
        /// Gets or sets the maximum scale applied to spawned particles.
        /// </summary>
        public float ScaleMax = 1f;

        /// <summary>
        /// Gets or sets the minimum initial rotation in degrees.
        /// </summary>
        public float RotationMin = 0f;            // degrees

        /// <summary>
        /// Gets or sets the maximum initial rotation in degrees.
        /// </summary>
        public float RotationMax = 0f;             // degrees

        /// <summary>
        /// Gets or sets the minimum angular velocity in degrees per second.
        /// </summary>
        public float AngularVelocityMin = 0f;      // degrees per second

        /// <summary>
        /// Gets or sets the maximum angular velocity in degrees per second.
        /// </summary>
        public float AngularVelocityMax = 0f;      // degrees per second

        // Acceleration
        /// <summary>
        /// Gets or sets the constant acceleration applied to spawned particles.
        /// </summary>
        public Vector2 Acceleration = new Vector2(0f, 0f);

        // Sprite
        /// <summary>
        /// Gets or sets the minimum image index for spawned particles.
        /// </summary>
        public int ImageIndexMin = 0;

        /// <summary>
        /// Gets or sets the maximum image index for spawned particles.
        /// </summary>
        public int ImageIndexMax = 0;

        // Randomize image index on each particle
        /// <summary>
        /// Gets or sets a value indicating whether the image index is randomized per particle.
        /// </summary>
        public bool RandomImageIndex = false;

        /// <summary>
        /// Initializes the particle properties for a newly spawned particle.
        /// </summary>
        /// <param name="particle">The particle to initialize.</param>
        /// <param name="emitterPosition">The emitter position.</param>
        /// <param name="sprite">The sprite used by the particle.</param>
        public void Initialize(ref Particle particle, Vector2 emitterPosition, Sprite sprite)
        {
            // Position from shape
            Vector2 shapeOffset;
            switch (Shape)
            {
                case EmitShape.Circle:
                    shapeOffset = RandomHelper.InsideCircle(ShapeRadius);
                    break;
                case EmitShape.Rectangle:
                    shapeOffset = RandomHelper.InsideRectangle(ShapeWidth, ShapeHeight);
                    break;
                default:
                    shapeOffset = Vector2.Zero;
                    break;
            }

            particle.Position = emitterPosition + shapeOffset;

            // Velocity from direction range
            float speed = RandomHelper.Range(SpeedMin, SpeedMax);
            float direction = (float)MathHelper.DegToRad(RandomHelper.Range(DirectionMin, DirectionMax));
            particle.Velocity = new Vector2(
                (float)System.Math.Cos(direction) * speed,
                (float)System.Math.Sin(direction) * speed);

            // Acceleration
            particle.Acceleration = Acceleration;

            // Lifetime
            particle.Lifetime = RandomHelper.Range(LifetimeMin, LifetimeMax);
            particle.Age = 0f;

            // Visuals
            particle.Color = ColorStart;
            particle.Scale = RandomHelper.Range(ScaleMin, ScaleMax);
            particle.Rotation = (float)MathHelper.DegToRad(RandomHelper.Range(RotationMin, RotationMax));
            particle.AngularVelocity = (float)MathHelper.DegToRad(RandomHelper.Range(AngularVelocityMin, AngularVelocityMax));

            // Alive
            particle.Alive = true;

            // Image index
            if (RandomImageIndex && sprite != null)
            {
                int maxIdx = System.Math.Max(0, sprite.ImageCount - 1);
                int min = System.Math.Clamp(ImageIndexMin, 0, maxIdx);
                int max = System.Math.Clamp(ImageIndexMax, 0, maxIdx);
                particle.ImageIndex = RandomHelper.Range(min, max + 1);
            }
            else
            {
                particle.ImageIndex = ImageIndexMin;
            }

            // Emitter index will be set by the emitter itself
            particle.EmitterIndex = -1;
        }
    }
}
