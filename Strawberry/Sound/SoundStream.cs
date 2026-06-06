/*
 * Strawberry Game Engine
 * File: SoundStream.cs
 * Author: Koosha Aabedini Nassab
 *
 * Streamed audio interface for long or streaming sounds.
 */

namespace Strawberry.Sound
{
    /// <summary>
    /// A sound stream that can be used to play a sound
    /// </summary>
    public abstract class SoundStream : DisposableReferenceObject
    {
        /// <summary>
        /// The SoundManager using which the buffer is created.
        /// </summary>
        public abstract ISoundManager SoundManager { get; }
        /// <summary>
        /// Gets the bits per sample of the audio
        /// </summary>
        public abstract  int BitsPerSample { get; }
        /// <summary>
        /// Gets the sample rate of the audio
        /// </summary>
        public abstract  int SampleRate { get; }
        /// <summary>
        /// Gets the number of channels in the audio. 1 for mono, 2 for stereo
        /// </summary>
        public abstract  int Channels { get; }
        /// <summary>
        /// Gets whether the stream is looping
        /// </summary>
        public abstract bool IsLoop { get; }
        /// <summary>
        /// Gets the length of the stream in seconds
        /// </summary>
        public abstract  float Seconds { get; }
        /// <summary>
        /// Gets or sets the volume of the stream
        /// </summary>
        public abstract  float Volume { get; set; }
        /// <summary>
        /// Gets or sets the current play time of the stream in seconds
        /// </summary>
        public abstract float CurrentPlayTime { get; set; }
        /// <summary>
        /// Plays the stream
        /// </summary>
        /// <param name="loop">If true, the stream will loop</param>
        public abstract void Play(bool loop = false);
        /// <summary>
        /// Resumes the stream
        /// </summary>
        public abstract void Resume();
        /// <summary>
        /// Stops the stream
        /// </summary>
        public abstract void Stop();
        /// <summary>
        /// Pauses the stream
        /// </summary>
        public abstract void Pause();
        /// <summary>
        /// Returns true if the stream is playing
        /// </summary>
        /// <returns>The state of the stream. True if playing, false otherwise</returns>
        public abstract bool IsStreaming();
        /// <summary>
        /// Returns true if the stream is paused, false otherwise
        /// </summary>
        /// <returns>The state of the stream. True if paused, false otherwise</returns>
        public abstract bool IsPaused();
        /// <summary>
        /// Reads the stream and returns true if it is still streaming, false otherwise
        /// </summary>
        /// <returns>True if the stream is still streaming, false otherwise</returns>
        public abstract bool Update();
    }
}
