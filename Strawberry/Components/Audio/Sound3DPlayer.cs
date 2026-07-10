using Strawberry.Core;
using Strawberry.Math;
using Strawberry.Sound;

namespace Strawberry.Components.Audio;

/// <summary>
/// Used to play a sound in 3d space. The sound will be played at Entity's Transform.Position
/// </summary>
public class Sound3DPlayer : BaseComponent
{
    private TransformComponent transform = null;

    private List<WeakReference<Voice3D>> activeVoices = new List<WeakReference<Voice3D>>();

    /// <summary>
    /// The sound buffer to play
    /// </summary>
    public SoundBuffer SoundBuffer { get; set; }

    /// <summary>
    /// Gets or sets the volume by which the sound will be played (Default is 1)
    /// Can be changed using the Voice3D object returned by Play()
    /// </summary>
    public float Volume { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets the frequency ratio (pitch) by which the sound will be played (Default is 1)
    /// Can be changed using the Voice3D object returned by Play()
    /// </summary>
    public float Frequency { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets whether the position of all the active voices will be updated to follow the Transform's position.
    /// </summary>
    public bool Follow { get; set; }

    public override void OnComponentAdded(BaseComponent component)
    {
        base.OnComponentAdded(component);

        if (component is TransformComponent tc)
            transform = tc;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Follow && transform != null)
        {
            var v3 = new Vector3(transform.Position, 0f);
            for (int i = activeVoices.Count - 1; i >= 0; i--)
            {
                if (activeVoices[i].TryGetTarget(out Voice3D voice))
                {
                    voice.Position = v3;
                }
                else
                {
                    activeVoices.RemoveAt(i);
                }
            }
        }
        else
        {
            for (int i = activeVoices.Count - 1; i >= 0; i--)
            {
                if (!activeVoices[i].TryGetTarget(out _))
                {
                    activeVoices.RemoveAt(i);
                }
            }
        }
    }

    /// <summary>
    /// Plays the sound buffer
    /// </summary>
    /// <param name="priority">The priority of the sound. If the available sources of the sound manager is full, the sound with the lowest priority will be replaced</param>
    /// <returns>If the sound get's to the playing list, the voice object to control the options, if the sound manager is full, null</returns>
    public Voice3D Play(int priority = 0)
    {
        if (transform == null)
            return null;
        var v = SoundBuffer?.Play(transform.Position, Frequency, false, priority);
        if (v != null)
        {
            v.Volume = Volume;
            activeVoices.Add(new WeakReference<Voice3D>(v));
        }
        return v;
    }
    /// <summary>
    /// Stops all the sounds currently played by this component
    /// </summary>
    public void Stop()
    {
        for (int i = 0; i < activeVoices.Count; i++)
        {
            if (activeVoices[i].TryGetTarget(out Voice3D voice))
            {
                voice.Stop();
            }
        }

        activeVoices.Clear();
    }

    /// <summary>
    /// Whether the any sound that is played by this component is currently playing.
    /// </summary>
    /// <returns>True if at least one voice is still playing. otherwise false</returns>
    public bool IsPlaying()
    {
        bool isPlaying = false;

        for (int i = activeVoices.Count - 1; i >= 0; i--)
        {
            if (activeVoices[i].TryGetTarget(out Voice3D voice))
            {
                if (voice.IsPlaying())
                {
                    isPlaying = true;
                }
            }
            else
            {
                activeVoices.RemoveAt(i);
            }
        }

        return isPlaying;
    }
}