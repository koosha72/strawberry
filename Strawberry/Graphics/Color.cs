namespace Strawberry.Graphics
{
    /// <summary>
    /// Represents a Color
    /// </summary>
    [Serializable]
    public struct Color
    {
        /// <summary>
        /// Red value of the color from 0.0 to 1.0
        /// </summary>
        public float R { get; set; }

        /// <summary>
        /// Green value of the color from 0.0 to 1.0
        /// </summary>
        public float G { get; set; }

        /// <summary>
        /// Blue value of the color from 0.0 to 1.0
        /// </summary>
        public float B { get; set; }

        /// <summary>
        /// Alpha value of the color from 0.0 to 1.0 used for transparency. 1.0=fully opaque,0.0=fully transparent
        /// </summary>
        public float A { get; set; }
        /// <summary>
        /// create a Color using r,g,b,a values from 0.0 to 1.0
        /// </summary>
        /// <param name="r">Red value of the color</param>
        /// <param name="g">Green value of the color</param>
        /// <param name="b">Blue value of the color</param>
        /// <param name="a">Alpha value of the color used for transparency. 1.0=fully opaque,0.0=fully transparent</param>
        public Color(float r, float g, float b, float a)
            : this()
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }
        /// <summary>
        /// create a Color using r,g,b,a values from 0 to 255
        /// </summary>
        /// <param name="r">Red value of the color</param>
        /// <param name="g">Green value of the color</param>
        /// <param name="b">Blue value of the color</param>
        /// <param name="a">Alpha value of the color used for transparency. 255=fully opaque,0=fully transparent</param>
        public Color(byte r, byte g, byte b, byte a)
            : this()
        {
            this.R = ((float)r) / 255.0f;
            this.G = ((float)g) / 255.0f;
            this.B = ((float)b) / 255.0f;
            this.A = ((float)a) / 255.0f;
        }
        /// <summary>
        /// Creates a color using an unsigned integer. You can use hex values
        /// </summary>
        /// <param name="color">The color code</param>
        public Color(uint color)
            : this()
        {
            byte[] bytes = BitConverter.GetBytes(color);

            this.R = ((float)bytes[3]) / 255.0f;
            this.G = ((float)bytes[2]) / 255.0f;
            this.B = ((float)bytes[1]) / 255.0f;
            this.A = ((float)bytes[0]) / 255.0f;
        }
        /// <summary>
        /// Creates a color from an existing color with a new alpha (transparnecy) value
        /// </summary>
        /// <param name="color">The old color</param>
        /// <param name="a">Alpha value of the color used for transparency. 1.0=fully opaque,0.0=fully transparent</param>
        public Color(Color color, float alpha)
            : this()
        {
            this.R = color.R;
            this.G = color.G;
            this.B = color.B;
            this.A = alpha;
        }

        public static bool operator ==(Color c1, Color c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(Color c1, Color c2)
        {
            return !c1.Equals(c2);
        }

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                Color c = (Color)obj;
                return c.R == R && c.G == G && c.B == B && c.A == A;
            }
            else
                return false;
        }

        public override string ToString()
        {
            return String.Format("R:{0}, G:{1}, B:{2}, A:{3}", R, G, B, A);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public static Color White
        {
            get
            {
                return new Color(0xffffffff);
            }
        }

        public static Color Black
        {
            get
            {
                return new Color(0x000000ff);
            }
        }

        public static Color Silver
        {
            get
            {
                return new Color(0xc0c0c0ff);
            }
        }

        public static Color Gray
        {
            get
            {
                return new Color(0x808080ff);
            }
        }

        public static Color Red
        {
            get
            {
                return new Color(1.0f, 0.0f, 0.0f, 1.0f);
            }
        }

        public static Color Green
        {
            get
            {
                return new Color(0x008000ff);
            }
        }

        public static Color Blue
        {
            get
            {
                return new Color(0.0f, 0.0f, 1.0f, 1.0f);
            }
        }

        public static Color CornflowerBlue
        {
            get
            {
                return new Color(0x6495edff);
            }
        }

        public static Color Orange
        {
            get
            {
                return new Color(0xffa500ff);
            }
        }

        public static Color Cyan
        {
            get
            {
                return new Color(0x00ffffff);
            }
        }

        public static Color DarkBlue
        {
            get
            {
                return new Color(0x0000a0ff);
            }
        }

        public static Color LightBlue
        {
            get
            {
                return new Color(0xadd8e6ff);
            }
        }

        public static Color Purple
        {
            get
            {
                return new Color(0x800080ff);
            }
        }

        public static Color Yellow
        {
            get
            {
                return new Color(0xffff00ff);
            }
        }

        public static Color Lime
        {
            get
            {
                return new Color(0x00ff00ff);
            }
        }

        public static Color Magenata
        {
            get
            {
                return new Color(0xff00ffff);
            }
        }

        public static Color HotPink
        {
            get
            {
                return new Color(0xff5fb9ff);
            }
        }


        public static Color LightPink
        {
            get
            {
                return new Color(0xffb6c1ff);
            }
        }

        public static Color Pink
        {
            get
            {
                return new Color(0xffc0cbff);
            }
        }

        public static Color DeepPink
        {
            get
            {
                return new Color(0xff1493ff);
            }
        }

        public static Color Brown
        {
            get
            {
                return new Color(0xa52a2aff);
            }
        }

        public static Color Maroon
        {
            get
            {
                return new Color(0x800000ff);
            }
        }

        public static Color Transparent
        {
            get
            {
                return new Color();
            }
        }

        public string ToHex()
        {
            return ((byte)(R * 255)).ToString("X2") +
                ((byte)(G * 255)).ToString("X2") +
                ((byte)(B * 255)).ToString("X2") +
                ((byte)(A * 255)).ToString("X2");
        }
    }
}
