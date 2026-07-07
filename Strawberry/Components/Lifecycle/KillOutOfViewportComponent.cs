using Strawberry.Common;
using Strawberry.Core;
using Strawberry.Math;

namespace Strawberry.Components.Lifecycle;

/// <summary>
/// Kills the owner when it goes out of a viewport
/// </summary>
public class KillOutOfViewportComponent : BaseComponent
{
    private TransformComponent transform = null;

    /// <summary>
    /// Gets or sets a border in pixels which is added to the scene bounds when checking for out of scene
    /// </summary>
    public Vector2 Border { get; set; }

    /// <summary>
    /// Gets or sets The name of the viewport to check. (Default is "Default")
    /// </summary>
    public string ViewportName { get; set; } = "Default";

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
            var vp = Owner.Scene.Viewports.Where(x => x.Name == ViewportName).FirstOrDefault();
            if (vp == null)
                return;

            var bounds = vp.Bounds;
            if (Border.X != 0)
            {
                bounds.X -= Border.X;
                bounds.Width += Border.X * 2;
            }
            if (Border.Y != 0)
            {
                bounds.Y -= Border.Y;
                bounds.Height += Border.Y * 2;
            }
            if (!bounds.IsPointInside(transform.Position))
            {
                Owner.Destroy();
            }
        }
    }
}