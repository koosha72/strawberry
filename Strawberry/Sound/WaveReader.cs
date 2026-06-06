/*
 * Strawberry Game Engine
 * File: WaveReader.cs
 * Author: Koosha Aabedini Nassab
 *
 * Wave file reader implementation for PCM WAV audio.
 */

using System.Text;

namespace Strawberry.Sound;
/// <summary>
/// A wave file reader used to read and stream .wav files.
/// </summary>
public class WaveReader : ISoundReader
{
    public Stream Stream { get; private set; }

    public int BitsPerSample { get; private set; }

    public int SampleRate { get; private set; }

    public int Channels { get; private set; }

    public int DataSize { get; private set; }

    int headerSize = 0;

    private bool isDisposed;

    public WaveReader(Stream stream)
    {
        Stream = stream;

        if (stream == null)
            throw new ArgumentNullException("stream");

        ReadHeader();
    }

    public void Dispose()
    {
        if (!isDisposed)
        {
            isDisposed = true;
            Stream?.Dispose();
            Stream = null;
        }
    }

    public int Read(byte[] buffer, int offset, int count)
    {
        if (Stream == null)
            throw new ObjectDisposedException(nameof(Stream));

        return Stream.Read(buffer, offset, count);
    }

    public byte[] ReadAll()
    {
        if (Stream == null)
            return new byte[0];
        using (BinaryReader reader = new BinaryReader(Stream, new UTF8Encoding(), true))
        {
            return reader.ReadBytes(DataSize);
        }
    }

    public void Seek(long offset)
    {
        Stream?.Seek(headerSize + offset, SeekOrigin.Begin);
    }

    void ReadHeader()
    {
        if (Stream == null)
            return;
        using (BinaryReader reader = new BinaryReader(Stream, new UTF8Encoding(), true))
        {
            Stream.Seek(0, SeekOrigin.Begin);
            // RIFF header 
            string signature = new string(reader.ReadChars(4));
            if (signature != "RIFF")
                throw new NotSupportedException("Specified stream is not a wave file.");

            int riffChunckSize = reader.ReadInt32();


            string format = new string(reader.ReadChars(4));
            if (format != "WAVE")
                throw new NotSupportedException("Specified stream is not a wave file.");


            string formatSignature;
            string junk = new string(reader.ReadChars(4));
            if (junk == "JUNK")
            {
                int size = reader.ReadInt32();
                reader.ReadBytes(size);
                formatSignature = new string(reader.ReadChars(4));
                if (formatSignature == "bext")
                {
                    size = reader.ReadInt32();
                    reader.ReadBytes(size);
                    formatSignature = new string(reader.ReadChars(4));
                }
            }
            else
            {
                formatSignature = junk;
            }
            if (formatSignature == "bext")
            {
                int size = reader.ReadInt32();
                reader.ReadBytes(size);
                formatSignature = new string(reader.ReadChars(4));
            }

            if (formatSignature != "fmt ")
                throw new NotSupportedException("Specified wave file is not supported.");

            int formatChunkSize = reader.ReadInt32();
            int audioFormat = reader.ReadInt16();
            int numChannels = reader.ReadInt16();
            int sampleRate = reader.ReadInt32();
            int byteRate = reader.ReadInt32();
            int blockAlign = reader.ReadInt16();
            int bitsPerSample = reader.ReadInt16();
            reader.ReadBytes(formatChunkSize - 16);


            string dataSignature = new string(reader.ReadChars(4));
            while (dataSignature != "data")
            {
                reader.ReadBytes(reader.ReadInt32());
                dataSignature = new string(reader.ReadChars(4));
            }
            if (dataSignature != "data")
                throw new NotSupportedException("Specified wave file is not supported.");


            DataSize = reader.ReadInt32();


            Channels = numChannels;
            BitsPerSample = bitsPerSample;
            SampleRate = sampleRate;
            headerSize = (int)Stream.Position;
        }
    }
}
