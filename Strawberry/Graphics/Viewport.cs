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

        /// <summary>
        /// Centers the viewport on the screen
        /// </summary>
        /// <param name="displaySize"></param>
        public void Center(Vector2 displaySize)
        {
            ScreenPos = new Vector2((displaySize.X - ScreenSize.X) / 2, (displaySize.Y - ScreenSize.Y) / 2);
        }

        /// <summary>
        /// Converts a screen-space pixel position to scene coordinates.
        /// </summary>
        public Vector2 ScreenToScene(Vector2 screenPos)
        {
            float relX = (screenPos.X - ScreenPos.X) / ScreenSize.X;
            float relY = (screenPos.Y - ScreenPos.Y) / ScreenSize.Y;
            return new Vector2(
                ScenePos.X + relX * SceneSize.X,
                ScenePos.Y + relY * SceneSize.Y
            );
        }

        /// <summary>
        /// Converts a scene position to screen-space pixel coordinates.
        /// </summary>
        public Vector2 SceneToScreen(Vector2 scenePos)
        {
            float relX = (scenePos.X - ScenePos.X) / SceneSize.X;
            float relY = (scenePos.Y - ScenePos.Y) / SceneSize.Y;

            return new Vector2(
                ScreenPos.X + relX * ScreenSize.X,
                ScreenPos.Y + relY * ScreenSize.Y
            );
        }

        /// <summary>
        /// Resolves percent-based positioning into actual pixel coordinates.
        /// Call this when the display size changes or before rendering.
        /// </summary>
        public void ApplyPercent(Vector2 displaySize)
        {
            if (!UsePercent) return;

            ScreenPos = new Vector2(
                displaySize.X * PercentPos.X / 100f,
                displaySize.Y * PercentPos.Y / 100f
            );
            ScreenSize = new Vector2(
                displaySize.X * PercentSize.X / 100f,
                displaySize.Y * PercentSize.Y / 100f
            );
        }

        /// <summary>
        /// Fits the viewport to a given width. Keeps the aspect ratio. If your game runs in landscape mode, call this.
        /// </summary>
        /// <param name="displaySize"></param>
        public void FitWidth(Vector2 displaySize)
        {
            ScreenSize = new Vector2(displaySize.X, SceneSize.Y / SceneSize.X * displaySize.X);
        }

        /// <summary>
        /// Fits the viewport to a given height. Keeps the aspect ratio. If your game runs in portrait mode, call this.
        /// </summary>
        /// <param name="displaySize"></param>
        public void FitHeight(Vector2 displaySize)
        {
            ScreenSize = new Vector2(SceneSize.X / SceneSize.Y * displaySize.Y, displaySize.Y);
        }
    }
}

