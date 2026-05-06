using System;
using Strawberry.Math;
using Strawberry.Components;
using Strawberry.Core;
using Strawberry.Sound;

namespace Strawberry.Test;

public class SoundListenerComponent : BaseComponent
{
    public TransformComponent Transform { get { return Owner.GetComponent<TransformComponent>(); } }

    public Sound3DListener Listener { get; set; }

    Vector3 previousPosition;

    public void Begin()
    {
        Listener = GameContext.SoundManager.Create3DListener(new Vector3(Transform.Position, 0f), Vector3.Zero, -Vector3.UnitZ, new Vector3(-Vector2.Up(), 0f), true);
        Listener.FallOffMode = FallOffMode.InverseDistanceClamped;
    }

    public override void OnUpdate()
    {
        Listener.Position = new Vector3(Transform.Position, 0f);
        if (previousPosition != Vector3.Zero)
        {
            //Listener.Velocity = (Listener.Position - previousPosition) / FrameInfo.Information.DeltaTime;
        }
        
        previousPosition = Listener.Position;
    }
}
