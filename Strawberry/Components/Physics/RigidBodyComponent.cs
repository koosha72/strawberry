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

/// <summary>
/// The event occurs when two physical bodies separate
/// </summary>
public struct SeparationEvent : IStrawberryEvent
{
    /// <summary>
    /// The rigid body raising the event. (The event caller).
    /// </summary>
    public RigidBodyComponent RigidBody { get; init; }

    /// <summary>
    /// The fixture that was colliding
    /// </summary>
    public Fixture Self { get; init; }

    /// <summary>
    /// The other fixture that separated
    /// </summary>
    public Fixture Other { get; init; }
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

    /// <summary>
    /// A read-only collection of fixtures currently colliding with this body
    /// </summary>
    public IReadOnlyCollection<Fixture> CollidingFixtures => collidingFixtures;

    /// <summary>
    /// Gets whether this body is currently colliding with any other body
    /// </summary>
    public bool IsColliding => collidingFixtures.Count > 0;

    private readonly HashSet<Fixture> collidingFixtures = new();

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

    /// <summary>
    /// Checks if this body is currently colliding with the specified fixture
    /// </summary>
    /// <param name="fixture">The fixture to check against</param>
    /// <returns>True if currently colliding with the specified fixture</returns>
    public bool IsCollidingWith(Fixture fixture) => collidingFixtures.Contains(fixture);

    /// <summary>
    /// Checks if this body is currently colliding with any fixture belonging to the specified body
    /// </summary>
    /// <param name="body">The body to check against</param>
    /// <returns>True if currently colliding with any fixture from the specified body</returns>
    public bool IsCollidingWithBody(Body body)
    {
        foreach (var fixture in collidingFixtures)
        {
            if (fixture.Body == body)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if this body is currently colliding with any fixture belonging to the specified rigid body component
    /// </summary>
    /// <param name="rigidBody">The rigid body component to check against</param>
    /// <returns>True if currently colliding with the specified rigid body</returns>
    public bool IsCollidingWith(RigidBodyComponent rigidBody) => rigidBody != null && IsCollidingWithBody(rigidBody.Body);

    public override void OnComponentAdded(BaseComponent component)
    {
        base.OnComponentAdded(component);
        if (component is TransformComponent transform && Transform == null)
        {
            Transform = transform;
            if (Body == null)
            {
                Body = Scene.PhysicsWorld.CreateBody(Transform.Position / Scene.PixelPerMeter, -MathHelper.ToRadians(Transform.Angle), Type);
                Body.IgnoreGravity = ignoreGravity;
            }
        }
    }

    /// <summary>
    /// Enables collision and separation callbacks for the physical body.
    /// </summary>
    public void EnableCollisionEvent()
    {
        if (Body == null || collisionEnabled)
            return;
        Body.OnCollision += OnCollision;
        Body.OnSeparation += OnSeparation;
        collisionEnabled = true;
    }

    bool OnCollision(Fixture self, Fixture other, Contact contact)
    {
        collidingFixtures.Add(other);

        EventManager.Invoke<CollisionEvent>(this, new CollisionEvent
        {
            RigidBody = this,
            Self = self,
            Other = other,
            Contact = contact
        });

        return true;
    }

    void OnSeparation(Fixture self, Fixture other, Contact contact)
    {
        collidingFixtures.Remove(other);

        EventManager.Invoke<SeparationEvent>(this, new SeparationEvent
        {
            RigidBody = this,
            Self = self,
            Other = other
        });
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
            Body.OnCollision -= OnCollision;
            Body.OnSeparation -= OnSeparation;
            Scene.PhysicsWorld.Remove(Body);
            collidingFixtures.Clear();
        }
        base.OnFinished();
    }
}