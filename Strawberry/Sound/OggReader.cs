/*
 * Strawberry Game Engine
 * File: OggReader.cs
 * Author: Koosha Aabedini Nassab
 *
 * OGG Vorbis reader implementation used for streaming audio.
 */

using Math = System.Math;
using NVorbis;

namespace Strawberry.Sound
{
    using Math = System.Math;
    /// <summary>
    /// An <see cref="ISoundReader"/> that decodes OGG Vorbis audio files,
    /// compatible with the Strawberry engine's streaming system.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This reader uses NVorbis (fully managed, no native dependencies) to decode
    /// OGG Vorbis files. It is compatible with WebAssembly and AOT compilation.
    /// </para>
    /// <para>
    /// Requires the NVorbis NuGet package:
    ///   <c>dotnet add package NVorbis</c>
    /// </para>
    /// </remarks>
    public sealed class OggReader : ISoundReader
    {
        readonly VorbisReader vorbis;
        readonly bool ownsVorbis;
        bool disposed;

        // Reusable float decode buffer to avoid per-call GC allocation
        float[] decodeBuffer;
        int decodeBufferCapacity;

        // ─────────────────────────────────────────────
        //  Properties (ISoundReader)
        // ─────────────────────────────────────────────

        /// <summary>The original OGG file stream (read-only reference).</summary>
        public Stream Stream { get; }

        /// <summary>Always 16 (16-bit PCM output for OpenAL compatibility).</summary>
        public int BitsPerSample => 16;

        /// <summary>Output sample rate in Hz (from the OGG file header).</summary>
        public int SampleRate => vorbis.SampleRate;

        /// <summary>Output channel count (1 = mono, 2 = stereo).</summary>
        public int Channels => vorbis.Channels;

        /// <summary>Total PCM data size in bytes, or int.MaxValue for unknown-length streams.</summary>
        public int DataSize { get; }

        // ─────────────────────────────────────────────
        //  Constructor
        // ─────────────────────────────────────────────

        /// <summary>
        /// Create an OggReader from a stream containing OGG Vorbis data.
        /// The stream is not closed when the reader is disposed.
        /// </summary>
        /// <param name="stream">Stream containing OGG Vorbis data. Must be seekable.</param>
        public OggReader(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            Stream = stream;
            vorbis = new VorbisReader(stream, false);
            ownsVorbis = true;

            long totalSamples = vorbis.TotalSamples;
            if (totalSamples > 0)
            {
                long dataSize = totalSamples * Channels * (BitsPerSample / 8);
                DataSize = dataSize > int.MaxValue ? int.MaxValue : (int)dataSize;
            }
            else
            {
                // Unknown length (e.g. live stream) — report max so the
                // streaming system keeps reading until Read returns 0.
                DataSize = int.MaxValue;
            }
        }

        // ─────────────────────────────────────────────
        //  ISoundReader — Read
        // ─────────────────────────────────────────────

        /// <summary>
        /// Read decoded PCM audio into the buffer. OGG Vorbis data is decoded
        /// on-demand and converted from float to 16-bit PCM.
        /// </summary>
        /// <returns>Number of bytes written into the buffer, or 0 if at end of data.</returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            ThrowIfDisposed();

            if (count <= 0) return 0;

            int bytesPerFrame = Channels * (BitsPerSample / 8);
            int framesToRead = count / bytesPerFrame;
            if (framesToRead <= 0) return 0;

            // NVorbis.ReadSamples reads interleaved float values:
            // for stereo, each frame = 2 floats (L, R)
            int floatCount = framesToRead * Channels;

            EnsureDecodeBuffer(floatCount);

            int samplesRead = vorbis.ReadSamples(decodeBuffer!, 0, floatCount);
            if (samplesRead <= 0) return 0;

            // Only write complete frames (trim partial frames at end of stream)
            int framesRead = samplesRead / Channels;
            int samplesToConvert = framesRead * Channels;

            // Convert float [-1..1] → 16-bit PCM directly into the byte buffer.
            // This avoids allocating an intermediate short[] array.
            for (int i = 0; i < samplesToConvert; i++)
            {
                float v = decodeBuffer![i];

                // Clamp to valid range
                if (v > 1.0f) v = 1.0f;
                else if (v < -1.0f) v = -1.0f;

                short s = (short)(v * 32767f);

                // Write little-endian 16-bit sample
                int bytePos = offset + i * 2;
                buffer[bytePos] = (byte)(s & 0xFF);
                buffer[bytePos + 1] = (byte)((s >> 8) & 0xFF);
            }

            return framesRead * bytesPerFrame;
        }

        // ─────────────────────────────────────────────
        //  ISoundReader — Seek
        // ─────────────────────────────────────────────

        /// <summary>
        /// Seek to a byte offset in the decoded PCM data.
        /// The byte offset is converted to a sample frame position.
        /// </summary>
        public void Seek(long offset)
        {
            ThrowIfDisposed();

            int bytesPerFrame = Channels * (BitsPerSample / 8);
            long targetFrame = offset / bytesPerFrame;

            // Clamp to valid range if total length is known
            if (vorbis.TotalSamples > 0)
                targetFrame = Math.Clamp(targetFrame, 0, vorbis.TotalSamples);

            vorbis.SamplePosition = targetFrame;
        }

        // ─────────────────────────────────────────────
        //  ISoundReader — ReadAll
        // ─────────────────────────────────────────────

        /// <summary>
        /// Read the entire OGG file as decoded PCM data.
        /// Resets the reader to the beginning first, then decodes all audio.
        /// </summary>
        public byte[] ReadAll()
        {
            ThrowIfDisposed();
            Seek(0);

            if (DataSize == int.MaxValue)
            {
                // Unknown length — read until end
                using var ms = new MemoryStream();
                byte[] temp = new byte[16384];
                int read;
                while ((read = Read(temp, 0, temp.Length)) > 0)
                    ms.Write(temp, 0, read);
                return ms.ToArray();
            }

            var buffer = new byte[DataSize];
            int totalRead = 0;
            while (totalRead < DataSize)
            {
                int bytesRead = Read(buffer, totalRead, DataSize - totalRead);
                if (bytesRead <= 0) break;
                totalRead += bytesRead;
            }
            return buffer;
        }

        // ─────────────────────────────────────────────
        //  IDisposable
        // ─────────────────────────────────────────────

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            if (ownsVorbis)
                vorbis.Dispose();

            decodeBuffer = null;
        }

        // ─────────────────────────────────────────────
        //  Internal
        // ─────────────────────────────────────────────

        void EnsureDecodeBuffer(int floatCount)
        {
            if (decodeBuffer == null || decodeBufferCapacity < floatCount)
            {
                decodeBufferCapacity = Math.Max(floatCount, 4096);
                decodeBuffer = new float[decodeBufferCapacity];
            }
        }

        void ThrowIfDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(OggReader));
        }
    }
}
