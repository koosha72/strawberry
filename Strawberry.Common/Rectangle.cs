namespace Strawberry.Common
{
    /// <summary>
    /// Represents a floating-point rectangle defined by its X, Y, Width, and Height coordinates.
    /// </summary>
    [Serializable]
    public struct Rectangle
    {
        /// <summary>
        /// Gets or sets the X-coordinate of the rectangle.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the Y-coordinate of the rectangle.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the width of the rectangle.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the rectangle.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Gets or sets the X-coordinate (left edge) of the rectangle.
        /// </summary>
        public float Left
        {
            get { return this.X; }
            set { this.X = value; }
        }

        /// <summary>
        /// Gets or sets the Y-coordinate (top edge) of the rectangle.
        /// </summary>
        public float Top
        {
            get { return this.Y; }
            set { this.Y = value; }
        }

        /// <summary>
        /// Gets the X-coordinate of the right edge of the rectangle.
        /// </summary>
        public float Right
        {
            get { return this.X + this.Width; }
        }

        /// <summary>
        /// Gets the Y-coordinate of the bottom edge of the rectangle.
        /// </summary>
        public float Bottom
        {
            get { return this.Y + this.Height; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> struct with the specified location and size.
        /// </summary>
        /// <param name="x">The X-coordinate of the rectangle.</param>
        /// <param name="y">The Y-coordinate of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public Rectangle(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> struct using position and size vectors.
        /// </summary>
        /// <param name="pos">The position of the top-left corner of the rectangle.</param>
        /// <param name="size">The size (width and height) of the rectangle.</param>
        public Rectangle(Math.Vector2 pos, Math.Vector2 size)
            : this(pos.X, pos.Y, size.X, size.Y)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> struct using a 4D vector.
        /// </summary>
        /// <param name="xywh">A vector where X and Y represent the position, and Z and W represent the width and height respectively.</param>
        public Rectangle(Math.Vector4 xywh)
            : this(xywh.X, xywh.Y, xywh.Z, xywh.W)
        {
        }

        /// <summary>
        /// Determines whether the specified point lies inside the rectangle's bounds.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns><c>true</c> if the point is inside the rectangle; otherwise, <c>false</c>.</returns>
        public bool IsPointInside(Math.Vector2 point)
        {
            return point.X <= Right && point.X >= Left && point.Y >= Top && point.Y <= Bottom;
        }

        /// <summary>
        /// Determines whether the specified point lies inside the rectangle's bounds.
        /// </summary>
        /// <param name="x">The X-coordinate of the point to check.</param>
        /// <param name="y">The Y-coordinate of the point to check.</param>
        /// <returns><c>true</c> if the point is inside the rectangle; otherwise, <c>false</c>.</returns>
        public bool IsPointInside(float x, float y)
        {
            return x <= Right && x >= Left && y >= Top && y <= Bottom;
        }

        /// <summary>
        /// Determines whether this rectangle overlaps with another rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to check for overlap.</param>
        /// <returns><c>true</c> if the rectangles overlap; otherwise, <c>false</c>.</returns>
        public bool Overlap(Rectangle rect)
        {
            return this.Left <= rect.Right && this.Right >= rect.Left &&
                        this.Top <= rect.Bottom && this.Bottom >= rect.Top;
        }

        /// <summary>
        /// Determines whether the specified rectangle is entirely contained within this rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to check for containment.</param>
        /// <returns><c>true</c> if the specified rectangle is entirely inside this rectangle; otherwise, <c>false</c>.</returns>
        public bool IsRectangleInside(Rectangle rect)
        {
            return this.Left <= rect.Left && this.Right >= rect.Right &&
                        this.Top <= rect.Top && this.Bottom >= rect.Bottom;
        }

        /// <summary>
        /// Determines whether this rectangle intersects with another rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to check for intersection.</param>
        /// <returns><c>true</c> if the rectangles intersect; otherwise, <c>false</c>.</returns>
        public bool IntersectsWith(Rectangle rect)
        {
            return (rect.X < this.X + this.Width) &&
            (this.X < (rect.X + rect.Width)) &&
            (rect.Y < this.Y + this.Height) &&
            (this.Y < rect.Y + rect.Height);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current rectangle.</param>
        /// <returns><c>true</c> if the specified object is a <see cref="Rectangle"/> and has the same X, Y, Width, and Height values; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Returns a hash code for this <see cref="Rectangle"/>.
        /// </summary>
        /// <returns>A hash code for the current rectangle.</returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Converts the numeric values of this <see cref="Rectangle"/> to its equivalent string representation.
        /// </summary>
        /// <returns>A string that contains the X, Y, Width, and Height values of this rectangle.</returns>
        public override string ToString()
        {
            return String.Format("X: {0}, Y: {1}, W: {2}, H: {3}", X, Y, Width, Height);
        }
    }
}