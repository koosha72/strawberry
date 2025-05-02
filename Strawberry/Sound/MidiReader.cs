using System;

namespace Strawberry.Sound;

using System;
using System.IO;
using System.Text;
using MeltySynth;

public class MidiReader : ISoundReader
{
    public Stream Stream { get; private set; }
    public int BitsPerSample { get; } = 16;
    public int SampleRate { get; private set; } = 44100;
    public int Channels { get; } = 2; // Stereo output
    public int DataSize { get; private set; }

    private readonly MidiFile midiFile;
    private readonly Synthesizer synthesizer;
    private MemoryStream pcmStream;
    private bool isDisposed;

    public MidiReader(Stream stream, int sampleRate = 44100)
    {
        Stream = stream ?? throw new ArgumentNullException(nameof(stream));
        SampleRate = sampleRate;

        // Initialize MIDI synthesis
        midiFile = new MidiFile(stream);
        Stream sf2stream = typeof(MidiReader).Assembly.GetManifestResourceStream("Strawberry.Sound.8MBGMSFX.SF2");
        var soundFont = new SoundFont(sf2stream); // The sound font file
        synthesizer = new Synthesizer(soundFont, sampleRate);

        pcmStream = new MemoryStream();
        RenderToStream(pcmStream);
        DataSize = (int)pcmStream.Length;
        pcmStream.Position = 0;
    }

    private void RenderToStream(Stream outputStream)
    {
        // Create a sequencer to handle MIDI timing
        var sequencer = new MidiFileSequencer(synthesizer);
        sequencer.Play(midiFile, false); // false = don't loop

        // Calculate total samples needed (duration in seconds * sample rate)
        double duration = midiFile.Length.TotalSeconds;
        int totalSamples = (int)(duration * SampleRate);
        int blockSize = synthesizer.BlockSize;

        // Buffer for rendered audio
        float[] leftBuffer = new float[blockSize];
        float[] rightBuffer = new float[blockSize];
        byte[] pcmBuffer = new byte[blockSize * 4]; // 16-bit stereo = 4 bytes per sample pair

        // Render the entire MIDI to PCM
        int samplesRemaining = totalSamples;
        while (samplesRemaining > 0)
        {
            int samplesToRender = Math.Min(blockSize, samplesRemaining);

            // Render stereo audio
            sequencer.Render(leftBuffer, rightBuffer);

            // Convert float samples to 16-bit PCM
            for (int i = 0; i < samplesToRender; i++)
            {
                // Left channel (16-bit)
                short leftSample = (short)(leftBuffer[i] * short.MaxValue);
                pcmBuffer[i * 4] = (byte)(leftSample & 0xFF);
                pcmBuffer[i * 4 + 1] = (byte)(leftSample >> 8);

                // Right channel (16-bit)
                short rightSample = (short)(rightBuffer[i] * short.MaxValue);
                pcmBuffer[i * 4 + 2] = (byte)(rightSample & 0xFF);
                pcmBuffer[i * 4 + 3] = (byte)(rightSample >> 8);
            }

            outputStream.Write(pcmBuffer, 0, samplesToRender * 4);
            samplesRemaining -= samplesToRender;
        }
    }

    public int Read(byte[] buffer, int offset, int count)
    {
        if (isDisposed) throw new ObjectDisposedException(nameof(MidiReader));

        return pcmStream.Read(buffer, offset, count);
    }

    public byte[] ReadAll()
    {
        if (isDisposed) throw new ObjectDisposedException(nameof(MidiReader));

        return pcmStream.ToArray();
    }

    public void Seek(long offset)
    {
        if (isDisposed) throw new ObjectDisposedException(nameof(MidiReader));

        pcmStream.Seek(offset, SeekOrigin.Begin);
    }

    public void Dispose()
    {
        if (!isDisposed)
        {
            isDisposed = true;
            pcmStream?.Dispose();
            Stream?.Dispose();
            //synthesizer?.Dispose();
        }
    }
}
