using Strawberry.Math;
using Strawberry.Sound;

namespace Strawberry.OpenAL
{
    public class SoundBuffer : Strawberry.Sound.SoundBuffer
    {
        SoundManager soundManager;

        public override ISoundManager SoundManager => soundManager;

        public override int BitsPerSample { get;}

        public override int SampleRate { get; }

        public override int Channels { get; }

        public int ID { get; }

        public override float Seconds
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

        public override bool IsPlaying()
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

        public override Strawberry.Sound.Voice Play(bool loop = false)
        {
            return soundManager.Play(this, 1.0f, loop);
        }

        public override Strawberry.Sound.Voice Play(float frequencyRatio = 1.0f, bool loop = false)
        {
            return soundManager.Play(this, frequencyRatio, loop);
        }

        public override void Stop()
        {
            soundManager.Stop(this);
        }

        protected override void CleanUnmanaged()
        {
            AL.DeleteBuffer(ID);
            base.CleanUnmanaged();
        }

        public override Strawberry.Sound.Voice3D Play(Vector2 position, bool loop = false)
        {
            return soundManager.Play(this, new Voice3DSettings()
            {
                Position = new Vector3(position, 0.0f),
                Direction = new Vector3(0.0f),
                Velocity = new Vector3(0.0f)
            }, 1.0f, loop);
        }

        public override Strawberry.Sound.Voice3D Play(Vector2 position, float frequencyRatio, bool loop = false)
        {
            return soundManager.Play(this, new Voice3DSettings()
            {
                Position = new Vector3(position, 0.0f),
                Direction = new Vector3(0.0f),
                Velocity = new Vector3(0.0f)
            }, frequencyRatio, loop);
        }

        public override Strawberry.Sound.Voice3D Play(Voice3DSettings settings, bool loop = false)
        {
            return soundManager.Play(this, settings, 1.0f, loop);
        }

        public override Strawberry.Sound.Voice3D Play(Voice3DSettings settings, float frequencyRatio, bool loop = false)
        {
            return soundManager.Play(this, settings, frequencyRatio, loop);
        }
    }
}
