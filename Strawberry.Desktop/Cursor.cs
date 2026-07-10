using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Strawberry.Platform;

namespace Strawberry.Desktop;

public class Cursor : ICursor
{
    private bool visible;
    public bool Visible
    {
        get => visible;
        set
        {
            visible = value;
            if (value == true)
                window.CursorState = CursorState.Normal;
            else
                window.CursorState = CursorState.Hidden;
        }
    }
    

    private GameWindow window;

    public Cursor(GameWindow wnd)
    {
        window = wnd;
    }
}