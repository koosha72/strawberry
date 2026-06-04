using System;

namespace Strawberry.OpenAL;

public interface IVoice : IDisposable
{
    int SourceInd { get; set; }
    Strawberry.Sound.SoundBuffer Buffer { get; }

    public void MarkRecycled();


    void SetBuffer(SoundBuffer soundBuffer);

    public void Pause();
    public void Resume();
    public void Stop();


    bool IsPlaying();
    bool IsPaused();
}
