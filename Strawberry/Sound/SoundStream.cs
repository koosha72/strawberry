namespace Strawberry.Sound
{
    public abstract class SoundStream : DisposableReferenceObject
    {
        /// <summary>
        /// The SoundManager using which the buffer is created.
        /// </summary>
        public abstract ISoundManager SoundManager { get; }

        public abstract  int BitsPerSample { get; }

        public abstract  int SampleRate { get; }

        public abstract  int Channels { get; }
        public abstract bool IsLoop { get; }

        public abstract  float Seconds { get; }

        public abstract  float Volume { get; set; }

        public abstract float CurrentPlayTime { get; set; }

        public abstract void Play(bool loop = false);

        public abstract void Resume();

        public abstract void Stop();

        public abstract void Pause();

        public abstract bool IsStreaming();

        public abstract bool IsPaused();

        public abstract bool Update();
    }
}
