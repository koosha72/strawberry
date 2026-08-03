using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using Strawberry.Input;
using Strawberry.Web.Graphics;

namespace Strawberry.Web;
#pragma warning disable CA1416
/// <summary>
/// The interop class for web assembly. The main.js file of the game calls callbacks from this class.
/// </summary>
public static partial class Interop
{
    [JSImport("initialize", "main.js")]
    public static partial Task Initialize();

    [JSImport("request_root_url", "main.js")]
    public static partial string RequestRootURL();

    public static event Action<Keys, bool, bool, bool, bool> KeyDown;
    public static event Action<Keys, bool, bool, bool> KeyUp;
    public static event Action<PointerButtons, int, bool, bool, bool> PointerDown;
    public static event Action<PointerButtons, int, bool, bool, bool> PointerUp;
    public static event Action<int, float, float> PointerMove;

    public static event Action Paused;
    public static event Action Resumed;

    public static event Action GraphicsContextLost;
    public static event Action GraphicsContextRestored;

    public static string RootUrl = "";

    [JSExport]
    public static void OnKeyDown(bool shift, bool ctrl, bool alt, bool repeat, int code)
    {
        Keys key = (Keys)code;
        KeyDown?.Invoke(key, shift, ctrl, alt, repeat);
    }

    [JSExport]
    public static void OnKeyUp(bool shift, bool ctrl, bool alt, int code)
    {
        Keys key = (Keys)code;
        KeyUp?.Invoke(key, shift, ctrl, alt);
    }

    [JSExport]
    public static void OnMouseMove(int index, float x, float y)
    {
        PointerMove?.Invoke(index, x, y);
    }

    [JSExport]
    public static void OnMouseDown(int index, bool shift, bool ctrl, bool alt, int button)
    {
        PointerButtons b = PointerButtons.None;
        if (button == 0)
        {
            b = PointerButtons.Primary;
        }
        else if (button == 1)
        {
            b = PointerButtons.Alternative;
        }
        else if (button == 2)
        {
            b = PointerButtons.Secondary;
        }
        PointerDown?.Invoke(b, index, shift, ctrl, alt);
    }

    [JSExport]
    public static void OnMouseUp(int index, bool shift, bool ctrl, bool alt, int button)
    {
        PointerButtons b = PointerButtons.None;
        if (button == 0)
        {
            b = PointerButtons.Primary;
        }
        else if (button == 1)
        {
            b = PointerButtons.Alternative;
        }
        else if (button == 2)
        {
            b = PointerButtons.Secondary;
        }
        PointerUp?.Invoke(b, index, shift, ctrl, alt);
    }

    [JSExport]
    public static void OnCanvasResize(float width, float height, float devicePixelRatio)
    {
    }

    [JSExport]
    public static void OnPause() => Paused?.Invoke();


    [JSExport]
    public static void OnResume() => Resumed?.Invoke();

    [JSExport]
    public static void SetUserDataCache(string json) => UserDataStorage.InitializeFromJson(json);

    [JSExport]
    public static void OnFocusLost() => Game.Instance?.GameContext?.OnFocusLost();

    [JSExport]
    public static void OnFocusGained() => Game.Instance?.GameContext?.OnFocusGained();

    [JSExport]
    public static void OnGraphicsContextLost() => GraphicsContextLost?.Invoke();

    [JSExport]
    public static void OnGraphicsContextRestored() => GraphicsContextRestored?.Invoke();
}
