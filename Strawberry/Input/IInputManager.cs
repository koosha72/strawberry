namespace Strawberry.Input
{
    public interface IInputManager
    {
        /// <summary>
        /// The pointing device used by the game. You can use this object to get information about mouse clicks or touch input.
        /// </summary>
        IPoitingDevice PointingDevice { get; }

        /// <summary>
        /// The keyboard device used by the game.
        /// </summary>
        IKeyboard Keyboard { get; }

        /// <summary>
        /// Initializes the input manager setting up the keyboard and pointing device.
        /// </summary>
        void Initialize();

        void Update();
    }
}
