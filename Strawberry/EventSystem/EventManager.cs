/*
 * Strawberry Game Engine
 * File: EventManager.cs
 * Author: Koosha Aabedini Nassab
 *
 * Event manager responsible for subscribing, queuing and executing
 * game events.
 */

using System.Collections.Concurrent;
using System.Reflection;

namespace Strawberry.EventSystem;

/// <summary>
/// This is a static class that manages events in the game.
/// </summary>
public static class EventManager
{
    private static Dictionary<Type, List<StrawberryEventObject>> globalEvents = new();
    private static Dictionary<Type, Dictionary<WeakReference, List<StrawberryEventObject>>> instanceEvents = new();
    private static Dictionary<EventCallTime, PriorityQueue<IQueuedEvent, int>> callbackQueue = new();
    private static readonly ConcurrentDictionary<(Type target, Type arg), ConstructorInfo> _weakActionCtors = new();

    /// <summary>
    /// If false the event manager will throw exceptions when an exception occurs in a handler. If true, the exception will be swallowed and logged.
    /// Also you can use OnHandlerException to handle exceptions yourself (if OnHandlerException is set no log will be written). (Default is true)
    /// </summary>
    public static bool SwallowExceptions { get; set; } = true;

    /// <summary>
    /// If an exception occurs in a callback, this will be called with the unhandled Exception object as the parameter.
    /// </summary>
    public static Action<Exception> OnHandlerException { get; set; }

    private static readonly object _lock = new();

    /// <summary>
    /// Subscribe to an event globally.
    /// </summary>
    /// <typeparam name="T">Event type</typeparam>
    /// <param name="callback">The callback to be called when the event is fired.</param>
    /// <param name="priority">The priority of the callback. Higher priority means it will be called first. Default is 0.</param>
    /// <returns>A token used to unsubscribe from the event.</returns>
    public static SubscriptionToken Subscribe<T>(Action<T> callback, int priority = 0) where T : IStrawberryEvent
    {
        // Create the weak action OUTSIDE the lock to minimize lock duration
        var weakAction = CreateWeakAction(callback);
        var eventType = typeof(T);
        var token = new SubscriptionToken(eventType, isGlobal: true);

        lock (_lock)
        {
            if (!globalEvents.TryGetValue(eventType, out var list))
            {
                list = new List<StrawberryEventObject>();
                globalEvents[eventType] = list;
            }

            list.Add(new()
            {
                Callback = weakAction,
                Priority = priority,
                Token = token
            });
        }

        return token;
    }
    /// <summary>
    /// Subscribes to an event for a specific instance of the object.
    /// </summary>
    /// <typeparam name="T">Event type</typeparam>
    /// <param name="sender">The object to subscribe to (source)</param>
    /// <param name="callback">The callback to invoke when the event is raised</param>
    /// <param name="priority">The priority of the callback.  Higher priority means it will be called first. Default is 0.</param>
    /// <returns>A token used to unsubscribe from the event.</returns>
    public static SubscriptionToken Subscribe<T>(object sender, Action<T> callback, int priority = 0) where T : IStrawberryEvent
    {
        // Create the weak action OUTSIDE the lock
        var weakAction = CreateWeakAction(callback);
        var eventType = typeof(T);
        var token = new SubscriptionToken(eventType, isGlobal: false, sender);

        lock (_lock)
        {
            if (!instanceEvents.TryGetValue(eventType, out var dict))
            {
                dict = new Dictionary<WeakReference, List<StrawberryEventObject>>();
                instanceEvents[eventType] = dict;
            }

            List<WeakReference> removeList = new();
            bool added = false;

            foreach (var (reference, callbacks) in dict)
            {
                if (!reference.IsAlive)
                {
                    removeList.Add(reference);
                    continue;
                }
                if (reference.Target == sender)
                {
                    callbacks.Add(new()
                    {
                        Callback = weakAction,
                        Priority = priority,
                        Token = token
                    });
                    added = true;
                    break;
                }
            }

            foreach (var r in removeList)
            {
                dict.Remove(r);
            }

            if (!added)
            {
                var lst = new List<StrawberryEventObject>
                {
                    new()
                    {
                        Callback = weakAction,
                        Priority = priority,
                        Token = token
                    }
                };
                dict.Add(new WeakReference(sender), lst);
            }
        }

        return token;
    }
    /// <summary>
    /// Unsubscribe from an event by token
    /// </summary>
    /// <param name="token">The token returned from the Subscribe method</param>
    public static void Unsubscribe(SubscriptionToken token)
    {
        lock (_lock)
        {
            var eventType = token.EventType;

            if (token.IsGlobal)
            {
                if (globalEvents.TryGetValue(eventType, out var list))
                {
                    list.RemoveAll(e => e.Token.Id == token.Id);
                    if (list.Count == 0)
                        globalEvents.Remove(eventType);
                }
            }
            else
            {
                if (instanceEvents.TryGetValue(eventType, out var dict))
                {
                    List<WeakReference> toRemove = new();

                    foreach (var (weakRef, callbacks) in dict)
                    {
                        if (!weakRef.IsAlive)
                        {
                            toRemove.Add(weakRef);
                            continue;
                        }

                        callbacks.RemoveAll(e => e.Token.Id == token.Id);

                        if (callbacks.Count == 0)
                            toRemove.Add(weakRef);
                    }

                    foreach (var r in toRemove)
                        dict.Remove(r);

                    if (dict.Count == 0)
                        instanceEvents.Remove(eventType);
                }
            }
        }
    }

    /// <summary>
    /// Invokes an event. The method is not invoked instantly but rather queued for the next frame.
    /// The invoke time is determined by the <see cref="IStrawberryEvent.EventCallTime"/> property of the event object.
    /// </summary>
    /// <typeparam name="T">Event type</typeparam>
    /// <param name="sender">The object invoking the event</param>
    /// <param name="args">The event object</param>
    public static void Invoke<T>(object sender, T args) where T : IStrawberryEvent
    {
        if (args == null) return;

        var eventCallTime = args.EventCallTime;

        lock (_lock)
        {
            if (!callbackQueue.TryGetValue(eventCallTime, out var queue))
            {
                queue = new PriorityQueue<IQueuedEvent, int>();
                callbackQueue[eventCallTime] = queue;
            }

            // Global events
            if (globalEvents.TryGetValue(typeof(T), out var globalList))
            {
                globalList.RemoveAll(obj => !obj.Callback.IsAlive);

                foreach (var obj in globalList)
                {
                    queue.Enqueue(
                        new QueuedEvent<T>(obj.Callback, args, obj.Priority),
                        obj.Priority
                    );
                }

                if (globalList.Count == 0)
                    globalEvents.Remove(typeof(T));
            }

            // Instance events
            if (instanceEvents.TryGetValue(typeof(T), out var instanceDict))
            {
                List<WeakReference> removeList = new();

                foreach (var (reference, callbacks) in instanceDict)
                {
                    if (!reference.IsAlive)
                    {
                        removeList.Add(reference);
                        continue;
                    }

                    if (reference.Target == sender)
                    {
                        callbacks.RemoveAll(obj => !obj.Callback.IsAlive);

                        foreach (var obj in callbacks)
                        {
                            queue.Enqueue(
                                new QueuedEvent<T>(obj.Callback, args, obj.Priority),
                                obj.Priority
                            );
                        }

                        if (callbacks.Count == 0)
                            removeList.Add(reference);
                    }
                }

                foreach (var r in removeList)
                    instanceDict.Remove(r);

                if (instanceDict.Count == 0)
                    instanceEvents.Remove(typeof(T));
            }
        }
    }

    /// <summary>
    /// This is called by Game class
    /// </summary>
    /// <param name="eventCallTime">The event group to execute</param>
    public static void Execute(EventCallTime eventCallTime)
    {
        List<IQueuedEvent> eventsToExecute;

        lock (_lock)
        {
            if (!callbackQueue.TryGetValue(eventCallTime, out var queue))
                return;

            // Dequeue everything into a local list quickly, so we release the lock
            // before actually invoking the events. This prevents deadlocks if an
            // event handler calls Subscribe/Unsubscribe/Invoke.
            eventsToExecute = new List<IQueuedEvent>(queue.Count);
            while (queue.TryDequeue(out var queuedEvent, out _))
            {
                eventsToExecute.Add(queuedEvent);
            }
        }

        foreach (var queuedEvent in eventsToExecute)
        {
            try
            {
                queuedEvent.Invoke();
            }
            catch (Exception ex)
            {
                if (SwallowExceptions)
                {
                    var handler = OnHandlerException;
                    if (handler != null)
                        handler(ex);
                    else
                        Console.Error.WriteLine($"[EventManager] Handler threw: {ex}");
                }

                if (!SwallowExceptions)
                    throw;
            }
        }
    }

    private static IWeakAction CreateWeakAction<T>(Action<T> action) where T : IStrawberryEvent
    {
        if (action.Target == null)
            throw new Exception("Static methods are not supported");

        var senderType = action.Target.GetType();
        var key = (senderType, typeof(T));

        var ctor = _weakActionCtors.GetOrAdd(key, static k =>
        {
            var weakActionType = typeof(WeakAction<,>).MakeGenericType(k.target, k.arg);
            var actionType = typeof(Action<>).MakeGenericType(k.arg);
            return weakActionType.GetConstructor(new[] { actionType })
                ?? throw new InvalidOperationException($"Constructor not found for {weakActionType}");
        });

        return (IWeakAction)ctor.Invoke(new object[] { action })!;
    }
}