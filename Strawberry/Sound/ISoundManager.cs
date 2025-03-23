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
        /// Creates a sound buffer using the file specified.
        /// </summary>
        /// <param name="fileName">File path</param>
        /// <returns>The created sound buffer. You can use this buffer to play sounds</returns>
        ISoundBuffer CreateSoundBuffer(string fileName);

        /// <summary>
        /// Creates a sound buffer using the file specified.
        /// </summary>
        /// <param name="fileName">File path</param>
        /// <param name="fileName">The format of the file</param>
        /// <returns>The created sound buffer. You can use this buffer to play sounds</returns>
        ISoundBuffer CreateSoundBuffer(string fileName, SoundFormat format);

        /// <summary>
        /// Creates a sound buffer using a byte array.
        /// </summary>
        /// <param name="data">The sound data</param>
        /// <returns>The created sound buffer. You can use this buffer to play sounds</returns>
        ISoundBuffer CreateSoundBuffer(byte[] data);

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
