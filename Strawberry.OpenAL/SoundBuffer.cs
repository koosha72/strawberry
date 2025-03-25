using Strawberry.Math;
using Strawberry.Sound;

namespace Strawberry.OpenAL
{
    public class SoundBuffer : Base, ISoundBuffer
    {
        SoundManager soundManager;

        public ISoundManager SoundManager => soundManager;

        public int BitsPerSample { get; internal set; }

        public int SampleRate { get; internal set; }

        public int Channels { get; internal set; }

        public int ID { get; private set; }

        public float Seconds
        {
            get
            {
                var size = AL.GetBufferi(ID, ALGetBufferi.Size);
                if (size == 0)
                    return 0;
                var frequency = (float)AL.GetBufferi(ID, ALGetBufferi.Frequency);
                var channels = (float)AL.GetBufferi(ID, ALGetBufferi.Channels);
                var bits = (float)AL.GetBufferi(ID, ALGetBufferi.Bits);
                return (size / channels / (bits / 8)) / frequency;
            }
        }

        public SoundBuffer(int id, SoundManager soundManager)
        {
            this.ID = id;
            this.soundManager = soundManager;
        }

        public bool IsPlaying()
        {
            foreach (Voice voice in soundManager.Sources)
            {
                if (voice != null)
                {
                    if (voice.IsPlaying() && voice.Buffer == this)
                        return true;
                }
            }

            return false;
        }

        public IVoice Play(bool loop = false)
        {
            return soundManager.Play(this, 1.0f, loop);
        }

        public IVoice Play(float frequencyRatio = 1.0f, bool loop = false)
        {
            return soundManager.Play(this, frequencyRatio, loop);
        }

        public void Stop()
        {
            soundManager.Stop(this);
        }

        protected override void CleanUnmanaged()
        {
            AL.DeleteBuffer(ID);
            base.CleanUnmanaged();
        }

        public IVoice3D Play(Vector2 position, bool loop = false)
        {
            return soundManager.Play(this, new Voice3DSettings()
            {
                Position = new Vector3(position, 0.0f),
                Direction = new Vector3(0.0f),
                Velocity = new Vector3(0.0f)
            }, 1.0f, loop);
        }

        public IVoice3D Play(Vector2 position, float frequencyRatio, bool loop = false)
        {
            return soundManager.Play(this, new Voice3DSettings()
            {
                Position = new Vector3(position, 0.0f),
                Direction = new Vector3(0.0f),
                Velocity = new Vector3(0.0f)
            }, frequencyRatio, loop);
        }

        public IVoice3D Play(Voice3DSettings settings, bool loop = false)
        {
            return soundManager.Play(this, settings, 1.0f, loop);
        }

        public IVoice3D Play(Voice3DSettings settings, float frequencyRatio, bool loop = false)
        {
            return soundManager.Play(this, settings, frequencyRatio, loop);
        }
    }
}
