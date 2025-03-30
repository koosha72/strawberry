using Strawberry.Input;

namespace Strawberry.Web.Input;

public class Keyboard : IKeyboard
{
    public IEnumerable<Keys> DownKeys => downKeys;

    public IEnumerable<Keys> PressedKeys => pressedKeys;

    public IEnumerable<Keys> RelasedKeys => releasedKeys;

    List<Keys> downKeys = new List<Keys>();
    List<Keys> pressedKeys = new List<Keys>();
    List<Keys> releasedKeys = new List<Keys>();

    public Keyboard()
    {
        Interop.KeyDown += KeyDown;
        Interop.KeyUp += KeyUp;
    }

    public void FirePressed(Keys key)
    {
        if (!IsKeyDown(key))
        {
            pressedKeys.Add(key);
            downKeys.Add(key);
        }
    }

    public void FireReleased(Keys key)
    {
        releasedKeys.Add(key);
        if (downKeys.Contains(key))
            downKeys.Remove(key);
    }

    public bool IsKeyDown(Keys key)
    {
        return downKeys.Contains(key);
    }

    public bool IsKeyPressed(Keys key)
    {
        return pressedKeys.Contains(key);
    }

    public bool IsKeyReleased(Keys key)
    {
        return releasedKeys.Contains(key);
    }

    void KeyDown(Keys key, bool shift, bool ctrl, bool alt, bool repeat)
    {
        if (!repeat)
        {
            pressedKeys.Add(key);
            downKeys.Add(key);
        }
    }

    void KeyUp(Keys key, bool shift, bool ctrl, bool alt)
    {
        releasedKeys.Add(key);
        if (downKeys.Contains(key))
            downKeys.Remove(key);
    }

    public void Update()
    {
        pressedKeys.Clear();
        releasedKeys.Clear();
    }
}
