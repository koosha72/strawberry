/*
 * Strawberry Game Engine
 * File: IStrawberryEvent.cs
 * Author: Koosha Aabedini Nassab
 *
 * Defines the event call times and the base event interface
 */

namespace Strawberry.EventSystem;

/// <summary>
/// The main event on which the custom events are executed.
/// </summary>
public enum EventCallTime
{
    /// <summary>
    /// The custom event will fire after all components' OnBeginUpdate methods have been called.
    /// </summary>
    OnBeginUpdate,
    /// <summary>
    /// The custom event will fire after all components' OnUpdate methods have been called.
    /// </summary>
    OnUpdate,
    /// <summary>
    /// The custom event will fire after all components' OnEndUpdate methods have been called.
    /// </summary>
    OnEndUpdate,
    /// <summary>
    /// The custom event will fire after all components' OnFixedUpdate methods have been called. It is supposed to be used for physics calculations.
    /// </summary>
    OnFixedUpdate,
    /// <summary>
    /// The custom event will fire after all components' OnLateUpdate methods have been called.
    /// </summary>
    OnBeginRender,
    /// <summary>
    /// The custom event will fire after all components' OnRender methods have been called. It is supposed to be used for rendering and drawing to the screen.
    /// </summary>
    OnRender,
    /// <summary>
    /// The custom event will fire after all components' OnEndRender methods have been called.
    /// </summary>
    OnEndRender
}

/// <summary>
/// An event that can be subscribed to.
/// </summary>
public interface IStrawberryEvent
{
    /// <summary>
    /// The time at which the event should be called.
    /// </summary>
    EventCallTime EventCallTime => EventCallTime.OnUpdate;
}


internal class StrawberryEventObject
{
    public IWeakAction Callback { get; set; }
    public int Priority { get; set; }
    public SubscriptionToken Token { get; set; }
}