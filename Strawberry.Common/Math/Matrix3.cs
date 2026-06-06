/*
 * Strawberry Game Engine
 * File: Matrix3.cs
 * Author: Koosha Aabedini Nassab
 *
 * 3x3 matrix implementation used by engine math routines.
 */

namespace Strawberry.Math
{
    public struct Matrix3
    {
        internal Vector3 ex, ey, ez;

        /// <summary>
        ///  Construct this matrix using columns.
        /// </summary>
        internal Matrix3(Vector3 c1, Vector3 c2, Vector3 c3)
        {
            ex = c1;
            ey = c2;
            ez = c3;
        }

        /// <summary>
        ///  Set this matrix to all zeros.
        /// </summary>
        internal void SetZero()
        {
            ex = Vector3.Zero;
            ey = Vector3.Zero;
            ez = Vector3.Zero;
        }

        /// <summary>
        ///  Solve A * x = b, where b is a column vector. This is more efficient
        ///  than computing the inverse in one-shot cases.
        /// </summary>
        internal Vector3 Solve33(Vector3 b)
        {
            float det = Vector3.Dot(ex, Vector3.Cross(ey, ez));
            //Debug.Assert(det != 0.0f);
            if (det != 0.0f)
            {
                det = 1.0f / det;
            }
            var x = new Vector3();
            x.X = det * Vector3.Dot(b, Vector3.Cross(ey, ez));
            x.Y = det * Vector3.Dot(ex, Vector3.Cross(b, ez));
            x.Z = det * Vector3.Dot(ex, Vector3.Cross(ey, b));
            return x;
        }

        /// <summary>
        ///  Solve A * x = b, where b is a column vector. This is more efficient
        ///  than computing the inverse in one-shot cases. Solve only the upper
        ///  2-by-2 matrix equation.
        /// </summary>
        internal Vector2 Solve22(Vector2 b)
        {
            float a11 = ex.X, a12 = ey.X, a21 = ex.Y, a22 = ey.Y;
            float det = a11 * a22 - a12 * a21;
            //Debug.Assert(det != 0.0f);
            if (det != 0.0f)
            {
                det = 1.0f / det;
            }
            var x = new Vector2();
            x.X = det * (a22 * b.X - a12 * b.Y);
            x.Y = det * (a11 * b.Y - a21 * b.X);
            return x;
        }

        internal Matrix3 GetInverse22(Matrix3 M)
        {
            float a = ex.X, b = ey.X, c = ex.Y, d = ey.Y;
            float det = a * d - b * c;
            if (det != 0.0f)
            {
                det = 1.0f / det;
            }

            M.ex.X = det * d;
            M.ey.X = -det * b;
            M.ex.Z = 0.0f;
            M.ex.Y = -det * c;
            M.ey.Y = det * a;
            M.ey.Z = 0.0f;
            M.ez.X = 0.0f;
            M.ez.Y = 0.0f;
            M.ez.Z = 0.0f;

            return M;
        }

        internal Matrix3 GetSymInverse33(Matrix3 M)
        {
            float det = Vector3.Dot(ex, Vector3.Cross(ey, ez));
            if (det != 0.0f)
            {
                det = 1.0f / det;
            }

            float a11 = ex.X, a12 = ey.X, a13 = ez.X;
            float a22 = ey.Y, a23 = ez.Y;
            float a33 = ez.Z;

            M.ex.X = det * (a22 * a33 - a23 * a23);
            M.ex.Y = det * (a13 * a23 - a12 * a33);
            M.ex.Z = det * (a12 * a23 - a13 * a22);

            M.ey.X = M.ex.Y;
            M.ey.Y = det * (a11 * a33 - a13 * a13);
            M.ey.Z = det * (a13 * a12 - a11 * a23);

            M.ez.X = M.ex.Z;
            M.ez.Y = M.ey.Z;
            M.ez.Z = det * (a11 * a22 - a12 * a12);

            return M;
        }
    }
}
