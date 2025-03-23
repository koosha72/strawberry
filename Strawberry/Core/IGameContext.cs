using Strawberry.Graphics;
using Strawberry.Input;
using Strawberry.Sound;

namespace Strawberry.Core
{
    /// <summary>
    /// Represents the context of a game, providing access to essential components
    /// and lifecycle methods for initialization, updating, and rendering.
    /// </summary>
    public interface IGameContext
    {
        /// <summary>
        /// Gets the width of the game context.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the game context.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the graphics context used for rendering.
        /// </summary>
        IGraphicsContext GraphicsContext { get; }

        /// <summary>
        /// Gets the input manager for handling user input.
        /// </summary>
        IInputManager InputManager { get; }

        /// <summary>
        /// Gets the sound manager for handling audio playback.
        /// </summary>
        ISoundManager SoundManager { get; }

        /// <summary>
        /// Called during the initialization phase of the game.
        /// </summary>
        /// <param name="launcher">The game launcher used to initialize the game.</param>
        void OnInitialize(IGameLauncher launcher);

        /// <summary>
        /// Called at the beginning of the update phase.
        /// </summary>
        /// <returns>True if the update phase should proceed; otherwise, false.</returns>
        bool OnBeginUpdate();

        /// <summary>
        /// Called during the update phase to update game logic.
        /// </summary>
        void OnUpdate();

        /// <summary>
        /// Called during the fixed update phase for physics or time-sensitive updates.
        /// </summary>
        void OnFixedUpdate();

        /// <summary>
        /// Called at the end of the update phase.
        /// </summary>
        void OnEndUpdate();

        /// <summary>
        /// Called at the beginning of the render phase.
        /// </summary>
        /// <returns>True if the render phase should proceed; otherwise, false.</returns>
        bool OnBeginRender();

        /// <summary>
        /// Called during the render phase to draw the game.
        /// </summary>
        void OnRender();

        /// <summary>
        /// Called at the end of the render phase.
        /// </summary>
        void OnEndRender();
    }
}
