using Strawberry.Input;

namespace Strawberry.Web.Input;

public class InputManager : IInputManager
{
    public IPointingDevice PointingDevice { get; private set; }

    public IKeyboard Keyboard { get; private set; }

    public void Initialize()
    {
        Keyboard = new Keyboard();
        PointingDevice = new PointingDevice();
    }

    public void Update()
    {
        (PointingDevice as PointingDevice).Update();
        (Keyboard as Keyboard).Update();
    }
}
