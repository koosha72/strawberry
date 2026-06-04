namespace Strawberry.EventSystem;

/// <summary>
/// The main event on which the custom events are executed.
/// </summary>
public enum EventCallTime
{
    OnBeginUpdate,
    OnUpdate,
    OnEndUpdate,
    OnFixedUpdate,
    OnBeginRender,
    OnRender,
    OnEndRender
}

/// <summary>
/// An event that can be subscribed to.
/// </summary>
public interface IStrawberryEvent
{
    /// <summary>
    /// The time at which the event should be called.
    /// </summary>
    EventCallTime EventCallTime => EventCallTime.OnUpdate;
}


internal class StrawberryEventObject
{
    public IWeakAction Callback { get; set; }
    public int Priority { get; set; }
    public SubscriptionToken Token { get; set; }
}