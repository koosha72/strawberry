using Strawberry.Graphics;

namespace Strawberry.Android.Graphics;

public class BlendState
{
    public BlendFactor RGBSource;
    public BlendFactor RGBDest;

    public BlendFactor AlphaSource;
    public BlendFactor AlphaDest;

    public BlendFactor RGBEquation;
    public BlendFactor AlphaEquation;

    public Color Color;
}
