namespace Strawberry.EventSystem;
public readonly record struct SubscriptionToken
{
    internal readonly Guid Id;
    internal readonly Type EventType;
    internal readonly bool IsGlobal;
    internal readonly WeakReference? Target; // for instance events

    internal SubscriptionToken(Type eventType, bool isGlobal, object? target = null)
    {
        Id = Guid.NewGuid();
        EventType = eventType;
        IsGlobal = isGlobal;
        Target = target != null ? new WeakReference(target) : null;
    }
}