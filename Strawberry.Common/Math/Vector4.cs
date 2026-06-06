/*
 * Strawberry Game Engine
 * File: Vector4.cs
 * Author: Koosha Aabedini Nassab
 *
 * 4D vector struct used by the engine math utilities.
 */

namespace Strawberry.Math
{
    public struct Vector4
    {
        float x;
        float y;
        float z;
        float w;

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        public float W
        {
            get { return w; }
            set { w = value; }
        }

        public float Length
        {
            get { return (float)System.Math.Sqrt(x * x + y * y + z * z + w * w); }
        }


        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vector4(Vector4 vec)
        {
            this.x = vec.x;
            this.y = vec.y;
            this.z = vec.z;
            this.w = vec.w;
        }

        public Vector4(Vector2 v1, Vector2 v2)
        {
            this.x = v1.X;
            this.y = v1.Y;
            this.z = v2.X;
            this.w = v2.Y;
        }


        public static Vector4 operator +(Vector4 vec1, Vector4 vec2)
        {
            return new Vector4(vec1.x + vec2.x, vec1.y + vec2.y, vec1.z + vec2.z, vec1.w + vec2.w);
        }

        public static Vector4 operator -(Vector4 vec1, Vector4 vec2)
        {
            return new Vector4(vec1.x - vec2.x, vec1.y - vec2.y, vec1.z - vec2.z, vec1.w - vec2.w);
        }

        public static Vector4 operator *(Vector4 vec1, float s)
        {
            return new Vector4(vec1.x * s, vec1.y * s, vec1.z * s, vec1.w * s);
        }

        public static float operator *(Vector4 vec1, Vector4 vec2)
        {
            return (vec1.x * vec2.x) + (vec1.y * vec2.y) + (vec1.z * vec2.z) + (vec1.w * vec2.w);
        }

        public static Vector4 Normalize(Vector4 vec)
        {
            return new Vector4(vec.x / vec.Length, vec.y / vec.Length, vec.z / vec.Length, vec.w / vec.Length);
        }
    }
}
