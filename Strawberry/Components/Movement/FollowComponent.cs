using System;
using Strawberry.Core;
using Strawberry.Math;

namespace Strawberry.Components.Movement;

/// <summary>
/// Follows another entity's position at a specified offset, with optional smoothing for smooth catch-up.
/// Useful for pets, floating UI, and minimap blips.
/// </summary>
public class FollowComponent : BaseComponent
{
    /// <summary>
    /// Gets the transform of the owner.
    /// </summary>
    public TransformComponent Transform { get; private set; }

    /// <summary>
    /// Gets or sets the entity to follow.
    /// </summary>
    public Entity Target { get; set; }

    /// <summary>
    /// Gets or sets the offset from the target's position. (Default: Vector2.Zero)
    /// </summary>
    public Vector2 Offset { get; set; }

    /// <summary>
    /// Gets or sets the smoothing factor (0–1). 1 means instant follow, lower values mean smoother and slower catch-up. (Default: 1)
    /// </summary>
    public float LerpFactor { get; set; } = 1f;

    public override void OnComponentAdded(BaseComponent component)
    {
        base.OnComponentAdded(component);
        if (component is TransformComponent transformComponent && Transform == null)
            Transform = transformComponent;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Transform == null || Target == null)
            return;

        if (Target.TryGetComponent<TransformComponent>(out var targetTransform))
        {
            Vector2 desired = targetTransform.Position + Offset;
            float t = 1f - MathF.Pow(1f - LerpFactor, FrameInfo.Information.DeltaTime * 60f);
            Transform.Position = Vector2.Lerp(Transform.Position, desired, t);
        }
    }
}