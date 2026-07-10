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

    public override void OnRender()
    {
        base.OnUpdate();
        if (transform == null) return;

        var vp = Scene.Viewports.FirstOrDefault(x => x.Name == ViewportName);
        if (vp == null) return;

        Vector2 targetPos = vp.ScenePos;

        if (Center)
        {
            targetPos = transform.Position - (vp.SceneSize / 2);
        }
        else
        {
            if (transform.Position.X > vp.ScenePos.X + vp.SceneSize.X - Border.X)
            {
                targetPos.X = transform.Position.X - vp.SceneSize.X + Border.X;
            }
            else if (transform.Position.X < vp.ScenePos.X + Border.X)
            {
                targetPos.X = transform.Position.X - Border.X;
            }

            if (transform.Position.Y > vp.ScenePos.Y + vp.SceneSize.Y - Border.Y)
            {
                targetPos.Y = transform.Position.Y - vp.SceneSize.Y + Border.Y;
            }
            else if (transform.Position.Y < vp.ScenePos.Y + Border.Y)
            {
                targetPos.Y = transform.Position.Y - Border.Y;
            }
        }

        if (Speed.X == -1 && Speed.Y == -1)
        {
            vp.ScenePos = targetPos;
        }
        else
        {
            Vector2 diff = targetPos - vp.ScenePos;

            float moveX = System.MathF.Sign(diff.X) * Speed.X * FrameInfo.Information.DeltaTime;
            float moveY = System.MathF.Sign(diff.Y) * Speed.Y * FrameInfo.Information.DeltaTime;

            if (System.MathF.Abs(moveX) > System.MathF.Abs(diff.X)) moveX = diff.X;
            if (System.MathF.Abs(moveY) > System.MathF.Abs(diff.Y)) moveY = diff.Y;

            vp.ScenePos += new Vector2(moveX, moveY);
        }
    }
}