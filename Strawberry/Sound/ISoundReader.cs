/*
 * Strawberry Game Engine
 * File: ISoundReader.cs
 * Author: Koosha Aabedini Nassab
 *
 * Sound reader interface for decoding and streaming audio data.
 */

namespace Strawberry.Sound;

/// <summary>
/// Defines an interface for reading and decoding sound data from a stream.
/// </summary>
public interface ISoundReader : IDisposable
{
    /// <summary>
    /// Gets the underlying stream that contains the sound data.
    /// </summary>
    public Stream Stream { get; }

    /// <summary>
    /// Gets the number of bits per sample (bit depth) of the audio data (e.g., 8, 16, 24).
    /// </summary>
    public int BitsPerSample { get; }

    /// <summary>
    /// Gets the sample rate of the audio data in samples per second (Hz) (e.g., 44100, 48000).
    /// </summary>
    public int SampleRate { get; }

    /// <summary>
    /// Gets the number of audio channels (e.g., 1 for mono, 2 for stereo).
    /// </summary>
    public int Channels { get; }

    /// <summary>
    /// Gets the total size of the raw audio data in bytes.
    /// </summary>
    public int DataSize { get; }

    /// <summary>
    /// Reads a sequence of bytes from the audio stream and advances the position within the stream by the number of bytes read.
    /// </summary>
    /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source.</param>
    /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
    /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
    /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero if the end of the stream has been reached.</returns>
    public int Read(byte[] buffer, int offset, int count);

    /// <summary>
    /// Sets the position within the audio data stream.
    /// </summary>
    /// <param name="offset">The byte offset to seek to.</param>
    public void Seek(long offset);

    /// <summary>
    /// Reads all the audio data from the current position to the end of the stream.
    /// </summary>
    /// <returns>A byte array containing all the remaining audio data.</returns>
    public byte[] ReadAll();
}