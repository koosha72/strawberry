using Strawberry.Core;

namespace Strawberry.Components.Movement;

/// <summary>
/// Rotates the entity around its own axis
/// </summary>
public class RotateComponent : BaseComponent
{
    /// <summary>
    /// Gets the transform of the owner
    /// </summary>
    public TransformComponent Transform { get; private set; }

    /// <summary>
    /// Gets or sets the angular speed of the rotation.
    /// </summary>
    public float AngularSpeed { get; set; }

    public override void OnComponentAdded(BaseComponent component)
    {
        base.OnComponentAdded(component);
        if (component is TransformComponent transform && Transform == null)
            Transform = transform;
    }


    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Transform != null)
            Transform.Angle = (Transform.Angle + AngularSpeed * FrameInfo.Information.DeltaTime) % (2f * MathF.PI);
    }
}