using Strawberry.Math;

namespace Strawberry.Input
{
    /// <summary>
    /// A pointing device like mouse, touchpad or touchscreen.
    /// </summary>
    public interface IPoitingDevice
    {
        /// <summary>
        /// The buttons that are pressed. A button can only be pressed for 1 FixedUpdate.
        /// The pressed button will be available in DownButtons untill the button is released;
        /// </summary>
        PointerButtons[] PressedButtons { get; }
        /// <summary>
        /// The buttons that are down.
        /// </summary>
        PointerButtons[] DownButtons { get; }
        /// <summary>
        /// The buttons that are released. A button can only be released for 1 FixedUpdate.
        /// The released button will be removed from DownButtons
        /// </summary>
        PointerButtons[] ReleasedButtons { get; }

        /// <summary>
        /// Gets the position of the pointer.
        /// </summary>
        /// <param name="index">The index of the pointer (used for multi-touch screens)</param>
        /// <returns></returns>
        Vector2 GetPosition(int index);

        /// <summary>
        /// Changes the state of the button to pressed
        /// </summary>
        /// <param name="index">Index of the touch/pointer</param>
        /// <param name="button">The pressed button</param>
        void FirePressed(int index, PointerButtons button);
        /// <summary>
        /// Changes the state of the button to released
        /// </summary>
        /// <param name="index">Index of the touch/pointer</param>
        /// <param name="button">The released button</param>
        void FireReleased(int index, PointerButtons button);

        /// <summary>
        /// Checks wheather the specified button is down.
        /// </summary>
        /// <param name="button">The button to check</param>
        /// <returns>True if the button is down</returns>
        bool IsButtonDown(int index, PointerButtons button);
        /// <summary>
        /// Checks wheather the specified button is pressed.
        /// </summary>
        /// <param name="button">The button to check</param>
        /// <returns>True if the button is pressed</returns>
        bool IsButtonPressed(int index, PointerButtons button);
        /// <summary>
        /// Checks wheather the specified button is released.
        /// </summary>
        /// <param name="button">The button to check</param>
        /// <returns>True if the button is released</returns>
        bool IsButtonReleased(int index, PointerButtons button);
    }
}
