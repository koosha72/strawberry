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

        void Play(bool loop = false);

        bool IsStreaming();
    }
}
