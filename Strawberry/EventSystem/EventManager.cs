using System.Collections.Concurrent;
using System.Reflection;

namespace Strawberry.EventSystem;

public static class EventManager
{
    private static Dictionary<Type, List<StrawberryEventObject>> globalEvents = new();
    private static Dictionary<Type, Dictionary<WeakReference, List<StrawberryEventObject>>> instanceEvents = new();
    private static Dictionary<EventCallTime, PriorityQueue<IQueuedEvent, int>> callbackQueue = new();
    private static readonly ConcurrentDictionary<(Type target, Type arg), ConstructorInfo> _weakActionCtors = new();

    public static SubscriptionToken Subscribe<T>(Action<T> callback, int priority = 0) where T : IStrawberryEvent
    {
        var eventType = typeof(T);
        var token = new SubscriptionToken(eventType, isGlobal: true);
        if (!globalEvents.ContainsKey(eventType))
            globalEvents.Add(eventType, new List<StrawberryEventObject>());

        globalEvents[eventType].Add(new()
        {
            Callback = CreateWeakAction(callback),
            Priority = priority,
            Token = token
        });

        return token;
    }

    public static SubscriptionToken Subscribe<T>(object sender, Action<T> callback, int priority = 0) where T : IStrawberryEvent
    {
        var eventType = typeof(T);
        var token = new SubscriptionToken(eventType, isGlobal: false, sender);
        if (!instanceEvents.ContainsKey(eventType))
            instanceEvents.Add(eventType, new Dictionary<WeakReference, List<StrawberryEventObject>>());

        List<WeakReference> removeList = new();
        bool added = false;
        foreach (var (reference, callbacks) in instanceEvents[eventType])
        {
            if (!reference.IsAlive)
            {
                removeList.Add(reference);
                continue;
            }
            if (reference.Target == sender)
            {
                instanceEvents[eventType][reference].Add(new()
                {
                    Callback = CreateWeakAction(callback),
                    Priority = priority,
                    Token = token
                });
                added = true;
                break;
            }
        }

        foreach (var r in removeList)
        {
            instanceEvents[eventType].Remove(r);
        }

        if (!added)
        {
            var lst = new List<StrawberryEventObject>();
            lst.Add(new()
            {
                Callback = CreateWeakAction(callback),
                Priority = priority,
                Token = token
            });
            instanceEvents[eventType].Add(new WeakReference(sender), lst);
        }

        return token;
    }

    public static void Unsubscribe(SubscriptionToken token)
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

    public static void Invoke<T>(object sender, T args) where T : IStrawberryEvent
    {
        if (args == null) return;

        var eventCallTime = args.EventCallTime;  // or T's default if not set

        // Ensure queue exists
        if (!callbackQueue.ContainsKey(eventCallTime))
            callbackQueue[eventCallTime] = new PriorityQueue<IQueuedEvent, int>();

        var queue = callbackQueue[eventCallTime];

        // Global events
        if (globalEvents.TryGetValue(typeof(T), out var globalList))
        {
            foreach (var obj in globalList)
            {
                queue.Enqueue(
                    new QueuedEvent<T>(obj.Callback, args, obj.Priority),
                    obj.Priority
                );
            }
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
                    foreach (var obj in callbacks)
                    {
                        queue.Enqueue(
                            new QueuedEvent<T>(obj.Callback, args, obj.Priority),
                            obj.Priority
                        );
                    }
                }
            }

            foreach (var r in removeList)
                instanceDict.Remove(r);
        }
    }

    // === EXECUTE ===
    public static void Execute(EventCallTime eventCallTime)
    {
        if (!callbackQueue.TryGetValue(eventCallTime, out var queue))
            return;

        while (queue.TryDequeue(out var queuedEvent, out _))
        {
            queuedEvent.Invoke();
        }
    }

    private static IWeakAction CreateWeakAction<T>(Action<T> action) where T : IStrawberryEvent
    {
        if (action.Target == null)
            throw new Exception("Static methods are not supported");

        var senderType = action.Target.GetType();
        var key = (senderType, typeof(T));

        // Get or create the cached ConstructorInfo
        var ctor = _weakActionCtors.GetOrAdd(key, static k =>
        {
            // 1. Construct the generic type (only happens once per unique target/arg pair)
            var weakActionType = typeof(WeakAction<,>).MakeGenericType(k.target, k.arg);

            // 2. Find the constructor that takes Action<T>
            var actionType = typeof(Action<>).MakeGenericType(k.arg);
            return weakActionType.GetConstructor(new[] { actionType })
                ?? throw new InvalidOperationException($"Constructor not found for {weakActionType}");
        });

        // 3. Invoke the cached constructor directly
        return (IWeakAction)ctor.Invoke(new object[] { action })!;
    }
}