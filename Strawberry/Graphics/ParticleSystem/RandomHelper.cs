using System;
using Strawberry.Math;

namespace Strawberry.Graphics.ParticleSystem
{
    using Math = System.Math;
    public static class RandomHelper
    {
        static Random random = new Random();

        public static float Range(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }

        public static int Range(int min, int max)
        {
            return random.Next(min, max);
        }

        public static Vector2 InsideCircle(float radius)
        {
            double angle = random.NextDouble() * Math.PI * 2;
            float r = (float)Math.Sqrt(random.NextDouble()) * radius;
            return new Vector2((float)Math.Cos(angle) * r, (float)Math.Sin(angle) * r);
        }

        public static Vector2 InsideRectangle(float width, float height)
        {
            return new Vector2(Range(-width / 2, width / 2), Range(-height / 2, height / 2));
        }
    }
}
