/*
 * Strawberry Game Engine
 * File: IKeyboard.cs
 * Author: Koosha Aabedini Nassab
 *
 * Keyboard input interface for querying and simulating key states.
 */

namespace Strawberry.Input
{
    /// <summary>
    /// Defines an interface for keyboard input handling, providing access to key states 
    /// and methods to query and simulate keyboard events.
    /// </summary>
    public interface IKeyboard
    {
        /// <summary>
        /// Gets the collection of keys that are currently being held down.
        /// </summary>
        IEnumerable<Keys> DownKeys { get; }

        /// <summary>
        /// Gets the collection of keys that were pressed during the current frame.
        /// </summary>
        IEnumerable<Keys> PressedKeys { get; }

        /// <summary>
        /// Gets the collection of keys that were released during the current frame.
        /// </summary>
        IEnumerable<Keys> ReleasedKeys { get; }

        /// <summary>
        /// Triggers the pressed state for the specified key.
        /// Typically called by the input backend to signal a key press event.
        /// </summary>
        /// <param name="key">The key that was pressed.</param>
        void FirePressed(Keys key);

        /// <summary>
        /// Triggers the released state for the specified key.
        /// Typically called by the input backend to signal a key release event.
        /// </summary>
        /// <param name="key">The key that was released.</param>
        void FireReleased(Keys key);

        /// <summary>
        /// Determines whether the specified key is currently being held down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns><c>true</c> if the key is currently held down; otherwise, <c>false</c>.</returns>
        bool IsKeyDown(Keys key);

        /// <summary>
        /// Determines whether the specified key was just pressed during the current frame.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns><c>true</c> if the key was pressed in this frame; otherwise, <c>false</c>.</returns>
        bool IsKeyPressed(Keys key);

        /// <summary>
        /// Determines whether the specified key was just released during the current frame.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns><c>true</c> if the key was released in this frame; otherwise, <c>false</c>.</returns>
        bool IsKeyReleased(Keys key);
    }
}