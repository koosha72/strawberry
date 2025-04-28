using Strawberry.Input;

namespace Strawberry.OpenGL.Input
{
    public class InputManager : IInputManager
    {
        public IPointingDevice PointingDevice { get; private set; }

        public IKeyboard Keyboard { get; private set; }

        public void Initialize()
        {
            PointingDevice = new PointingDevice();
            Keyboard = new Keyboard();
        }

        public void Update()
        {
            (PointingDevice as PointingDevice).Update();
            (Keyboard as Keyboard).Update();
        }
    }
}
