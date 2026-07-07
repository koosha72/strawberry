using Strawberry.Math;
using Strawberry.Sound;

namespace Strawberry.OpenAL
{
    public class SoundManager : Base, ISoundManager
    {
        nint device;
        nint context;

        private const int MaxStreamingSources = 4;
        private const int MaxEffectSources = 28;

        private Stack<int> availableEffectSources = new Stack<int>();
        private Stack<int> availableStreamingSources = new Stack<int>();

        List<IVoice> activeVoices = new List<IVoice>();

        public IEnumerable<IVoice> Sources { get { return activeVoices; } }

        bool streaming = true;

        // Thread streamBuffersThread;

        List<SoundStream> streams;

        object mutex = new object();

        public bool IsEnabled { get; set; } = true;

        Strawberry.Sound.Sound3DListener sound3DListener;

        private long lastStreamUpdate;

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

            for (int i = 0; i < MaxEffectSources; i++)
                availableEffectSources.Push(AL.GenSource());

            for (int i = 0; i < MaxStreamingSources; i++)
                availableStreamingSources.Push(AL.GenSource());

            /* streamBuffersThread = new Thread(StreamThread);
            streamBuffersThread.IsBackground = true;
            streamBuffersThread.Start(); */
        }

        public void Suspend()
        {
            lock (mutex)
            {
                IsEnabled = false;

                foreach (var voice in activeVoices)
                {
                    voice.Pause();
                }
                foreach (var s in streams)
                {
                    s.Pause();
                }
                ALC.MakeContextCurrent(0);
            }
        }

        public void RestoreState()
        {
            lock (mutex)
            {
                IsEnabled = true;
                ALC.MakeContextCurrent(context);
                foreach (var voice in activeVoices)
                {
                    voice.Resume();
                }
                foreach (var s in streams)
                {
                    s.Resume();
                }
            }
        }

        public void Update()
        {
            lock (mutex)
            {
                if (!IsEnabled)
                    return;
                long now = System.Diagnostics.Stopwatch.GetTimestamp();

                if ((double)(now - lastStreamUpdate) / System.Diagnostics.Stopwatch.Frequency < 0.1)
                {
                    return;
                }


                for (int i = 0; i < streams.Count; i++)
                {
                    if (!streams[i].Update())
                    {
                        availableStreamingSources.Push(streams[i].SourceInd);
                        streams[i].Dispose();
                        streams.Remove(streams[i]);
                        i--;
                    }
                }
                for (int i = activeVoices.Count - 1; i >= 0; i--)
                {
                    var v = activeVoices[i];
                    if (!v.IsPlaying() && !v.IsPaused() && !v.IsVirtual)
                    {
                        AL.Sourcei(v.SourceInd, ALSourcei.Buffer, 0);

                        if (v is SoundStream)
                            availableStreamingSources.Push(v.SourceInd);
                        else
                            availableEffectSources.Push(v.SourceInd);

                        v.SourceInd = -1;
                        activeVoices.RemoveAt(i);
                    }
                }

                lastStreamUpdate = now;
            }
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
            int source = RequestStreamingSource();

            if (source == -1)
            {
                throw new Exception(string.Format("No free streaming sources you can stream {0}", MaxStreamingSources));
            }
            return new SoundStream(this, soundReader, source);
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
            foreach (var voice in activeVoices)
            {
                voice.Stop();
            }

            activeVoices.Clear();
        }

        internal Voice Play(SoundBuffer buffer, float frequencyRatio = 1.0f, bool loop = false, int priority = 0)
        {
            int source = RequestEffectSource(priority);

            if (source == -1)
            {
                return null;
            }

            Voice v = new Voice(buffer, priority);
            v.SourceInd = source;

            AL.Sourcei(source, ALSourcei.Buffer, buffer.ID);
            AL.Sourcef(source, ALSourcef.Pitch, frequencyRatio);
            if (loop) AL.Sourceb(source, ALSourceb.Looping, true);
            AL.SourcePlay(source);

            activeVoices.Add(v);
            v.Volume = 1.0f;
            return v;
        }

        internal Voice3D Play(SoundBuffer buffer, Voice3DSettings settings, float frequencyRatio = 1.0f, bool loop = false, int priority = 0)
        {
            int source = RequestEffectSource(priority);

            if (source == -1)
            {
                return null;
            }

            Voice3D v = new Voice3D(buffer, settings, priority);
            v.SourceInd = source;

            AL.Sourcei(source, ALSourcei.Buffer, buffer.ID);
            AL.Sourcef(source, ALSourcef.Pitch, frequencyRatio);
            AL.Source3f(source, ALSource3f.Position, settings.Position.X, settings.Position.Y, settings.Position.Z);
            AL.Source3f(source, ALSource3f.Velocity, settings.Velocity.X, settings.Velocity.Y, settings.Velocity.Z);
            AL.Source3f(source, ALSource3f.Direction, settings.Direction.X, settings.Direction.Y, settings.Direction.Z);
            AL.Sourceb(source, ALSourceb.SourceRelative, false);
            AL.Sourcef(source, ALSourcef.ReferenceDistance, 100.0f);
            AL.Sourcef(source, ALSourcef.RolloffFactor, 1.0f);
            AL.Sourcef(source, ALSourcef.MaxDistance, 10000.0f);
            if (loop) AL.Sourceb(source, ALSourceb.Looping, true);
            AL.SourcePlay(source);

            activeVoices.Add(v);
            v.Volume = 1.0f;
            return v;
        }

        internal void Stop(IVoice voice)
        {
            if (voice.IsVirtual) return;

            if (AL.IsSource(voice.SourceInd))
            {
                AL.SourceStop(voice.SourceInd);
                AL.Sourcei(voice.SourceInd, ALSourcei.Buffer, 0);

                if (voice is SoundStream)
                    availableStreamingSources.Push(voice.SourceInd);
                else
                    availableEffectSources.Push(voice.SourceInd);
            }

            activeVoices.Remove(voice);
        }

        internal void Stop(SoundBuffer buffer)
        {
            for (int i = activeVoices.Count - 1; i >= 0; i--)
            {
                IVoice voice = activeVoices[i];
                if (voice != null)
                {
                    if (voice.Buffer == buffer)
                        Stop(voice);
                }
            }
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
            StopAll();
            var device = ALC.GetContextsDevice(context);
            ALC.MakeContextCurrent(0);
            ALC.DestroyContext(context);
            ALC.CloseDevice(device);
            streaming = false;
            foreach (var source in availableEffectSources)
            {
                AL.SourceStop(source);
                AL.DeleteSource(source);
            }
            availableEffectSources.Clear();
            foreach (var source in availableStreamingSources)
            {
                AL.SourceStop(source);
                AL.DeleteSource(source);
            }
            availableStreamingSources.Clear();
            base.CleanUnmanaged();
        }

        private int RequestEffectSource(int requestedPriority)
        {
            if (availableEffectSources.Count > 0)
                return availableEffectSources.Pop();

            IVoice voiceToSteal = null;
            int lowestPriority = int.MaxValue;

            foreach (var v in activeVoices)
            {
                if (v is SoundStream) continue;

                if (v.Priority < lowestPriority && !v.IsPaused())
                {
                    lowestPriority = v.Priority;
                    voiceToSteal = v;
                }
            }

            if (voiceToSteal != null && requestedPriority > lowestPriority)
            {
                int stolenSource = voiceToSteal.SourceInd;

                AL.SourceStop(stolenSource);
                AL.Sourcei(stolenSource, ALSourcei.Buffer, 0);

                voiceToSteal.SourceInd = -1;
                activeVoices.Remove(voiceToSteal);

                return stolenSource;
            }

            return -1;
        }

        private int RequestStreamingSource()
        {
            if (availableStreamingSources.Count > 0)
                return availableStreamingSources.Pop();

            return -1;
        }
    }
}
