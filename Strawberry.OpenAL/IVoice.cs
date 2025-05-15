using System;

namespace Strawberry.OpenAL;

public interface IVoice : IDisposable
{
    int SourceInd { get; set; }
    Strawberry.Sound.SoundBuffer Buffer { get; }

    void SetBuffer(SoundBuffer soundBuffer);

    bool IsPlaying();
    bool IsPaused();
}
