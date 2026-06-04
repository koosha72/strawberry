namespace Strawberry.EventSystem;

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

public interface IStrawberryEvent
{
    EventCallTime EventCallTime => EventCallTime.OnUpdate;
}

internal class StrawberryEventObject
{
    public IWeakAction Callback { get; set; }
    public int Priority { get; set; }
    public SubscriptionToken Token { get; set; }
}