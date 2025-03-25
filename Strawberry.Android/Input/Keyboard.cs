using System;
using Strawberry.Input;

namespace Strawberry.Android.Input;

public class Keyboard : IKeyboard
{
    public IEnumerable<Keys> DownKeys => throw new NotImplementedException();

    public IEnumerable<Keys> PressedKeys => throw new NotImplementedException();

    public IEnumerable<Keys> RelasedKeys => throw new NotImplementedException();

    public void FirePressed(Keys key)
    {
        throw new NotImplementedException();
    }

    public void FireReleased(Keys key)
    {
        throw new NotImplementedException();
    }

    public bool IsKeyDown(Keys key)
    {
        throw new NotImplementedException();
    }

    public bool IsKeyPressed(Keys key)
    {
        throw new NotImplementedException();
    }

    public bool IsKeyReleased(Keys key)
    {
        throw new NotImplementedException();
    }
}
