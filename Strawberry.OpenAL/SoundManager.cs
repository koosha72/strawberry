using Strawberry.Sound;

namespace Strawberry.OpenAL
{
    public class SoundManager : Base, ISoundManager
    {
        nint device;
        nint context;

        List<Voice> sources = new List<Voice>();

        public IEnumerable<Voice> Sources { get { return sources; } }

        bool streaming = true;

        Thread streamBuffersThread;

        List<SoundStream> streams;

        object mutex = new object();

        public bool IsEnabled { get; set; }
        public ISound3DListener ActiveListener { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SoundManager()
        {
            device = ALC.OpenDevice(null);
            int[] att = null;
            context = ALC.CreateContext(device, att);
            ALC.MakeContextCurrent(context);
            streams = new List<SoundStream>();

            streamBuffersThread = new Thread(StreamThread);
            streamBuffersThread.IsBackground = true;
            streamBuffersThread.Start();
        }

        void StreamThread()
        {
            while (streaming)
            {
                lock (mutex)
                {
                    Thread.Sleep(100);
                    for (int i = 0; i < streams.Count; i++)
                    {
                        if (!streams[i].Update())
                        {
                            streams.Remove(streams[i]);
                            i--;
                        }
                    }
                }
            }
        }

        public ISoundBuffer CreateSoundBuffer(ISoundReader soundReader)
        {
            int buffer = AL.GenBuffer();

            AL.BufferData(buffer, GetSoundFormat(soundReader.Channels, soundReader.BitsPerSample),
                soundReader.ReadAll(), soundReader.DataSize, soundReader.SampleRate);

            return new SoundBuffer(buffer, this);
        }

        public ISoundStream CreateStream(ISoundReader soundReader)
        {
            return new SoundStream(this, soundReader);
        }

        public void StopAll()
        {
            for (int i = 0; i < sources.Count; i++)
            {
                if (sources[i] != null)
                {
                    AL.SourceStop(sources[i].SourceInd);
                    AL.DeleteSource(sources[i].SourceInd);
                }
            }

            sources.Clear();
        }

        internal Voice Play(SoundBuffer buffer, float frequencyRatio = 1.0f, bool loop = false)
        {
            int ind = FindIndex();
            int source = -1;
            Voice v;
            if (ind == -1)
            {
                source = AL.GenSource();

                v = new Voice(buffer, source);
                sources.Add(v);
            }
            else
            {
                source = sources[ind].SourceInd;
                sources[ind].Buffer = buffer;
                v = sources[ind];
            }


            AL.Sourcei(source, ALSourcei.Buffer, buffer.ID);
            AL.Sourcef(source, ALSourcef.Pitch, frequencyRatio);
            if (loop)
                AL.Sourceb(source, ALSourceb.Looping, true);
            AL.SourcePlay(source);
            return v;
        }

        internal Voice3D Play(SoundBuffer buffer, Voice3DSettings settings, float frequencyRatio = 1.0f, bool loop = false)
        {
            int ind = FindIndex();
            int source = -1;
            Voice3D v;
            if (ind == -1)
            {
                source = AL.GenSource();

                v = new Voice3D(buffer, settings, source);
                sources.Add(v);
            }
            else
            {
                source = sources[ind].SourceInd;
                sources[ind].Buffer = buffer;
                v = (Voice3D)sources[ind];
            }


            AL.Sourcei(source, ALSourcei.Buffer, buffer.ID);
            AL.Sourcef(source, ALSourcef.Pitch, frequencyRatio);
            AL.Source3f(source, ALSource3f.Position, settings.Position.X, settings.Position.Y, settings.Position.Z);
            AL.Source3f(source, ALSource3f.Velocity, settings.Velocity.X, settings.Velocity.Y, settings.Velocity.Z);
            AL.Source3f(source, ALSource3f.Direction, settings.Direction.X, settings.Direction.Y, settings.Direction.Z);
            if (loop)
                AL.Sourceb(source, ALSourceb.Looping, true);
            AL.SourcePlay(source);
            return v;
        }

        internal void Stop(Voice voice)
        {
            if (AL.IsSource(voice.SourceInd))
            {
                AL.SourceStop(voice.SourceInd);
                AL.DeleteSource(voice.SourceInd);
                sources.Remove(voice);
            }
        }

        internal void Stop(SoundBuffer buffer)
        {
            for (int i = 0; i < sources.Count; i++)
            {
                Voice voice = sources[i];
                if (voice != null)
                {
                    if (voice.IsPlaying() && voice.Buffer == buffer)
                    {
                        Stop(voice);
                        i--;
                    }
                }
            }
        }

        private int FindIndex()
        {
            int ind = -1;

            for (int i = 0; i < sources.Count; i++)
            {
                if (!sources[i].IsPlaying() && !sources[i].IsPaused())
                {
                    ind = i;
                    break;
                }
            }

            return ind;
        }

        internal ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }

        internal void AddStream(SoundStream soundStream)
        {
            lock (mutex)
            {
                streams.Add(soundStream);
            }
        }

        protected override void CleanUnmanaged()
        {
            var device = ALC.GetContextsDevice(context);
            ALC.MakeContextCurrent(0);
            ALC.DestroyContext(context);
            ALC.CloseDevice(device);
            streaming = false;
            base.CleanUnmanaged();
        }
    }
}
