namespace Strawberry.Sound
{
    public interface ISoundStream : IBase
    {
        /// <summary>
        /// The SoundManager using which the buffer is created.
        /// </summary>
        ISoundManager SoundManager { get; }

        public int BitsPerSample { get; }

        public int SampleRate { get; }

        public int Channels { get; }
        bool IsLoop { get; }

        public float Seconds { get; }

        public float Volume { get; set; }

        float CurrentPlayTime { get; set; }

        void Play(bool loop = false);

        bool IsStreaming();

        bool Update();
    }
}
