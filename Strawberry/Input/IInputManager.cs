/*
 * Strawberry Game Engine
 * File: IInputManager.cs
 * Author: Koosha Aabedini Nassab
 *
 * Input manager interface for keyboard and pointing device handling.
 */

namespace Strawberry.Input
{
    /// <summary>
    /// The manager that handles all user input in the game
    /// </summary>
    public interface IInputManager
    {
        /// <summary>
        /// The pointing device used by the game. You can use this object to get information about mouse clicks or touch input.
        /// </summary>
        IPointingDevice PointingDevice { get; }

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
