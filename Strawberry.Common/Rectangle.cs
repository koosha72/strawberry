namespace Strawberry.Common
{
    [Serializable]
    public struct Rectangle
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public float Left
        {
            get { return this.X; }
            set { this.X = value; }
        }

        public float Top
        {
            get { return this.Y; }
            set { this.Y = value; }
        }

        public float Right
        {
            get { return this.X + this.Width; }
        }

        public float Bottom
        {
            get { return this.Y + this.Height; }
        }

        public Rectangle(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public Rectangle(Math.Vector2 pos, Math.Vector2 size)
            : this(pos.X, pos.Y, size.X, size.Y)
        {
        }

        public Rectangle(Math.Vector4 xywh)
            : this(xywh.X, xywh.Y, xywh.Z, xywh.W)
        {
        }

        public bool IsPointInside(Math.Vector2 point)
        {
            return point.X <= Right && point.X >= Left && point.Y >= Top && point.Y <= Bottom;
        }

        public bool IsPointInside(float x, float y)
        {
            return x <= Right && x >= Left && y >= Top && y <= Bottom;
        }

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

        public bool IntersectsWith(Rectangle rect)
        {
            return (rect.X < this.X + this.Width) &&
            (this.X < (rect.X + rect.Width)) &&
            (rect.Y < this.Y + this.Height) &&
            (this.Y < rect.Y + rect.Height);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rectangle))
                return false;
            else
            {
                Rectangle other = (Rectangle)obj;
                return other.X == this.X && other.Y == this.Y && other.Width == this.Width && other.Height == this.Height;
            }
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("X: {0}, Y: {1}, W: {2}, H: {3}", X, Y, Width, Height);
        }
    }
}
