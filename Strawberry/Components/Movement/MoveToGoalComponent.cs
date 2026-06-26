using Strawberry.Core;
using Strawberry.EventSystem;
using Strawberry.Math;

namespace Strawberry.Components.Movement;

/// <summary>
/// The event occurs when the MoveToGoal component arrives at its destination.
/// </summary>
public struct MoveToGoalArrivedEvent : IStrawberryEvent
{
    /// <summary>
    /// The MoveToGoal component that arrived at its destination (The event caller).
    /// </summary>
    public MoveToGoalComponent MoveToGoal;
}

/// <summary>
/// Moves the entity towards a goal position.
/// </summary>
public class MoveToGoalComponent : BaseComponent
{
    public TransformComponent Transform { get; private set; }

    bool arrived = false;
    Vector2 goal;

    /// <summary>
    /// Gets or sets target position to move towards.
    /// </summary>
    public Vector2 Goal
    {
        get
        {
            return goal;
        }
        set
        {
            goal = value;
            arrived = false;
        }
    }

    /// <summary>
    /// Gets or sets radius at which the entity will stop moving.
    /// </summary>
    public float ArrivalRadius { get; set; }

    /// <summary>
    /// Gets or sets the speed of the movement. (Pixels per second)
    /// </summary>
    public float Speed { get; set; }

    /// <summary>
    /// Gets or sets the heading of the owner. If true the owner will rotate to face its direction of movement. (Default: false)
    /// </summary>
    public bool HeadToGoal { get; set; }

    public override void OnComponentAdded(BaseComponent component)
    {
        base.OnComponentAdded(component);
        if (component is TransformComponent transformComponent && Transform == null)
            Transform = transformComponent;
    }

    public override void OnUpdate()
    {
        if (Transform == null || Speed <= 0f)
            return;
        var direction = Goal - Transform.Position;
        if (direction.LengthSquared() > ArrivalRadius * ArrivalRadius)
        {
            var remaining = direction.Length;
            var directionNormalized = Vector2.Normalize(direction);
            var distance = MathF.Min(Speed * FrameInfo.Information.DeltaTime, remaining);
            Transform.Position += directionNormalized * distance;
            if (HeadToGoal)
            {
                Transform.Angle = (float)direction.Direction;
            }
        }
        else
        {
            if (!arrived)
            {
                arrived = true;
                EventManager.Invoke<MoveToGoalArrivedEvent>(this, new MoveToGoalArrivedEvent
                {
                    MoveToGoal = this
                });
            }
        }
    }
}