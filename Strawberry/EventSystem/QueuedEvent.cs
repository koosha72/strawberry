/*
 * Strawberry Game Engine
 * File: QueuedEvent.cs
 * Author: Koosha Aabedini Nassab
 *
 * Internal queued event types used by the event manager.
 */

namespace Strawberry.EventSystem;

internal interface IQueuedEvent
{
    void Invoke();
    int Priority { get; }
}

internal class QueuedEvent<T> : IQueuedEvent where T : IStrawberryEvent
{
    public IWeakAction Callback { get; }
    public T Args { get; }
    public int Priority { get; }

    public QueuedEvent(IWeakAction callback, T args, int priority)
    {
        Callback = callback;
        Args = args;
        Priority = priority;
    }

    public void Invoke() => Callback.Invoke(Args);
}