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
    /// Gets or sets the linear velocity of the body in scene units per second.
    /// Setting this directly overrides any velocity from forces or impulses.
    /// </summary>
    public Vector2 LinearVelocity
    {
        get => Body?.LinearVelocity * Scene.PixelPerMeter ?? Vector2.Zero;
        set
        {
            if (Body != null)
                Body.LinearVelocity = value / Scene.PixelPerMeter;
        }
    }

    /// <summary>
    /// Gets or sets the angular velocity of the body in radians per second.
    /// </summary>
    public float AngularVelocity
    {
        get => Body?.AngularVelocity ?? 0f;
        set
        {
            if (Body != null)
                Body.AngularVelocity = value;
        }
    }

    /// <summary>
    /// Gets or sets the linear damping coefficient. Higher values cause the body
    /// to slow down more quickly (simulates air resistance). Default is 0.
    /// </summary>
    public float LinearDamping
    {
        get => Body?.LinearDamping ?? 0f;
        set
        {
            if (Body != null)
                Body.LinearDamping = value;
        }
    }

    /// <summary>
    /// Gets or sets the angular damping coefficient. Higher values cause the body
    /// to stop rotating more quickly. Default is 0.
    /// </summary>
    public float AngularDamping
    {
        get => Body?.AngularDamping ?? 0f;
        set
        {
            if (Body != null)
                Body.AngularDamping = value;
        }
    }


    /// <summary>
    /// Gets the body's mass in kilograms.
    /// </summary>
    public float Mass => Body?.Mass ?? 0f;

    /// <summary>
    /// Gets the body's rotational inertia (resistance to angular acceleration).
    /// </summary>
    public float Inertia => Body?.Inertia ?? 0f;

    /// <summary>
    /// Gets whether the body is awake and simulating. Sleeping bodies are not
    /// processed by the physics engine to save CPU. Touching a sleeping body wakes it.
    /// </summary>
    public bool Awake
    {
        get => Body?.Awake ?? false;
        set
        {
            if (Body != null)
                Body.Awake = value;
        }
    }

    /// <summary>
    /// Gets or sets whether the body's rotation is locked. When true, the body
    /// can still move but won't rotate from forces or collisions. Useful for
    /// top-down characters that shouldn't tip over.
    /// </summary>
    public bool FixedRotation
    {
        get => Body?.FixedRotation ?? false;
        set
        {
            if (Body != null)
                Body.FixedRotation = value;
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

    /// <summary>
    /// Applies a force to the body's center of mass. Affects velocity gradually
    /// based on the body's mass. Use this for continuous pushes (thrusters, wind).
    /// </summary>
    /// <param name="force">The force vector in scene units per second squared.</param>
    public void ApplyForce(Vector2 force)
    {
        Body?.ApplyForce(force / Scene.PixelPerMeter);
    }

    /// <summary>
    /// Applies a force to a specific world-space point on the body. May generate torque
    /// if the point is not the center of mass (causing rotation).
    /// </summary>
    /// <param name="force">The force vector in scene units per second squared.</param>
    /// <param name="point">The world-space point where the force is applied, in scene units.</param>
    public void ApplyForce(Vector2 force, Vector2 point)
    {
        Body?.ApplyForce(force / Scene.PixelPerMeter, point / Scene.PixelPerMeter);
    }

    /// <summary>
    /// Applies an instantaneous change in momentum (impulse) to the body's center of mass.
    /// Immediately modifies velocity. Use this for jumps, hits, explosions.
    /// </summary>
    /// <param name="impulse">The impulse vector in scene units per second.</param>
    public void ApplyLinearImpulse(Vector2 impulse)
    {
        Body?.ApplyLinearImpulse(impulse / Scene.PixelPerMeter);
    }

    /// <summary>
    /// Applies an instantaneous impulse to a specific world-space point. May generate
    /// angular impulse if the point is not the center of mass (causing rotation).
    /// </summary>
    /// <param name="impulse">The impulse vector in scene units per second.</param>
    /// <param name="point">The world-space point where the impulse is applied, in scene units.</param>
    public void ApplyLinearImpulse(Vector2 impulse, Vector2 point)
    {
        Body?.ApplyLinearImpulse(impulse / Scene.PixelPerMeter, point / Scene.PixelPerMeter);
    }

    /// <summary>
    /// Applies an instantaneous angular impulse, immediately changing the body's
    /// angular velocity. Positive values rotate counter-clockwise.
    /// </summary>
    /// <param name="impulse">The angular impulse in radians per second.</param>
    public void ApplyAngularImpulse(float impulse)
    {
        Body?.ApplyAngularImpulse(impulse);
    }

    /// <summary>
    /// Applies a torque to the body, causing angular acceleration.
    /// </summary>
    /// <param name="torque">The torque in Newton-meters.</param>
    public void ApplyTorque(float torque)
    {
        Body?.ApplyTorque(torque);
    }

    /// <summary>
    /// Wakes the body if it was sleeping. Equivalent to <c>Awake = true</c>,
    /// but more readable in code like <c>rigidBody.Wake()</c> after applying an impulse.
    /// </summary>
    public void Wake()
    {
        if (Body != null)
            Body.Awake = true;
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