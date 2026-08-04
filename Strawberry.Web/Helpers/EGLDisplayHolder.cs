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
        Interop.CanvasResized += OnCanvasResized;

        canvasSize = new Vector2(Interop.GetCanvasWidth(), Interop.GetCanvasHeight());
    }

    public Vector2 GetCanvasSize()
    {
        return canvasSize;
    }

    public void OnCanvasResized(float w, float h, float dpr)
    {
        canvasSize = new Vector2(w, h);
        Game.Instance?.GameContext?.OnResized(new Vector2(w, h));
    }
}