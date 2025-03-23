using Strawberry.Math;

namespace Strawberry.Misc
{
    public struct RotatedRectangle
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        Vector2 Position
        {
            get { return new Vector2(X, Y); }
        }

        public Math.Vector2 Vertex1
        {
            get
            {
                Vector2 v1 = new Vector2(-(int)Origin.X, -(int)Origin.Y);
                v1.Direction += Angle;
                return v1 + Position;
            }
        }

        public Math.Vector2 Vertex2
        {
            get
            {
                Vector2 v1 = new Vector2(-(int)Origin.X + (int)Width, -(int)Origin.Y);
                v1.Direction += Angle;
                return v1 + Position;
            }
        }

        public Math.Vector2 Vertex3
        {
            get
            {
                Vector2 v1 = new Vector2(-(int)Origin.X + (int)Width, -(int)Origin.Y + (int)Height);
                v1.Direction += Angle;
                return v1 + Position;
            }
        }

        public Math.Vector2 Vertex4
        {
            get
            {
                Vector2 v1 = new Vector2(-(int)Origin.X, -(int)Origin.Y + (int)Height);
                v1.Direction += Angle;
                return v1 + Position;
            }
        }

        public float Left
        {
            get { return this.X - Origin.X; }
            set { this.X = value + Origin.X; }
        }

        public float Top
        {
            get { return this.Y - Origin.Y; }
            set { this.Y = value + Origin.Y; }
        }

        public float Right
        {
            get { return this.X + this.Width - Origin.X; }
        }

        public float Bottom
        {
            get { return this.Y + this.Height - Origin.Y; }
        }

        public float Angle { get; set; }

        public Vector2 Origin { get; set; }

        public RotatedRectangle(float x, float y, float width, float height, float angle, Vector2 origin)
        {
            if (width > 0)
                this.X = x;
            else
                this.X = x - width;
            if(height > 0)
                this.Y = y;
            else
                this.Y = y - height;
            if (width > 0)
                this.Width = width;
            else
                this.Width = -width;
            if(height > 0)
                this.Height = height;
            else
                this.Height = -height;
            this.Angle = angle;
            this.Origin = origin;
        }

        public RotatedRectangle(float x, float y, float width, float height, float angle)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.Angle = angle;
            this.Origin = new Math.Vector2();
        }

        public RotatedRectangle(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.Angle = 0;
            this.Origin = new Math.Vector2();
        }

        public RotatedRectangle(Math.Vector2 pos, Math.Vector2 size)
            : this(pos.X, pos.Y, size.X, size.Y)
        {
        }

        public RotatedRectangle(Math.Vector2 pos, Math.Vector2 size, float angle)
            : this(pos.X, pos.Y, size.X, size.Y, angle)
        {
        }

        public RotatedRectangle(Math.Vector2 pos, Math.Vector2 size, float angle, Math.Vector2 origin)
            : this(pos.X, pos.Y, size.X, size.Y, angle, origin)
        {
        }

        public RotatedRectangle(Math.Vector4 xywh)
            : this(xywh.X, xywh.Y, xywh.Z, xywh.W)
        {
        }

        public RotatedRectangle(Math.Vector4 xywh, float angle)
            : this(xywh.X, xywh.Y, xywh.Z, xywh.W, angle)
        {
        }
        public RotatedRectangle(Math.Vector4 xywh, float angle, Math.Vector2 origin)
            : this(xywh.X, xywh.Y, xywh.Z, xywh.W, angle, origin)
        {
        }

        public RotatedRectangle() : this(0, 0, 0, 0)
        {

        }

        public bool IsPointInside(Math.Vector2 point)
        {
            Vector2 p = new Vector2(point);
            p -= (Position);
            p.Direction -= Angle;
            p += Position;
            return p.X <= Right && p.X >= Left && p.Y >= Top && p.Y <= Bottom;
        }

        public bool IsPointInside(float x, float y)
        {
            return IsPointInside(new Vector2(x, y));
        }
        /*
        public bool Overlap(Rectangle rect)
        {
            return this.Left <= rect.Right && this.Right >= rect.Left &&
                        this.Top <= rect.Bottom && this.Bottom >= rect.Top;
        }

        public bool IsRectangleInside(Rectangle rect)
        {
            return this.Left <= rect.Left && this.Right >= rect.Right &&
                        this.Top <= rect.Top && this.Bottom >= rect.Bottom;
        }
        */
        public override bool Equals(object obj)
        {
            if (!(obj is RotatedRectangle))
                return false;
            else
            {
                RotatedRectangle other = (RotatedRectangle)obj;
                return other.X == this.X && other.Y == this.Y && other.Width == this.Width && other.Height == this.Height && other.Angle == this.Angle;
            }
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("X: {0}, Y: {1}, W: {2}, H: {3}, Angle: {4}", X, Y, Width, Height, Angle);
        }
    }
}
