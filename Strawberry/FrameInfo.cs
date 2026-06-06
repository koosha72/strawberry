/*
 * Strawberry Game Engine
 * File: FrameInfo.cs
 * Author: Koosha Aabedini Nassab
 *
 * Provides global access to frame timing and update information.
 */

namespace Strawberry
{
    /// <summary>
    /// Contains information about the current frame.
    /// </summary>
    public static class FrameInfo
    {
        /// <summary>
        /// Gets the current frame information.
        /// </summary>
        public static IFrameInfoProvider Information { get; private set; }
        /// <summary>
        /// Registers the frame information provider.
        /// </summary>
        /// <param name="infoContainer"></param>
        public static void Register(IFrameInfoProvider infoContainer)
        {
            Information = infoContainer;
        }
    }
}
