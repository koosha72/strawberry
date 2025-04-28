using Strawberry.Input;

namespace Strawberry.Android.Input;

public class Keyboard : IKeyboard
{
    public IEnumerable<Keys> DownKeys => downKeys;

    public IEnumerable<Keys> PressedKeys => pressedKeys;

    public IEnumerable<Keys> ReleasedKeys => releasedKeys;

    List<Keys> downKeys = new List<Keys>();
    List<Keys> pressedKeys = new List<Keys>();
    List<Keys> releasedKeys = new List<Keys>();

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

    public void Update()
    {
        pressedKeys.Clear();
        releasedKeys.Clear();
    }
}
