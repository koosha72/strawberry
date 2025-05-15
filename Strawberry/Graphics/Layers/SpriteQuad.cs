using Strawberry.Math;

namespace Strawberry.Graphics.Layers
{
    public class SpriteQuad : IEquatable<SpriteQuad>
    {
        public Texture Texture { get; set; }

        public Vector4 XYUV1 { get; set; }

        public Vector4 XYUV2 { get; set; }

        public Vector4 XYUV3 { get; set; }

        public Vector4 XYUV4 { get; set; }

        public Color Color { get; set; }

        public BasicShader Shader { get; set; }

        public string BlendName { get; set; }


        public SpriteQuad(Texture texture, Vector4 xyuv1, Vector4 xyuv2, Vector4 xyuv3, Vector4 xyuv4,
            Color color, BasicShader shader, string blendName)
        {
            this.Texture = texture;
            this.XYUV1 = xyuv1;
            this.XYUV2 = xyuv2;
            this.XYUV3 = xyuv3;
            this.XYUV4 = xyuv4;
            this.Color = color;
            this.Shader = shader;
            this.BlendName = blendName;
        }

        public bool Equals(SpriteQuad other)
        {
            bool result = this.Texture == other.Texture;
            result &= this.Shader == other.Shader;
            result &= this.BlendName == other.BlendName;
            return result;
        }

        public override bool Equals(object obj)
        {
            SpriteQuad other = (SpriteQuad)obj;
            bool result = this.Texture == other.Texture;
            result &= this.Shader == other.Shader;
            result &= this.BlendName == other.BlendName;
            return result;
        }

        public static bool operator ==(SpriteQuad q1, SpriteQuad q2)
        {
            if ((object)q1 != null && (object)q2 != null)
                return q1.Equals(q2);
            else return (object)q1 == (object)q2;
        }

        public static bool operator !=(SpriteQuad q1, SpriteQuad q2)
        {
            if ((object)q1 != null && (object)q2 != null)
                return !q1.Equals(q2);
            else return (object)q1 == (object)q2;
        }

        public override int GetHashCode()
        {
            return this.Texture.GetHashCode() ^ this.BlendName.GetHashCode();
        }
    }
}
