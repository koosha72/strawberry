using OpenTK.Graphics.OpenGL;
using Strawberry.Graphics;

namespace Strawberry.OpenGL.Graphics
{
    public class BlendState
    {
        public BlendingFactorSrc RGBSource;
        public BlendingFactorDest RGBDest;

        public BlendingFactorSrc AlphaSource;
        public BlendingFactorDest AlphaDest;

        public BlendEquationMode RGBEquation;
        public BlendEquationMode AlphaEquation;

        public Color Color;
    }
}
