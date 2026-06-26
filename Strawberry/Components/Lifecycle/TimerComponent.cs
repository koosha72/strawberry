using Strawberry.Core;
using Strawberry.EventSystem;

namespace Strawberry.Components.Lifecycle;

public struct TimerEndedEvent : IStrawberryEvent
{
    public TimerComponent Timer;
}


public class TimerComponent : BaseComponent
{
    bool finished;
    private float time;

    /// <summary>
    /// Gets or sets the time of the timer. (Seconds) When this value reaches 0 the timer will end and the TimerEndedEvent will be raised.
    /// </summary>
    public float Time
    {
        get { return time; }
        set
        {
            finished = false;
            time = value;
        }
    }

    /// <summary>
    /// Gets or sets the initial value of the timer. (Seconds)
    /// If Loop is true the timer will reset to this value when it ends.
    /// Sets the time of the timer to this value when you change the value of StartTime.
    /// </summary>
    private float startTime;
    public float StartTime
    {
        get { return startTime; }
        set
        {
            startTime = value;
            Time = value;
        }
    }

    /// <summary>
    /// Gets or sets whether the timer will loop when it ends. If true the time of the timer will restart. (Default is false)
    /// </summary>
    public bool Loop { get; set; }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (finished) return;
        if (time > 0f)
            time -= FrameInfo.Information.DeltaTime;
        else
        {
            finished = true;
            EventManager.Invoke(this, new TimerEndedEvent() { Timer = this });

            if (Loop) Time = MathF.Max(startTime, 0.001f);
        }
    }
}