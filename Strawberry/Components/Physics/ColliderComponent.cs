using Strawberry.Core;
using Strawberry.EventSystem;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace Strawberry.Components.Physics;

/// <summary>
/// Base class for colliders. The collider's shape will be created in the next FixedUpdate iteration.
/// </summary>
public abstract class ColliderComponent : BaseComponent
{
    /// <summary>
    /// Gets the transform of the owner
    /// </summary>
    public TransformComponent Transform { get; private set; }

    /// <summary>
    /// The rigid body owning this collider
    /// </summary>
    public RigidBodyComponent RigidBody { get; private set; }

    /// <summary>
    /// The underlying fixture of the collider
    /// </summary>
    public abstract Fixture Fixture { get; }

    bool isSensor;
    public bool Sensor
    {
        get => Fixture?.IsSensor ?? isSensor;
        set
        {
            if (Fixture != null)
            {
                Fixture.IsSensor = value;
            }
            isSensor = value;
        }
    }

    private float restitution;
    /// <summary>
    /// Gets or sets the coefficient of restitution. This will not affect existing contacts.
    /// </summary>
    public float Restitution
    {
        get => Fixture?.Restitution ?? restitution;
        set
        {
            if (Fixture != null)
            {
                Fixture.Restitution = value;
            }
            restitution = value;
        }
    }

    private float friction;
    /// <summary>
    /// Gets or sets the coefficient of friction between the collider and other objects. This will not affect existing contacts.
    /// </summary>
    public float Friction
    {
        get => Fixture?.Friction ?? friction;
        set
        {
            if (Fixture != null)
            {
                Fixture.Friction = value;
            }
            friction = value;
        }
    }

    private float density = 1.0f;
    /// <summary>
    /// Gets or sets density. Changing density will recalculate the shape properties.
    /// </summary>
    public float Density
    {
        get { return density; }
        set
        {
            density = value;
            if (Fixture != null)
            {
                Fixture.Shape.Density = density;
                Fixture.Body.ResetMassData();
            }
        }
    }

    Category collisionCategories = Category.Cat1;
    /// <summary>
    /// The collision category this collider belongs to. (Default is Category.Cat1)
    /// </summary>
    public Category CollisionCategories
    {
        get => Fixture?.CollisionCategories ?? collisionCategories;
        set
        {
            if (Fixture != null) Fixture.CollisionCategories = value;
            collisionCategories = value;
        }
    }

    Category collidesWith = Category.All;
    /// <summary>
    /// The collision category this collider would accept for collision. (Default is Category.All)
    /// </summary>
    public Category CollidesWith
    {
        get => Fixture?.CollidesWith ?? collidesWith;
        set
        {
            if (Fixture != null) Fixture.CollidesWith = value;
            collidesWith = value;
        }
    }

    bool collisionEnabled = false;
    bool eventAttached = false;

    public override void OnComponentAdded(BaseComponent component)
    {
        base.OnComponentAdded(component);
        if (component is TransformComponent transform && Transform == null)
            Transform = transform;
        if (component is RigidBodyComponent rigidBody && RigidBody == null)
            RigidBody = rigidBody;
    }

    /// <summary>
    /// Enables collision callbacks for the fixture.
    /// </summary>
    public void EnableCollisionEvent()
    {
        collisionEnabled = true;
    }

    protected virtual bool Collision(Fixture firstBody, Fixture secondBody, Contact contact)
    {
        EventManager.Invoke<CollisionEvent>(this, new CollisionEvent
        {
            RigidBody = RigidBody,
            Self = firstBody,
            Other = secondBody,
            Contact = contact
        });

        return true;
    }

    /// <summary>
    /// Creates the fixture
    /// </summary>
    /// <param name="body">The body owning this fixture</param>
    protected abstract void CreateFixture(Body body);

    public override void OnFinished()
    {
        if (Fixture != null && RigidBody?.Body != null)
        {
            if (collisionEnabled)
                Fixture.OnCollision -= Collision;
            if (!Owner.Destroyed)
                RigidBody.Body.Remove(Fixture);
        }
        base.OnFinished();
    }

    protected virtual void FixtureRecreated(Fixture oldFixture, Fixture newFixture)
    {
        if (collisionEnabled)
        {
            oldFixture.OnCollision -= Collision;
            newFixture.OnCollision += Collision;
        }
        newFixture.IsSensor = oldFixture.IsSensor;
        newFixture.Restitution = oldFixture.Restitution;
        newFixture.Friction = oldFixture.Friction;
        Fixture.CollidesWith = oldFixture.CollidesWith;
        Fixture.CollisionCategories = oldFixture.CollisionCategories;
    }

    public override void OnFixedUpdate()
    {
        if (RigidBody?.Body != null && Fixture == null)
        {
            CreateFixture(RigidBody.Body);
            Fixture.IsSensor = isSensor;
            Fixture.Restitution = restitution;
            Fixture.Friction = friction;
            Fixture.CollidesWith = collidesWith;
            Fixture.CollisionCategories = collisionCategories;
            Fixture.Tag = this;
        }
        if (Fixture != null && collisionEnabled && !eventAttached)
        {
            Fixture.OnCollision += Collision;
            eventAttached = true;
        }
        base.OnFixedUpdate();
    }
}