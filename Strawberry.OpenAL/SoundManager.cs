using Strawberry.Math;
using Strawberry.Sound;

namespace Strawberry.OpenAL
{
    public class SoundManager : Base, ISoundManager
    {
        nint device;
        nint context;

        List<IVoice> sources = new List<IVoice>();

        public IEnumerable<IVoice> Sources { get { return sources; } }

        bool streaming = true;

        // Thread streamBuffersThread;

        List<SoundStream> streams;

        object mutex = new object();

        public bool IsEnabled { get; set; }

        Strawberry.Sound.Sound3DListener sound3DListener;

        private long lastStreamUpdate;
        private const long ticksPer100ms = 1000000;

        public Strawberry.Sound.Sound3DListener ActiveListener
        {
            get => sound3DListener;
            set
            {
                sound3DListener = value;
                if (sound3DListener != null)
                {
                    sound3DListener.Activate();
                }
            }
        }

        public SoundManager()
        {
            device = ALC.OpenDevice(null);
            int[] att = null;
            context = ALC.CreateContext(device, att);
            ALC.MakeContextCurrent(context);
            streams = new List<SoundStream>();

            /* streamBuffersThread = new Thread(StreamThread);
            streamBuffersThread.IsBackground = true;
            streamBuffersThread.Start(); */
        }

        public void Update()
        {
            long now = System.Diagnostics.Stopwatch.GetTimestamp();

            if ((double)(now - lastStreamUpdate) / System.Diagnostics.Stopwatch.Frequency < 0.1)
            {
                return;
            }


            for (int i = 0; i < streams.Count; i++)
            {
                if (!streams[i].Update())
                {
                    streams[i].Dispose();
                    streams.Remove(streams[i]);
                    i--;
                }
            }
            for (int i = 0; i < sources.Count; i++)
            {
                if (!sources[i].IsPlaying() && !sources[i].IsPaused())
                {
                    sources[i].Dispose();
                    sources.RemoveAt(i);
                    i--;
                }
            }

            lastStreamUpdate = now;
        }

        public Strawberry.Sound.SoundBuffer CreateSoundBuffer(ISoundReader soundReader)
        {
            int buffer = AL.GenBuffer();

            AL.BufferData(buffer, GetSoundFormat(soundReader.Channels, soundReader.BitsPerSample),
                soundReader.ReadAll(), soundReader.DataSize, soundReader.SampleRate);

            return new SoundBuffer(buffer, this);
        }

        public Strawberry.Sound.SoundStream CreateStream(ISoundReader soundReader)
        {
            return new SoundStream(this, soundReader);
        }

        public Strawberry.Sound.Sound3DListener Create3DListener(Vector3 position, Vector3 velocity, Vector3 lookAt, Vector3 up, bool activate)
        {
            var listener = new Sound3DListener(this)
            {
                Position = position,
                Velocity = velocity,
                LookAt = lookAt,
                Up = up,
            };

            if (activate)
            {
                ActiveListener = listener;
            }

            return listener;
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
                if (sources[ind] is not Voice)
                {
                    sources[ind] = new Voice(buffer, source);
                }
                sources[ind].SetBuffer(buffer);
                v = (Voice)sources[ind];
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
                if (sources[ind] is not Voice3D)
                {
                    sources[ind] = new Voice3D(buffer, settings, source);
                }
                sources[ind].SetBuffer(buffer);
                v = (Voice3D)sources[ind];
            }


            AL.Sourcei(source, ALSourcei.Buffer, buffer.ID);
            AL.Sourcef(source, ALSourcef.Pitch, frequencyRatio);
            AL.Source3f(source, ALSource3f.Position, settings.Position.X, settings.Position.Y, settings.Position.Z);
            AL.Source3f(source, ALSource3f.Velocity, settings.Velocity.X, settings.Velocity.Y, settings.Velocity.Z);
            AL.Source3f(source, ALSource3f.Direction, settings.Direction.X, settings.Direction.Y, settings.Direction.Z);
            AL.Sourceb(source, ALSourceb.SourceRelative, false);
            AL.Sourcef(source, ALSourcef.ReferenceDistance, 100.0f);
            AL.Sourcef(source, ALSourcef.RolloffFactor, 1.0f);
            AL.Sourcef(source, ALSourcef.MaxDistance, 10000.0f);

            if (loop)
                AL.Sourceb(source, ALSourceb.Looping, true);
            AL.SourcePlay(source);
            return v;
        }

        internal void Stop(IVoice voice)
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
                IVoice voice = sources[i];
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
            streams.Add(soundStream);
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
