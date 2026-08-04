/*
 * Strawberry Game Engine
 * File: IGameContext.cs
 * Author: Koosha Aabedini Nassab
 *
 * Interface representing the main game context and lifecycle hooks.
 */

using Strawberry.Graphics;
using Strawberry.Input;
using Strawberry.Math;
using Strawberry.Sound;

namespace Strawberry.Core
{
    /// <summary>
    /// Represents the context of a game, providing access to essential components
    /// and lifecycle methods for initialization, updating, and rendering.
    /// You should implement this interface in your game and launch it using Game class's Run method.
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
        /// Gets the asset manager of this game context. Used as a Global Asset Manager.
        /// </summary>
        AssetManager Assets { get; }

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

        /// <summary>
        /// Called when the game rendering area (window, surface or canvas) has been resized.
        /// </summary>
        /// <param name="size">The new size of the rendering area</param>
        void OnResized(Vector2 size);

        /// <summary>
        /// Called when the game has lost the focus (e.g. on windows the game window is minimized).
        /// </summary>
        void OnFocusLost();

        /// <summary>
        /// Called when the game has gained the focus (e.g. on windows the game window focused).
        /// </summary>
        void OnFocusGained();

        /// <summary>
        /// Called when the game is exiting, just before the application is shutdown.
        /// </summary>
        void OnClosing();

        /// <summary>
        /// Called when the graphics context is lost (never called on desktop platforms).
        /// </summary>
        void OnGraphicsContextLost();

        /// <summary>
        /// Called when the graphics context is restored (never called on desktop platforms).
        /// Can be used to restore assets (like textures) on android and web platforms.
        /// </summary>
        void OnGraphicsContextRestored();
    }
}
