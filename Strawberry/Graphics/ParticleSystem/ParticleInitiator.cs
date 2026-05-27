using Strawberry.Math;

namespace Strawberry.Graphics.ParticleSystem
{
    public enum EmitShape
    {
        Point,
        Circle,
        Rectangle
    }

    public class ParticleInitiator
    {
        // Emission shape
        public EmitShape Shape = EmitShape.Point;
        public float ShapeRadius = 0f;
        public float ShapeWidth = 0f;
        public float ShapeHeight = 0f;

        // Velocity
        public float SpeedMin = 50f;
        public float SpeedMax = 150f;
        public float DirectionMin = 0f;          // degrees
        public float DirectionMax = 360f;         // degrees

        // Lifetime
        public float LifetimeMin = 1f;
        public float LifetimeMax = 3f;

        // Initial visuals
        public Color ColorStart = Color.White;
        public float ScaleMin = 1f;
        public float ScaleMax = 1f;
        public float RotationMin = 0f;            // degrees
        public float RotationMax = 0f;             // degrees
        public float AngularVelocityMin = 0f;      // degrees per second
        public float AngularVelocityMax = 0f;      // degrees per second

        // Acceleration
        public Vector2 Acceleration = new Vector2(0f, 0f);

        // Sprite
        public int ImageIndexMin = 0;
        public int ImageIndexMax = 0;

        // Randomize image index on each particle
        public bool RandomImageIndex = false;

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
