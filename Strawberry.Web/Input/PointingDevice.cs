using System;
using Strawberry.Input;
using Strawberry.Math;

namespace Strawberry.Web.Input;

public class PointingDevice : IPoitingDevice
{
    public PointerButtons[] PressedButtons { get; private set; } = new PointerButtons[10];

    public PointerButtons[] DownButtons { get; private set; } = new PointerButtons[10];

    public PointerButtons[] ReleasedButtons { get; private set; } = new PointerButtons[10];

    Vector2[] positions = new Vector2[10];

    public PointingDevice()
    {
        Interop.PointerDown += OnPointerDown;
        Interop.PointerUp += OnPointerUp;
        Interop.PointerMove += OnPointerMove;
    }


    public void FirePressed(int index, PointerButtons button)
    {

    }

    public void FireReleased(int index, PointerButtons button)
    {

    }

    public Vector2 GetPosition(int index)
    {
        return positions[index];
    }

    public bool IsButtonDown(int index, PointerButtons button)
    {
        return (DownButtons[index] & button) == button;
    }

    public bool IsButtonPressed(int index, PointerButtons button)
    {
        return (PressedButtons[index] & button) == button;
    }

    public bool IsButtonReleased(int index, PointerButtons button)
    {
        return (ReleasedButtons[index] & button) == button;
    }

    private void OnPointerDown(PointerButtons button, int index, bool shift, bool ctrl, bool alt)
    {
        DownButtons[index] |= button;
        PressedButtons[index] |= button;
    }

    private void OnPointerUp(PointerButtons button, int index, bool shift, bool ctrl, bool alt)
    {
        DownButtons[index] &= ~button;
        ReleasedButtons[index] |= button;
    }

    private void OnPointerMove(int index, float x, float y)
    {
        positions[index] = new Vector2(x, y);
    }

    public void Update()
    {
        for (int i = 0; i < PressedButtons.Length; i++)
        {
            PressedButtons[i] = PointerButtons.None;
            ReleasedButtons[i] = PointerButtons.None;
        }
    }
}
