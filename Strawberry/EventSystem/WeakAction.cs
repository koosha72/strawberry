using System.Diagnostics;
using System.Reflection;

namespace Strawberry.EventSystem;

public interface IWeakAction
{
    public void Invoke(object arg);
    public bool IsAlive { get; }
}

public class WeakAction<TTarget, T> : IWeakAction
{
    private readonly WeakReference<object?> _weakTarget;
    private readonly Action<T> _invokeAction;   // This is the final callable action

    // Reflection fallback fields
    private readonly MethodInfo? _method;
    private readonly object?[]? _argBuffer;     // Cached array to prevent allocations!

    public WeakAction(Action<T> originalAction)
    {
        if (originalAction == null)
            throw new ArgumentNullException(nameof(originalAction));

        _weakTarget = new WeakReference<object?>(originalAction.Target);

        if (originalAction.Target == null)
        {
            // Static method - just use directly (Currently static methods are not supported)
            _invokeAction = originalAction;
        }
        else
        {
            // Instance method - create a wrapper that does the weak check
            var method = originalAction.Method;

            // Create open delegate safely
            var openDelegate = Delegate.CreateDelegate(
                typeof(Action<TTarget, T>),
                null,
                method,
                false);   // throwOnBindFailure = false

            if (openDelegate is Action<TTarget, T> openAction)
            {
                _invokeAction = arg =>
                {
                    if (_weakTarget.TryGetTarget(out var target))
                    {
                        openAction((TTarget)target, arg);
                        Console.WriteLine("Open Action Called!");
                    }
                };
            }
            else
            {
                // Fallback (should rarely happen)
                // Cache the MethodInfo and allocate the argument buffer ONCE
                _method = method;
                _argBuffer = new object?[1];

                _invokeAction = arg =>
                {
                    if (_weakTarget.TryGetTarget(out var target))
                    {
                        _argBuffer[0] = arg;                         // Reuse buffer
                        _method.Invoke(target, _argBuffer);          // Invoke
                        _argBuffer[0] = null;                        // Clear to allow GC of 'arg' if it's a reference type
                        Console.WriteLine("Reflection Called!");
                    }
                };
            }
        }
    }

    public bool IsAlive => _weakTarget.TryGetTarget(out _);

    public void Invoke(object arg)
    {
        _invokeAction((T)arg);
    }
}