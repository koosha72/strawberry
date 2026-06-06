/*
 * Strawberry Game Engine
 * File: Voice.cs
 * Author: Koosha Aabedini Nassab
 *
 * Abstract playback voice representing a playing instance of a buffer.
 */

namespace Strawberry.Sound
{
    public abstract class Voice : DisposableReferenceObject

    {
        public abstract SoundBuffer Buffer { get; }

        /// <summary>
        /// Gets the current play time of the voice in seconds.
        /// </summary>
        public abstract float CurrentPlayTime { get; set; }

        /// <summary>
        /// Gets or sets the playback volume for this voice. This property allows real-time volume adjustment during playback.
        /// </summary>
        public abstract float Volume { get; set; }

        /// <summary>
        /// Checks whether the voice is beign played.
        /// </summary>
        /// <returns>True if the voice is playing, Otherwise false.</returns>
        public abstract bool IsPlaying();

        /// <summary>
        /// Checks wether the voice is paused.
        /// </summary>
        /// <returns>True if the voice is paused, Otherwise false.</returns>
        public abstract bool IsPaused();

        /// <summary>
        /// Stops the voice from playing.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Pauses the voice.
        /// </summary>
        public abstract void Pause();

        /// <summary>
        /// Resumes the voice.
        /// </summary>
        public abstract void Resume();
    }
}
