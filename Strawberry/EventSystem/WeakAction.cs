namespace Strawberry.EventSystem;

public interface IWeakAction
{
}

public class WeakAction<T> : IWeakAction
{
    private readonly WeakReference<object?> _weakTarget;
    private readonly Action<T> _invokeAction;   // This is the final callable action

    public WeakAction(Action<T> originalAction)
    {
        if (originalAction == null)
            throw new ArgumentNullException(nameof(originalAction));

        _weakTarget = new WeakReference<object?>(originalAction.Target);

        if (originalAction.Target == null)
        {
            // Static method - just use directly
            _invokeAction = originalAction;
        }
        else
        {
            // Instance method - create a wrapper that does the weak check
            var method = originalAction.Method;
            var targetType = originalAction.Target.GetType();

            // Create open delegate safely
            var openDelegate = Delegate.CreateDelegate(
                typeof(Action<object?, T>),
                null,
                method,
                false);   // throwOnBindFailure = false

            if (openDelegate is Action<object?, T> openAction)
            {
                _invokeAction = arg =>
                {
                    if (_weakTarget.TryGetTarget(out var target))
                    {
                        openAction(target, arg);
                    }
                };
            }
            else
            {
                // Fallback (should rarely happen)
                _invokeAction = arg =>
                {
                    if (_weakTarget.TryGetTarget(out var target))
                    {
                        method.Invoke(target, new object?[] { arg });
                    }
                };
            }
        }
    }

    public bool IsAlive => _weakTarget.TryGetTarget(out _);

    public void Invoke(T arg)
    {
        _invokeAction(arg);
    }
}