/*
 * Strawberry Game Engine
 * File: BlendMode.cs
 * Author: Koosha Aabedini Nassab
 *
 * Blend factor and mode enumerations used by the graphics system.
 */

namespace Strawberry.Graphics
{
    /// <summary>
    /// Specifies the blend factor used in blending operations,
    /// determining how source and destination colors are weighted.
    /// </summary>
    public enum BlendFactor
    {
        /// <summary>
        /// Uses the source alpha value as the blend factor.
        /// </summary>
        SrcAlpha,

        /// <summary>
        /// Uses one minus the source alpha value as the blend factor.
        /// </summary>
        InvSrcAlpha,

        /// <summary>
        /// Uses the source color value as the blend factor.
        /// </summary>
        SrcColor,

        /// <summary>
        /// Uses one minus the source color value as the blend factor.
        /// </summary>
        InvSrcColor,

        /// <summary>
        /// Uses zero (0) as the blend factor, effectively removing the contribution.
        /// </summary>
        Zero,

        /// <summary>
        /// Uses one (1) as the blend factor, leaving the contribution unchanged.
        /// </summary>
        One
    }

    /// <summary>
    /// Specifies the arithmetic operation used to combine the source
    /// and destination blend factors.
    /// </summary>
    public enum BlendEquation
    {
        /// <summary>
        /// Adds the source and destination blend factors together.
        /// </summary>
        Add,

        /// <summary>
        /// Subtracts the destination blend factor from the source blend factor.
        /// </summary>
        Subtract
    }

    /// <summary>
    /// Defines the blending configuration for render operations, specifying
    /// how source and destination pixels are combined for both RGB and alpha channels.
    /// </summary>
    [Serializable]
    public struct BlendMode
    {
        /// <summary>
        /// The blend factor applied to the source RGB color value.
        /// </summary>
        public BlendFactor RGBSourceFactor;

        /// <summary>
        /// The blend factor applied to the destination RGB color value.
        /// </summary>
        public BlendFactor RGBDestFactor;

        /// <summary>
        /// The blend factor applied to the source alpha value.
        /// </summary>
        public BlendFactor AlphaSourceFactor;

        /// <summary>
        /// The blend factor applied to the destination alpha value.
        /// </summary>
        public BlendFactor AlphaDestFactor;

        /// <summary>
        /// The arithmetic equation used to combine the RGB source and destination factors.
        /// </summary>
        public BlendEquation RGBEquation;

        /// <summary>
        /// The arithmetic equation used to combine the alpha source and destination factors.
        /// </summary>
        public BlendEquation AlphaEquation;

        /// <summary>
        /// A constant color value used by certain blend factors (e.g., when a constant
        /// blend color is referenced in the blending operation).
        /// </summary>
        public Color Color;
    }
}