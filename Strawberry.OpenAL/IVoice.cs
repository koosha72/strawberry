using System;

namespace Strawberry.OpenAL;

public interface IVoice : IDisposable
{
    int SourceInd { get; set; }
    int Priority { get; }

    public bool IsVirtual { get; }

    Strawberry.Sound.SoundBuffer Buffer { get; }

    public void Pause();
    public void Resume();
    public void Stop();

    void ApplyCachedState();


    bool IsPlaying();
    bool IsPaused();
}
