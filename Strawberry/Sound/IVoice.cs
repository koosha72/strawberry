namespace Strawberry.Sound
{
    public interface IVoice
    {
        public ISoundBuffer Buffer { get; }

        /// <summary>
        /// Gets the current play time of the voice in seconds.
        /// </summary>
        float CurrentPlayTime { get; set; }

        public float Volume { get; set; }

        /// <summary>
        /// Checks whether the voice is beign played.
        /// </summary>
        /// <returns>True if the voice is playing, Otherwise false.</returns>
        bool IsPlaying();

        /// <summary>
        /// Checks wether the voice is paused.
        /// </summary>
        /// <returns>True if the voice is paused, Otherwise false.</returns>
        bool IsPaused();

        /// <summary>
        /// Stops the voice from playing.
        /// </summary>
        void Stop();

        /// <summary>
        /// Pauses the voice.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes the voice.
        /// </summary>
        void Resume();
    }
}
