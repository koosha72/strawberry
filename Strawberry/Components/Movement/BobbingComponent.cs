using System;
using Strawberry.Core;
using Strawberry.EventSystem;
using Strawberry.Math;

namespace Strawberry.Components.Movement;

/// <summary>
/// Implements a bobbing animation on the entity.
/// </summary>
public class BobbingComponent : BaseComponent
{
    /// <summary>
    /// Gets the transform of the owner
    /// </summary>
    public TransformComponent Transform { get; private set; }

    /// <summary>
    /// Gets or sets the amplitude (Height) of the bobbing animation. (Default: 5)
    /// </summary>
    public float Amplitude { get; set; } = 4f;

    /// <summary>
    /// Gets or sets the frequency (Cycles per second) of the bobbing animation. (Default: 1)
    /// </summary>
    public float Frequency { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets the phase (Offset) of the bobbing animation. (Default: 0)
    /// </summary>
    public float Phase { get; set; } = 0.0f;

    private float elapsed;

    float previousOffset;

    public override void OnComponentAdded(BaseComponent component)
    {
        base.OnComponentAdded(component);
        if (component is TransformComponent transformComponent && Transform == null)
        {
            Transform = transformComponent;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        elapsed += FrameInfo.Information.DeltaTime;
        float offset = Amplitude *
               MathF.Sin((2f * MathF.PI * Frequency * elapsed) + Phase);

        float delta = offset - previousOffset;
        previousOffset = offset;

        Transform.Position += Vector2.Up() * delta;
    }
}