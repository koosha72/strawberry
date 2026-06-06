/*
 * Strawberry Game Engine
 * File: Viewport.cs
 * Author: Koosha Aabedini Nassab
 *
 * Represents a rendering viewport including scene/world coordinates and screen mapping.
 */

using Strawberry.Math;


namespace Strawberry.Graphics
{
    /// <summary>
    /// Represents a viewport for a game engine, defining screen and scene coordinates.
    /// </summary>
    /// <remarks>
    /// This class is serializable to support saving and loading viewport configurations.
    /// Percent-based positioning is used when UsePercent is true.
    /// </remarks>
    [Serializable]
    public class Viewport
    {
        /// <summary>
        /// Gets or sets position on the screen in pixel coordinates.
        /// </summary>
        public Vector2 ScreenPos { get; set; }

        /// <summary>
        /// Gets or sets the size of the viewport area on the screen in pixel units.
        /// </summary>
        public Vector2 ScreenSize { get; set; }

        /// <summary>
        /// Gets or sets the position in scene/world coordinates (e.g., for camera targets).
        /// </summary>
        public Vector2 ScenePos { get; set; }

        /// <summary>
        /// Gets or sets the size of the viewport area in scene/world units.
        /// </summary>
        public Vector2 SceneSize { get; set; }

        /// <summary>
        /// Gets or sets whether to use percent-based positioning should be used instead of absolute pixels.
        /// </summary>
        public bool UsePercent { get; set; } = false;

        /// <summary>
        /// Gets or sets the position offset when using percent-based coordinates (0-100% of the screen).
        /// </summary>
        public Vector2 PercentPos { get; set; } = new Vector2();

        /// <summary>
        /// Gets or sets the size percentage when using percent-based coordinates (0-100% of the screen).
        /// </summary>
        public Vector2 PercentSize { get; set; } = new Vector2(100);

        /// <summary>
        /// Gets or sets the unique identifier name for this viewport.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Creates a new viewport with specified screen and scene coordinates.
        /// </summary>
        /// <param name="name">Unique identifier for the viewport</param>
        /// <param name="screenPos">Position on the screen in pixel coordinates</param>
        /// <param name="screenSize">Size of the viewport area on the screen</param>
        /// <param name="scenePos">Position in scene/world coordinates</param>
        /// <param name="sceneSize">Size of the viewport area in world units</param>
        public Viewport(string name, Vector2 screenPos, Vector2 screenSize,
                       Vector2 scenePos, Vector2 sceneSize)
        {
            ScreenPos = screenPos;
            ScreenSize = screenSize;
            ScenePos = scenePos;
            SceneSize = sceneSize;
            Name = name;
        }
    }
}

