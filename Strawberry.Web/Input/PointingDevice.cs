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


    public void FirePressed(int index, PointerButtons button)
    {

    }

    public void FireReleased(int index, PointerButtons button)
    {

    }

    public Vector2 GetPosition(int index)
    {
        return new Vector2();
    }

    public bool IsButtonDown(int index, PointerButtons button)
    {
        return false;
    }

    public bool IsButtonPressed(int index, PointerButtons button)
    {
        return false;
    }

    public bool IsButtonReleased(int index, PointerButtons button)
    {
        return false;
    }
}
