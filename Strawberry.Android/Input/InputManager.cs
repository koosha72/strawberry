using Strawberry.Input;

namespace Strawberry.Android.Input;

public class InputManager : IInputManager
{
    public IPoitingDevice PointingDevice { get; private set; }

    public IKeyboard Keyboard { get; private set; }

    public void Initialize()
    {
        PointingDevice = new PointingDevice();
        Keyboard = new Keyboard();
    }

    public void Update()
    {
        (PointingDevice as PointingDevice).Update();
        /*(Keyboard as Keyboard).Update();*/
    }
}
