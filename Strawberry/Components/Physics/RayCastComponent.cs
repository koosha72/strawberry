using Strawberry.Core;
using Strawberry.EventSystem;
using Strawberry.Math;
using tainicom.Aether.Physics2D.Dynamics;

namespace Strawberry.Components.Physics;

/// <summary>
/// This event occurs when a ray starts hitting a fixture.
/// </summary>
public struct RayHitEvent : IStrawberryEvent
{
    /// <summary>
    /// The ray cast component that started hitting a fixture.
    /// </summary>
    public RayCastComponent RayCast { get; init; }
}

/// <summary>
/// This event occurs when a ray stops hitting a fixture.
/// </summary>
public struct RayClearEvent : IStrawberryEvent
{
    /// <summary>
    /// The ray cast component that stopped hitting a fixture.
    /// </summary>
    public RayCastComponent RayCast { get; init; }
}

/// <summary>
/// Casts a ray in a specified direction each FixedUpdate and exposes the hit results.
/// Used for ground detection, wall detection and ledge detection.
/// </summary>
public class RayCastComponent : BaseComponent
{
    /// <summary>
    /// Gets the transform of the owner.
    /// </summary>
    public TransformComponent Transform { get; private set; }

    Vector2 direction = Vector2.Down();

    /// <summary>
    /// Gets or sets the direction of the ray in world space. (Default: Down)
    /// </summary>
    public Vector2 Direction
    {
        get => direction;
        set => direction = value.LengthSquared() > 0f ? value : Vector2.Down();
    }

    /// <summary>
    /// Gets or sets the maximum distance of the ray in pixels. (Default: 10)
    /// </summary>
    public float Distance { get; set; } = 10f;

    /// <summary>
    /// Gets or sets the starting offset of the ray relative to the entity's position, in pixels. (Default: Vector2.Zero)
    /// </summary>
    public Vector2 Offset { get; set; }

    /// <summary>
    /// Gets or sets the collision categories the ray will detect. Only fixtures whose CollisionCategories overlap with this value will be detected. (Default: Category.All)
    /// </summary>
    public Category CollidesWith { get; set; } = Category.All;

    /// <summary>
    /// Gets whether the ray is currently hitting a fixture.
    /// </summary>
    public bool IsHit { get; private set; }

    /// <summary>
    /// Gets the distance from the ray origin to the hit point, in pixels. Returns 0 if not hitting.
    /// </summary>
    public float HitDistance { get; private set; }

    /// <summary>
    /// Gets the normal of the surface at the hit point.
    /// </summary>
    public Vector2 HitNormal { get; private set; }

    /// <summary>
    /// Gets the world position where the ray hit a fixture.
    /// </summary>
    public Vector2 HitPoint { get; private set; }

    /// <summary>
    /// Gets the fixture that was hit, or null if not hitting.
    /// </summary>
    public Fixture HitFixture { get; private set; }

    /// <summary>
    /// Gets the rigid body component of the owner, if any. Used to ignore the owner's own fixtures.
    /// </summary>
    public RigidBodyComponent RigidBody { get; private set; }

    public override void OnComponentAdded(BaseComponent component)
    {
        base.OnComponentAdded(component);
        if (component is TransformComponent transform && Transform == null)
            Transform = transform;
        if (component is RigidBodyComponent rigidBody && RigidBody == null)
            RigidBody = rigidBody;
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if (Transform == null || Scene?.PhysicsWorld == null || Distance <= 0f)
            return;

        var ppm = Scene.PixelPerMeter;
        var directionNormalized = Vector2.Normalize(direction);
        var origin = (Transform.Position + Offset) / ppm;
        var end = origin + directionNormalized * (Distance / ppm);

        bool wasHit = IsHit;
        IsHit = false;
        HitFixture = null;
        HitDistance = 0f;
        HitNormal = Vector2.Zero;
        HitPoint = Vector2.Zero;

        Scene.PhysicsWorld.RayCast((fixture, point, normal, fraction) =>
        {
            if (RigidBody?.Body == fixture.Body)
                return -1f;

            if ((fixture.CollisionCategories & CollidesWith) == 0)
                return -1f;

            IsHit = true;
            HitFixture = fixture;
            HitDistance = fraction * Distance;
            HitNormal = normal;
            HitPoint = point * ppm;

            return fraction;
        }, origin, end);

        if (IsHit && !wasHit)
            EventManager.Invoke(this, new RayHitEvent { RayCast = this });
        if (!IsHit && wasHit)
            EventManager.Invoke(this, new RayClearEvent { RayCast = this });
    }
}