using Strawberry.Input;
using Strawberry.Math;

namespace Strawberry.OpenGL.Input
{
    public class PointingDevice : IPoitingDevice
    {
        public PointerButtons[] PressedButtons { get; private set; }

        public PointerButtons[] DownButtons { get; private set; }

        public PointerButtons[] ReleasedButtons { get; private set; }

        Vector2 position;
        bool pressedOnce = false;
        bool releasedOnce = false;

        public PointingDevice()
        {
            PressedButtons = new PointerButtons[] { PointerButtons.None };
            DownButtons = new PointerButtons[] { PointerButtons.None };
            ReleasedButtons = new PointerButtons[] { PointerButtons.None };
            position = new Vector2();
        }

        public void FirePressed(int index, PointerButtons button)
        {
            throw new NotImplementedException();
        }

        public void FireReleased(int index, PointerButtons button)
        {
            throw new NotImplementedException();
        }

        public Vector2 GetPosition(int index)
        {
            return position;
        }

        public bool IsButtonDown(int index, PointerButtons button)
        {
            return (DownButtons[0] & button) == button;
        }

        public bool IsButtonPressed(int index, PointerButtons button)
        {
            return (PressedButtons[0] & button) == button;
        }

        public bool IsButtonReleased(int index, PointerButtons button)
        {
            return (ReleasedButtons[0] & button) == button;
        }

        internal void MouseMove(OpenTK.Windowing.Common.MouseMoveEventArgs obj)
        {
            position = new Vector2(obj.X, obj.Y);
        }

        internal void MousePressed(OpenTK.Windowing.Common.MouseButtonEventArgs obj)
        {
            pressedOnce = false;
            if (obj.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left)
            {
                DownButtons[0] |= PointerButtons.Primary;
                PressedButtons[0] |= PointerButtons.Primary;
            }
            if (obj.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Right)
            {
                DownButtons[0] |= PointerButtons.Secondary;
                PressedButtons[0] |= PointerButtons.Secondary;
            }
            if (obj.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Middle)
            {
                DownButtons[0] |= PointerButtons.Alternative;
                PressedButtons[0] |= PointerButtons.Alternative;
            }
        }

        internal void MouseReleased(OpenTK.Windowing.Common.MouseButtonEventArgs obj)
        {
            releasedOnce = false;
            if (obj.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left)
            {
                DownButtons[0] = DownButtons[0] & ~PointerButtons.Primary;
                ReleasedButtons[0] |= PointerButtons.Primary;
            }
            if (obj.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Right)
            {
                DownButtons[0] = DownButtons[0] & ~PointerButtons.Secondary;
                ReleasedButtons[0] |= PointerButtons.Secondary;
            }
            if (obj.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Middle)
            {
                DownButtons[0] = DownButtons[0] & ~PointerButtons.Alternative;
                ReleasedButtons[0] |= PointerButtons.Alternative;
            }
        }

        public void Update()
        {
            PressedButtons[0] = PointerButtons.None;
            ReleasedButtons[0] = PointerButtons.None;
        }
    }
}
