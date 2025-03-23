using Strawberry.Graphics;
using Strawberry.Math;

namespace Strawberry.Serialization
{
    public class Vector2SerializeTemplate : SerializeTemplate
    {
        public override byte[] GetBytes(object obj)
        {
            Vector2 o = (Vector2)obj;
            byte[] xBytes = BitConverter.GetBytes(o.X);
            byte[] yBytes = BitConverter.GetBytes(o.Y);
            List<byte> bytes = new List<byte>();
            bytes.AddRange(xBytes);
            bytes.AddRange(yBytes);

            return bytes.ToArray();
        }

        public override object GetObjectBack(byte[] bytes)
        {
            float x;
            float y;
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    x = reader.ReadSingle();
                    y = reader.ReadSingle();
                }
            }

            return new Vector2(x, y);
        }
    }

    public class ColorSerializeTemplate : SerializeTemplate
    {
        public override byte[] GetBytes(object obj)
        {
            Color o = (Color)obj;
            byte[] rBytes = BitConverter.GetBytes(o.R);
            byte[] gBytes = BitConverter.GetBytes(o.G);
            byte[] bBytes = BitConverter.GetBytes(o.B);
            byte[] aBytes = BitConverter.GetBytes(o.A);
            List<byte> bytes = new List<byte>();
            bytes.AddRange(rBytes);
            bytes.AddRange(gBytes);
            bytes.AddRange(bBytes);
            bytes.AddRange(aBytes);

            return bytes.ToArray();
        }

        public override object GetObjectBack(byte[] bytes)
        {
            float r;
            float g;
            float b;
            float a;
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    r = reader.ReadSingle();
                    g = reader.ReadSingle();
                    b = reader.ReadSingle();
                    a = reader.ReadSingle();
                }
            }

            return new Color(r, g, b, a);
        }
    }

    public class BooleanSerializeTemplate : SerializeTemplate
    {
        public override byte[] GetBytes(object obj)
        {
            throw new NotImplementedException();
        }

        public override object GetObjectBack(byte[] bytes)
        {
            return BitConverter.ToBoolean(bytes, 0);
        }
    }

    public class Int16SerializeTemplate : SerializeTemplate
    {
        public override byte[] GetBytes(object obj)
        {
            throw new NotImplementedException();
        }

        public override object GetObjectBack(byte[] bytes)
        {
            return BitConverter.ToInt16(bytes, 0);
        }
    }

    public class Int32SerializeTemplate : SerializeTemplate
    {
        public override byte[] GetBytes(object obj)
        {
            throw new NotImplementedException();
        }

        public override object GetObjectBack(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }
    }

    public class Int64SerializeTemplate : SerializeTemplate
    {
        public override byte[] GetBytes(object obj)
        {
            throw new NotImplementedException();
        }

        public override object GetObjectBack(byte[] bytes)
        {
            return BitConverter.ToInt64(bytes, 0);
        }
    }

    public class FloatSerializeTemplate : SerializeTemplate
    {
        public override byte[] GetBytes(object obj)
        {
            throw new NotImplementedException();
        }

        public override object GetObjectBack(byte[] bytes)
        {
            return BitConverter.ToSingle(bytes, 0);
        }
    }

    public class DoubleSerializeTemplate : SerializeTemplate
    {
        public override byte[] GetBytes(object obj)
        {
            throw new NotImplementedException();
        }

        public override object GetObjectBack(byte[] bytes)
        {
            return BitConverter.ToDouble(bytes, 0);
        }
    }

    public class UInt16SerializeTemplate : SerializeTemplate
    {
        public override byte[] GetBytes(object obj)
        {
            throw new NotImplementedException();
        }

        public override object GetObjectBack(byte[] bytes)
        {
            return BitConverter.ToUInt16(bytes, 0);
        }
    }

    public class UInt32SerializeTemplate : SerializeTemplate
    {
        public override byte[] GetBytes(object obj)
        {
            throw new NotImplementedException();
        }

        public override object GetObjectBack(byte[] bytes)
        {
            return BitConverter.ToUInt32(bytes, 0);
        }
    }

    public class UInt64SerializeTemplate : SerializeTemplate
    {
        public override byte[] GetBytes(object obj)
        {
            throw new NotImplementedException();
        }

        public override object GetObjectBack(byte[] bytes)
        {
            return BitConverter.ToUInt64(bytes, 0);
        }
    }
}
