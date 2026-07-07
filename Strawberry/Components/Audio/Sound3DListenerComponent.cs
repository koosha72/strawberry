using Strawberry.Core;
using Strawberry.Math;
using Strawberry.Sound;

namespace Strawberry.Components.Audio;

/// <summary>
/// Makes an active 3d sound listener and moves it to the position of the owner's transform on each update.
/// </summary>
public class Sound3DListenerComponent : BaseComponent
{
    private TransformComponent transform = null;

    Sound3DListener listener = null;

    /// <summary>
    /// Gets the underlying sound listener object.
    /// </summary>
    public Sound3DListener Listener => listener;

    public override void OnBegin()
    {
        base.OnBegin();
    }

    public override void OnComponentAdded(BaseComponent component)
    {
        base.OnComponentAdded(component);

        if (component is TransformComponent tc)
        {
            this.transform = tc;
            if (listener == null)
            {
                listener = GameContext.SoundManager.Create3DListener(new Vector3(transform.Position, 0f), Vector3.Zero, -Vector3.UnitZ, new Vector3(-Vector2.Up(), 0f)
                    , true);
                listener.FallOffMode = FallOffMode.InverseDistanceClamped;
            }
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (transform != null && listener != null)
        {
            listener.Position = new Vector3(transform.Position, 0f);
        }
    }
}