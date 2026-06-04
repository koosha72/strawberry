using Strawberry.Math;

namespace Strawberry.Sound
{
    public abstract class SoundBuffer : DisposableReferenceObject
    {
        /// <summary>
        /// The SoundManager using which the buffer is created.
        /// </summary>
        public abstract ISoundManager SoundManager { get; }
        /// <summary>
        /// The bits per sample of the audio
        /// </summary>
        public abstract int BitsPerSample { get; }
        /// <summary>
        /// The sample rate of the audio
        /// </summary>
        public abstract int SampleRate { get; }
        /// <summary>
        /// The number of channels in the audio. 1 for mono, 2 for stereo
        /// </summary>
        public abstract int Channels { get; }
        /// <summary>
        /// The time in seconds of the audio
        /// </summary>
        public abstract float Seconds { get; }

        /// <summary>
        /// Plays the buffer
        /// </summary>
        /// <param name="loop">Wheather to loop the buffer</param>
        /// <returns>The corresponding voice which will be played. You can use this object to stop the playing sound</returns>
        public abstract Voice Play(bool loop = false);

        /// <summary>
        /// Plays the buffer using a frequency ratio
        /// </summary>
        /// <param name="frequencyRatio">The frequency ratio</param>
        /// <param name="loop">Whether to loop the buffer</param>
        /// <returns>The corresponding voice which will be played. You can use this object to stop the playing sound</returns>
        public abstract Voice Play(float frequencyRatio, bool loop = false);

        /// <summary>
        /// Plays the buffer on the specified position
        /// </summary>
        /// <param name="position">The position of the played voice</param>
        /// <param name="loop">Wheather to loop the buffer</param>
        /// <returns>The corresponding voice which will be played. You can use this object to stop the playing sound</returns>
        public abstract Voice3D Play(Vector2 position, bool loop = false);

        /// <summary>
        /// Plays the buffer using a frequency ratio on the specified position
        /// </summary>
        /// <param name="position">The position of the played voice</param>
        /// <param name="frequencyRatio">The frequency ratio</param>
        /// <param name="loop">Whether to loop the buffer</param>
        /// <returns>The corresponding voice which will be played. You can use this object to stop the playing sound</returns>
        public abstract Voice3D Play(Vector2 position, float frequencyRatio, bool loop = false);

        /// <summary>
        /// Plays the buffer using the specified settings
        /// </summary>
        /// <param name="position">The settings to be used by the voice</param>
        /// <param name="loop">Wheather to loop the buffer</param>
        /// <returns>The corresponding voice which will be played. You can use this object to stop the playing sound</returns>
        public abstract Voice3D Play(Voice3DSettings settings, bool loop = false);

        /// <summary>
        /// Plays the buffer using a frequency ratio using the specified settings
        /// </summary>
        /// <param name="settings">The settings to be used by the voice</param>
        /// <param name="frequencyRatio">The frequency ratio</param>
        /// <param name="loop">Whether to loop the buffer</param>
        /// <returns>The corresponding voice which will be played. You can use this object to stop the playing sound</returns>
        public abstract Voice3D Play(Voice3DSettings settings, float frequencyRatio, bool loop = false);

        /// <summary>
        /// Stops all the voices played using this buffer.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Checks whether or not a voice is being played using this buffer.
        /// </summary>
        /// <returns>True if a voice is beign played</returns>
        public abstract bool IsPlaying();
    }
}
