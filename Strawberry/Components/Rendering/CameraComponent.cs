using Strawberry.Core;
using Strawberry.Math;

namespace Strawberry.Components.Rendering;

public class CameraComponent : BaseComponent
{
    /// <summary>
    /// The name of the viewport this camera moves. (Default is "Default")
    /// </summary>
    public string ViewportName { get; set; } = "Default";

    /// <summary>
    /// The border at which the viewport starts following the target, in pixels from the edge of the screen (Default is 32, 32)
    /// </summary>
    public Vector2 Border { get; set; } = new Vector2(32);

    /// <summary>
    /// The speed of the camera, if it is not centered on its target, if x or y is set -1 the camera will jump to the target instantly.
    /// (Default is -1, -1)
    /// </summary>
    public Vector2 Speed { get; set; } = new Vector2(-1, -1);

    /// <summary>
    /// Gets or sets whether the camera should center on its target or not, if true Border and Speed are ignored.
    /// </summary>
    public bool Center { get; set; } = false;

    private TransformComponent transform;

    public override void OnComponentAdded(BaseComponent component)
    {
        base.OnComponentAdded(component);
        if (component is TransformComponent transform)
        {
            this.transform = transform;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (transform == null)
            return;
        var vp = Scene.Viewports.Where(x => x.Name == ViewportName).FirstOrDefault();
        if (vp != null)
        {
            if (Center)
            {
                vp.ScenePos = transform.Position - (vp.SceneSize / 2);
            }
            else
            {
                if (transform.Position.X > vp.ScenePos.X + vp.SceneSize.X - Border.X)
                {
                    if (Speed.X == -1)
                    {
                        vp.ScenePos = new Vector2(transform.Position.X - vp.SceneSize.X + Border.X, vp.ScenePos.Y);
                    }
                    else
                    {
                        vp.ScenePos += Vector2.Right() * Speed.X * FrameInfo.Information.DeltaTime;
                    }
                }
                if (transform.Position.X < vp.ScenePos.X + Border.X)
                {
                    if (Speed.X == -1)
                    {
                        vp.ScenePos = new Vector2(transform.Position.X - Border.X, vp.ScenePos.Y);
                    }
                    else
                    {
                        vp.ScenePos += Vector2.Left() * Speed.X * FrameInfo.Information.DeltaTime;
                    }
                }

                if (transform.Position.Y > vp.ScenePos.Y + vp.SceneSize.Y - Border.Y)
                {
                    if (Speed.Y == -1)
                    {
                        vp.ScenePos = new Vector2(vp.ScenePos.X, transform.Position.Y - vp.SceneSize.Y + Border.Y);
                    }
                    else
                    {
                        vp.ScenePos += Vector2.Down() * Speed.Y * FrameInfo.Information.DeltaTime;
                    }
                }
                if (transform.Position.Y < vp.ScenePos.Y + Border.Y)
                {
                    if (Speed.Y == -1)
                    {
                        vp.ScenePos = new Vector2(vp.ScenePos.X, transform.Position.Y - Border.Y);
                    }
                    else
                    {
                        vp.ScenePos += Vector2.Up() * Speed.Y * FrameInfo.Information.DeltaTime;
                    }
                }
            }
        }
    }
}