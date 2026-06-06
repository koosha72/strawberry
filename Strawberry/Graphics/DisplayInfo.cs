/*
 * Strawberry Game Engine
 * File: DisplayInfo.cs
 * Author: Koosha Aabedini Nassab
 *
 * Provides display information (width and height) for graphics contexts.
 */

namespace Strawberry.Graphics
{
    /// <summary>
    /// Gets information about the current display
    /// </summary>
    public abstract class DisplayInfo
    {
        /// <summary>
        /// Gets the width of the display.
        /// </summary>
        public abstract int Width { get; }
        /// <summary>
        /// Gets the height of the display.
        /// </summary>
        public abstract int Height { get; }
    }
}
