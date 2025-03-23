namespace Strawberry.Input
{
    public interface IKeyboard
    {
        IEnumerable<Keys> DownKeys { get; }
        IEnumerable<Keys> PressedKeys { get; }
        IEnumerable<Keys> RelasedKeys { get; }

        void FirePressed(Keys key);
        void FireReleased(Keys key);

        bool IsKeyDown(Keys key);
        bool IsKeyPressed(Keys key);
        bool IsKeyReleased(Keys key);
    }
}
