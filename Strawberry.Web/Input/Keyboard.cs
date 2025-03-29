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

    public void FirePressed(Keys key)
    {

    }

    public void FireReleased(Keys key)
    {

    }

    public bool IsKeyDown(Keys key)
    {
        return false;
    }

    public bool IsKeyPressed(Keys key)
    {
        return false;
    }

    public bool IsKeyReleased(Keys key)
    {
        return false;
    }
}
