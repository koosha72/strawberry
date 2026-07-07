using Strawberry.Common;
using Strawberry.Core;
using Strawberry.Math;

namespace Strawberry.Components.Lifecycle;

/// <summary>
/// Kills the owner when it goes out of scene
/// </summary>
public class KillOutOfSceneComponent : BaseComponent
{
    private TransformComponent transform = null;

    /// <summary>
    /// Gets or sets a border in pixels which is added to the scene bounds when checking for out of scene
    /// </summary>
    public Vector2 Border { get; set; }

    public override void OnComponentAdded(BaseComponent component)
    {
        base.OnComponentAdded(component);
        if (component is TransformComponent transform)
            this.transform = transform;
    }

    public override void OnUpdate()
    {
        if (Owner != null && !Owner.Destroyed && transform != null)
        {
            var bounds = Owner.Scene.Bounds;
            if (Border.X != 0)
            {
                bounds.Left -= Border.X;
                bounds.Width += Border.X * 2;
            }
            if (Border.Y != 0)
            {
                bounds.Top -= Border.Y;
                bounds.Height += Border.Y * 2;
            }
            if (!bounds.IsPointInside(transform.Position))
            {
                Owner.Destroy();
            }
        }
    }
}