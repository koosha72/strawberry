using Strawberry.Core;
using Strawberry.Math;

namespace Strawberry.Components.Movement;

public class LookAtComponent : BaseComponent
{
    /// <summary>
    /// Gets the transform of the owner
    /// </summary>
    public TransformComponent Transform { get; private set; }

    public Vector2 Target { get; set; }

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
            Transform.Angle = (float)(Target - Transform.Position).Direction;
    }
}