namespace Strawberry.Graphics
{
    public enum BlendFactor
    {
        SrcAlpha,
        InvSrcAlpha,
        SrcColor,
        InvSrcColor,
        Zero,
        One
    }

    public enum BlendEquation
    {
        Add,
        Subtract
    }

    [Serializable]
    public struct BlendMode
    {
        public BlendFactor RGBSourceFactor;
        public BlendFactor RGBDestFactor;
        public BlendFactor AlphaSourceFactor;
        public BlendFactor AlphaDestFactor;

        public BlendEquation RGBEquation;
        public BlendEquation AlphaEquation;

        public Color Color;
    }
}
