using Strawberry.Platform;

namespace Strawberry.Desktop;

public class DisplayConfig : IDisplayConfig
{
    private int width;
    public int Width
    {
        get { return width; }
        set
        {
            width = value;
            gameLauncher.SetSize(width, height);
        }
    }

    private int height;
    public int Height
    {
        get { return height; }
        set
        {
            height = value;
            gameLauncher.SetSize(width, height);
        }
    }

    private DisplayMode displayMode;
    public DisplayMode DisplayMode
    {
        get { return displayMode; }
        set
        {
            displayMode = value;
            gameLauncher.ChangeDisplayMode(displayMode);
        }
    }

    GameLauncher gameLauncher;

    public DisplayConfig(int width, int height, DisplayMode displayMode, GameLauncher gameLauncher)
    {
        this.width = width;
        this.height = height;
        this.displayMode = displayMode;
        this.gameLauncher = gameLauncher;
    }
}