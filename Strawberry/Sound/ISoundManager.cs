namespace Strawberry.Sound
{
    public interface ISoundManager : IBase
    {
        bool IsEnabled { get; set; }

        /// <summary>
        /// Current active sound listener.
        /// NOTE: There can only be 1 active listener.
        /// </summary>
        ISound3DListener ActiveListener { get; set; }

        /// <summary>
        /// Creates a sound buffer.
        /// </summary>
        /// <param name="soundReader">An implementation of a sound reader, That loads correct sound format, e.g. WaveReader</param>
        /// <returns>The created SoundBuffer</returns>
        ISoundBuffer CreateSoundBuffer(ISoundReader soundReader);

        /// <summary>
        /// Creates a sound stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>The created sound stream. You can stream sounds without loading them entirely in the memory using this class.</returns>
        ISoundStream CreateStream(Stream stream);

        /// <summary>
        /// Stops all the sounds playing.
        /// </summary>
        void StopAll();
    }
}
