using Strawberry.Sound;

namespace Strawberry.OpenGL.Sound
{
    public class SoundStream : Base, ISoundStream
    {
        public ISoundManager SoundManager => throw new NotImplementedException();

        public int BitsPerSample => throw new NotImplementedException();

        public int SampleRate => throw new NotImplementedException();

        public int Channels => throw new NotImplementedException();

        public bool IsLoop => throw new NotImplementedException();

        public bool IsStreaming()
        {
            throw new NotImplementedException();
        }

        public void Play(bool loop = false)
        {
            throw new NotImplementedException();
        }
    }
}
