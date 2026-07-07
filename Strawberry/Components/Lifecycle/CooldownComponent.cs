using Strawberry.Core;
using Strawberry.EventSystem;

namespace Strawberry.Components.Lifecycle;

/// <summary>
/// This event occurs every time the cooldown is finished for an action.
/// </summary>
public struct CooldownFinishedEvent : IStrawberryEvent
{
    /// <summary>
    /// The cooldown component that ended its cooldown (The event caller).
    /// </summary>
    public CooldownComponent Cooldown;

    /// <summary>
    /// Gets the key of the action
    /// </summary>
    public string Key { get; init; }
}

/// <summary>
/// Implements a mechanism to manage cooldowns for actions, such as shooting a gun or jumping.
/// </summary>
public class CooldownComponent : BaseComponent
{
    Dictionary<string, float> cooldowns = new();

    /// <summary>
    /// Starts the cooldown process for the key provided, if the key exists, it will reset the cooldown timer to the given time.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="time"></param>
    public void Start(string key, float time)
    {
        cooldowns[key] = time;
    }

    /// <summary>
    /// Returns whether the provided action (key) is ready.
    /// </summary>
    /// <param name="key">The key of the action</param>
    /// <returns>True if the action is ready, false otherwise</returns>
    public bool IsReady(string key)
    {
        return !cooldowns.ContainsKey(key);
    }

    /// <summary>
    /// Gets the remaining time for a given action
    /// </summary>
    /// <param name="key">The key of the action</param>
    /// <returns>The remaining time for the action</returns>
    public float GetRemaining(string key)
    {
        cooldowns.TryGetValue(key, out float time);
        return time;
    }

    /// <summary>
    /// Cancels a given action, making it ready to be used again.
    /// </summary>
    /// <param name="key">The key of the action</param>
    public void Cancel(string key)
    {
        cooldowns.Remove(key);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        List<string> remove = new();
        foreach (var key in cooldowns.Keys)
        {
            cooldowns[key] -= FrameInfo.Information.DeltaTime;
            if (cooldowns[key] <= 0f)
                remove.Add(key);
        }

        foreach (var key in remove)
        {
            cooldowns.Remove(key);
            EventManager.Invoke(this, new CooldownFinishedEvent() { Key = key, Cooldown = this });
        }
    }
}