/*
 * Strawberry Game Engine
 * File: WeakAction.cs
 * Author: Koosha Aabedini Nassab
 *
 * Implements weak-referenced action wrappers to avoid preventing
 * target objects from being garbage collected.
 */

using System.Diagnostics;
using System.Reflection;

namespace Strawberry.EventSystem;

internal interface IWeakAction
{
    public void Invoke(object arg);
    public bool IsAlive { get; }
}

internal class WeakAction<TTarget, T> : IWeakAction
{
    private readonly WeakReference<object?> _weakTarget;
    private readonly Action<T> _invokeAction;

    private readonly MethodInfo? _method;
    private readonly object?[]? _argBuffer;

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

            // Create open delegate
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
                    }
                };
            }
            else
            {
                // Fallback to reflection
                _method = method;
                _argBuffer = new object?[1];

                _invokeAction = arg =>
                {
                    if (_weakTarget.TryGetTarget(out var target))
                    {
                        _argBuffer[0] = arg;
                        _method.Invoke(target, _argBuffer);
                        _argBuffer[0] = null;
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