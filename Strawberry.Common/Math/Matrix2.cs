/*
 * Strawberry Game Engine
 * File: Matrix2.cs
 * Author: Koosha Aabedini Nassab
 *
 * 2x2 matrix implementation used by engine math routines.
 */

namespace Strawberry.Math
{
    public struct Matrix2
    {
        public Vector2 Col1, Col2;

        /// <summary>
        /// Construct this matrix using columns.
        /// </summary>
        public Matrix2(Vector2 c1, Vector2 c2)
        {
            Col1 = c1;
            Col2 = c2;
        }

        /// <summary>
        /// Construct this matrix using scalars.
        /// </summary>
        public Matrix2(float a11, float a12, float a21, float a22)
        {
            Col1.X = a11; Col1.Y = a21;
            Col2.X = a12; Col2.Y = a22;
        }

        /// <summary>
        /// Construct this matrix using an angle. 
        /// This matrix becomes an orthonormal rotation matrix.
        /// </summary>
        public Matrix2(float angle)
        {
            float c = (float)System.Math.Cos(angle), s = (float)System.Math.Sin(angle);
            Col1.X = c; Col2.X = -s;
            Col1.Y = s; Col2.Y = c;
        }

        /// <summary>
        /// Initialize this matrix using columns.
        /// </summary>
        public void Set(Vector2 c1, Vector2 c2)
        {
            Col1 = c1;
            Col2 = c2;
        }

        /// <summary>
        /// Initialize this matrix using an angle.
        /// This matrix becomes an orthonormal rotation matrix.
        /// </summary>
        public void Set(float angle)
        {
            float c = (float)System.Math.Cos(angle), s = (float)System.Math.Sin(angle);
            Col1.X = c; Col2.X = -s;
            Col1.Y = s; Col2.Y = c;
        }

        /// <summary>
        /// Set this to the identity matrix.
        /// </summary>
        public void SetIdentity()
        {
            Col1.X = 1.0f; Col2.X = 0.0f;
            Col1.Y = 0.0f; Col2.Y = 1.0f;
        }

        /// <summary>
        /// Set this matrix to all zeros.
        /// </summary>
        public void SetZero()
        {
            Col1.X = 0.0f; Col2.X = 0.0f;
            Col1.Y = 0.0f; Col2.Y = 0.0f;
        }

        /// <summary>
        /// Extract the angle from this matrix (assumed to be a rotation matrix).
        /// </summary>
        public float GetAngle()
        {
            return (float)System.Math.Atan2(Col1.Y, Col1.X);
        }

        /// <summary>
        /// Compute the inverse of this matrix, such that inv(A) * A = identity.
        /// </summary>
        public Matrix2 Invert()
        {
            float a = Col1.X, b = Col2.X, c = Col1.Y, d = Col2.Y;
            Matrix2 B = new Matrix2();
            float det = a * d - b * c;
            det = 1.0f / det;
            B.Col1.X = det * d; B.Col2.X = -det * b;
            B.Col1.Y = -det * c; B.Col2.Y = det * a;
            return B;
        }

        /// <summary>
        /// Solve A * x = b, where b is a column vector. This is more efficient
        /// than computing the inverse in one-shot cases.
        /// </summary>
        public Vector2 Solve(Vector2 b)
        {
            float a11 = Col1.X, a12 = Col2.X, a21 = Col1.Y, a22 = Col2.Y;
            float det = a11 * a22 - a12 * a21;
            det = 1.0f / det;
            Vector2 x = new Vector2();
            x.X = det * (a22 * b.X - a12 * b.Y);
            x.Y = det * (a11 * b.Y - a21 * b.X);
            return x;
        }

        public static Matrix2 Identity { get { return new Matrix2(1, 0, 0, 1); } }

        public static Matrix2 operator +(Matrix2 A, Matrix2 B)
        {
            Matrix2 C = new Matrix2();
            C.Set(A.Col1 + B.Col1, A.Col2 + B.Col2);
            return C;
        }
    }
}
