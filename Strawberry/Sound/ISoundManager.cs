using Strawberry.Math;

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
        /// Creates a 3D sound listener.
        /// </summary>
        /// <param name="position">The position of the listener</param>
        /// <param name="velocity">The velocity of the listener</param>
        /// <param name="lookAt">The direction of the listener</param>
        /// <param name="up">The up axis of the listener</param>
        /// <param name="activate">If true, the listener will be activated immediately after creation</param>
        /// <returns>A new sound listener</returns>
        Sound3DListener Create3DListener(Vector3 position, Vector3 velocity, Vector3 lookAt, Vector3 up, bool activate);

        /// <summary>
        /// Stops all the sounds playing.
        /// </summary>
        void StopAll();
    }
}
