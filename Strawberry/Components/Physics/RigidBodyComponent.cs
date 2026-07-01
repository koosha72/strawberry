using Strawberry.Core;
using Strawberry.EventSystem;
using Strawberry.Math;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace Strawberry.Components.Physics;


/// <summary>
/// The way the transformation elements are updated relative to the world and physics
/// </summary>
public enum UpdateDirection
{
    /// <summary>
    /// The transformation is retrieved from physical world.
    /// </summary>
    PhysicsToWorld,

    /// <summary>
    /// The physical body is moved to TransformComponent's position, (Used for collision detection only)
    /// </summary>
    WorldToPhysics,
}

/// <summary>
/// The event occurs when two physical bodies collide
/// </summary>
public struct CollisionEvent : IStrawberryEvent
{
    /// <summary>
    /// The rigid body raising the event. (The event caller).
    /// </summary>
    public RigidBodyComponent RigidBody { get; init; }

    /// <summary>
    /// The fixture colliding with another object
    /// </summary>
    public Fixture Self { get; init; }

    /// <summary>
    /// The other fixture colliding with the first one.
    /// </summary>
    public Fixture Other { get; init; }

    /// <summary>
    /// The contact information of the collision event
    /// </summary>
    public Contact Contact { get; init; }
}


public class RigidBodyComponent : BaseComponent
{
    /// <summary>
    /// Gets the transform of the owner
    /// </summary>
    public TransformComponent Transform { get; private set; }

    /// <summary>
    /// The physics body used by the component
    /// </summary>
    public Body Body { get; private set; }

    /// <summary>
    /// The direction of the update, See <see cref="UpdateDirection"/>. (Default is UpdateDirection.PhysicsToWorld)
    /// </summary>
    public UpdateDirection UpdateDirection { get; set; } = UpdateDirection.PhysicsToWorld;

    BodyType bodyType = BodyType.Static;

    bool collisionEnabled = false;

    public BodyType Type
    {
        get => Body?.BodyType ?? bodyType;
        set
        {
            bodyType = value;
            if (Body != null)
                Body.BodyType = value;
        }
    }

    private bool ignoreGravity = false;

    /// <summary>
    /// Gets or sets whether gravity should be ignored (Default is false)
    /// </summary>
    public bool IgnoreGravity
    {
        get => Body?.IgnoreGravity ?? ignoreGravity;
        set
        {
            ignoreGravity = value;
            if (Body != null)
                Body.IgnoreGravity = value;
        }
    }


    public override void OnComponentAdded(BaseComponent component)
    {
        base.OnComponentAdded(component);
        if (component is TransformComponent transform && Transform == null)
        {
            Transform = transform;
            if (Body == null) {
                Body = Scene.PhysicsWorld.CreateBody(Transform.Position / Scene.PixelPerMeter, -MathHelper.ToRadians(Transform.Angle), Type);
                Body.IgnoreGravity = ignoreGravity;
            }
        }
    }

    /// <summary>
    /// Enables collision callbacks for the physical body.
    /// </summary>
    public void EnableCollisionEvent()
    {
        if (Body == null || collisionEnabled)
            return;
        Body.OnCollision += Collision;
        collisionEnabled = true;
    }

    bool Collision(Fixture firstBody, Fixture secondBody, Contact contact)
    {
        EventManager.Invoke<CollisionEvent>(this, new CollisionEvent
        {
            RigidBody = this,
            Self = firstBody,
            Other = secondBody,
            Contact = contact
        });

        return true;
    }

    public override void OnFixedUpdate()
    {
        if (Body == null)
            return;
        if (UpdateDirection == UpdateDirection.WorldToPhysics)
        {
            Body.Position = Transform.Position / Scene.PixelPerMeter;
            Body.Rotation = -MathHelper.ToRadians(Transform.Angle);
        }
        else
        {
            Transform.Position = Body.Position * Scene.PixelPerMeter;
            Transform.Angle = -MathHelper.ToDegrees(Body.Rotation);
        }
        base.OnFixedUpdate();
    }

    public override void OnFinished()
    {
        if (Body != null)
        {
            Scene.PhysicsWorld.Remove(Body);
            Body.OnCollision -= Collision;
        }
        base.OnFinished();
    }
}