namespace Strawberry.Sound
{
    public interface ISoundManager : IBase
    {
        bool IsEnabled { get; set; }

        /// <summary>
        /// Current active sound listener.
        /// NOTE: There can only be 1 active listener.
        /// </summary>
        Sound3DListener ActiveListener { get; set; }

        /// <summary>
        /// Creates a sound buffer.
        /// </summary>
        /// <param name="soundReader">An implementation of a sound reader, That loads correct sound format, e.g. WaveReader</param>
        /// <returns>The created SoundBuffer</returns>
        SoundBuffer CreateSoundBuffer(ISoundReader soundReader);

        /// <summary>
        /// Creates a sound stream.
        /// </summary>
        /// <param name="soundReader">An implementation of a sound reader, That loads correct sound format, e.g. WaveReader</param>
        /// <returns>The created sound stream. You can stream sounds without loading them entirely in the memory using this class.</returns>
        SoundStream CreateStream(ISoundReader soundReader);

        /// <summary>
        /// Stops all the sounds playing.
        /// </summary>
        void StopAll();
    }
}
