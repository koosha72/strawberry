using Strawberry.Input;

namespace Strawberry.OpenGL.Input
{
    public class Keyboard : IKeyboard
    {
        public IEnumerable<Keys> DownKeys => downKeys;

        public IEnumerable<Keys> PressedKeys => pressedKeys;

        public IEnumerable<Keys> RelasedKeys => releasedKeys;

        List<Keys> downKeys = new List<Keys>();
        List<Keys> pressedKeys = new List<Keys>();
        List<Keys> releasedKeys = new List<Keys>();

        bool pressedOnce = false;
        bool releasedOnce = false;

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

        internal void KeyPressed(OpenTK.Windowing.Common.KeyboardKeyEventArgs obj)
        {
            if (!obj.IsRepeat)
            {
                pressedOnce = false;
                pressedKeys.Add((Keys)obj.Key);
                downKeys.Add((Keys)obj.Key);
            }
        }

        internal void KeyReleased(OpenTK.Windowing.Common.KeyboardKeyEventArgs obj)
        {
            releasedOnce = false;
            releasedKeys.Add((Keys)obj.Key);
            if (downKeys.Contains((Keys)obj.Key))
                downKeys.Remove((Keys)obj.Key);
        }

        public void Update()
        {
            if (pressedOnce)
                pressedKeys.Clear();
            if (pressedKeys.Count() > 0)
                pressedOnce = true;
            if (releasedOnce)
                releasedKeys.Clear();
            if (releasedKeys.Count() > 0)
                releasedOnce = true;
        }
    }
}
