using System;

namespace Strawberry.Sound;


public interface ISoundReader : IDisposable
{
    public Stream Stream { get; }

    public int BitsPerSample { get; }
    public int SampleRate { get; }
    public int Channels { get; }

    public int DataSize { get; }

    public int Read(byte[] buffer, int offset, int count);

    public void Seek(long offset);

    public byte[] ReadAll();
}
