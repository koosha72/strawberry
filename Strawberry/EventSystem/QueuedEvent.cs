namespace Strawberry.EventSystem;

internal interface IQueuedEvent
{
    void Invoke();
    int Priority { get; }
}

internal class QueuedEvent<T> : IQueuedEvent where T : IStrawberryEvent
{
    public WeakAction<T> Callback { get; }
    public T Args { get; }
    public int Priority { get; }

    public QueuedEvent(WeakAction<T> callback, T args, int priority)
    {
        Callback = callback;
        Args = args;
        Priority = priority;
    }

    public void Invoke() => Callback.Invoke(Args);
}