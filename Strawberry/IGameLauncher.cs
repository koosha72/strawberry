/*
 * Strawberry Game Engine
 * File: IGameLauncher.cs
 * Author: Koosha Aabedini Nassab
 *
 * Interface for platform-specific game launch and loop control.
 */

using Strawberry.Graphics;
using Strawberry.Input;
using Strawberry.Misc;
using Strawberry.Sound;

namespace Strawberry
{
    /// <summary>
    /// Provides mechanism for launching a game in different platforms.
    /// Methods implemented by using this interface are called by the Game class and you cannot run a game by implementing this interface alone
    /// </summary>
    public interface IGameLauncher
    {
        /// <summary>
        /// Gets the nderlining GraphicsContext by which the rendering is handled.
        /// </summary>
        IGraphicsContext GraphicsContext { get; }

        /// <summary>
        /// Gets the object that handles platform specific input.
        /// </summary>
        IInputManager InputManager { get; }

        /// <summary>
        /// Gets the platform specific object responsible for playing and managing sounds.
        /// </summary>
        ISoundManager SoundManager { get; }
        /// <summary>
        /// Gets the platform specific object responsible for retrieving data from a storage device (e.g. file system).
        /// </summary>
        IStorage Storage { get; }

        /// <summary>
        /// Occurs when the platform independent initializations (opening window, etc.) is finished.
        /// </summary>
        event Action Initialized;

        /// <summary>
        /// Occurs every step of the game. It is not a fixed step. fixed steps are handled by the game class. This should happen as fast as possible.
        /// </summary>
        event Action GameLoop;

        /// <summary>
        /// Starts the initialization process. By default this is called by the game class, do not call it!
        /// </summary>
        /// <param name="width">width of the window or rendering target</param>
        /// <param name="height">height of the window or rendering target</param>
        void Initialize(int width, int height);

        /// <summary>
        /// Runs the game. By default this is called by the game class, do not call it!
        /// </summary>
        void Run();
    }
}
