using Strawberry.Graphics;
using Strawberry.Input;
using Strawberry.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strawberry
{
    /// <summary>
    /// Provides mechanism for launching a game in different platforms.
    /// Methods implemented by using this interface are called by the Game class and you cannot run a game by implementing this interface alone
    /// </summary>
    public interface IGameLauncher
    {
        /// <summary>
        /// Underlining GraphicsContext by which the rendering is handled.
        /// </summary>
        IGraphicsContext GraphicsContext { get; }

        IInputManager InputManager { get; }

        ISoundManager SoundManager { get; }

        /// <summary>
        /// Happens when the platform independent initializations (opening window, etc.) is finished.
        /// </summary>
        event Action Initialized;

        /// <summary>
        /// Occurs everystep of the game. It is not a fixed step. fixed steps are handled by the game class. This should happen as fast as possible.
        /// </summary>
        event Action GameLoop;

        /// <summary>
        /// Starts the initialization process
        /// </summary>
        /// <param name="width">width of the window or rendering target</param>
        /// <param name="height">height of the window or rendering target</param>
        void Initialize(int width, int height);

        /// <summary>
        /// Runs the game.
        /// </summary>
        void Run();
    }
}
