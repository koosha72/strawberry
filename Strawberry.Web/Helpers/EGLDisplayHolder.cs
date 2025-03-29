using Strawberry.Math;

namespace Strawberry.Web.Helpers;

public class EGLDisplayHolder
{
    public nint Display { get; private set; }

    public nint Surface { get; private set; }

    Vector2 canvasSize;

    public EGLDisplayHolder(nint display, nint surface)
    {
        Display = display;
        Surface = surface;

        canvasSize = new Vector2(1280, 720);
    }

    public Vector2 GetCanvasSize()
    {
        return canvasSize;
    }
}