using System;
using Android.Views;
using Strawberry.Input;
using Strawberry.Math;

namespace Strawberry.Android.Input;

internal class PointingDevice : IPointingDevice
{
    public PointerButtons[] PressedButtons { get; private set; } = new PointerButtons[10];

    public PointerButtons[] DownButtons { get; private set; } = new PointerButtons[10];

    public PointerButtons[] ReleasedButtons { get; private set; } = new PointerButtons[10];

    Vector2[] positions = new Vector2[10];


    public void FirePressed(int index, PointerButtons button)
    {
        throw new NotImplementedException();
    }

    public void FireReleased(int index, PointerButtons button)
    {
        throw new NotImplementedException();
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

    internal void OnTouch(View view,MotionEvent e)
    {
        if (e != null)
        {
            if (e.Action == MotionEventActions.Down)
            {
                DownButtons[e.ActionIndex] |= PointerButtons.Primary;
                PressedButtons[e.ActionIndex] |= PointerButtons.Primary;
            }

            if (e.Action == MotionEventActions.Up)
            {
                DownButtons[e.ActionIndex] = DownButtons[e.ActionIndex] & ~PointerButtons.Primary;
                ReleasedButtons[e.ActionIndex] |= PointerButtons.Primary;
            }

            for (int i = 0; i < e.PointerCount; i++)
            {
                int[] location = new int[2];
                view.GetLocationOnScreen(location);
                positions[i] = new Vector2(e.GetX(i) + location[0], e.GetY(i) + location[1]);
            }
        }
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
