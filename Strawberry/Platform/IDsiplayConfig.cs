namespace Strawberry.Platform;

public enum DisplayMode
{
    /// <summary>
    /// A bordered window centered in the primary monitor, with a title bar.
    /// It is sized after GameContext's width and height.
    /// </summary>
    Windowed,
    /// <summary>
    /// A borderless fullscreen window.
    /// </summary>
    Borderless,
    /// <summary>
    /// Implements native fullscreen rendering, (may have problems with screen recording and screenshots)
    /// </summary>
    Fullscreen
}

/// <summary>
/// Provides a way to configure the display mode and size of the game window.
/// </summary>
public interface IDisplayConfig : IPlatformService
{
    /// <summary>
    /// Gets or sets the display mode of the game window.
    /// </summary>
    public DisplayMode DisplayMode { get; set; }

    /// <summary>
    /// Gets or sets the width of the game window.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the game window.
    /// </summary>
    public int Height { get; set; }
}