using Strawberry.Sound;

namespace Strawberry.OpenAL
{
    public class SoundStream : Strawberry.Sound.SoundStream
    {
        public override ISoundManager SoundManager { get; }

        public override int BitsPerSample { get; }

        public override int SampleRate { get; }

        public override int Channels { get; }

        bool isLoop = false;

        public override bool IsLoop { get { return isLoop; } }

        int totalSamplesPlayed = 0;
        bool shouldStop = false;

        bool paused = false;

        public override float Seconds
        {
            get
            {
                if (reader.DataSize == 0)
                    return 0;

                long totalSamples = reader.DataSize / (Channels * (BitsPerSample / 8));

                return (float)totalSamples / SampleRate;
            }
        }

        public override float CurrentPlayTime
        {
            get
            {
                if (reader.DataSize == 0 || !playing)
                    return 0;

                int currentSampleOffset = AL.GetSourcei(source, ALGetSourcei.SampleOffset);

                return (float)(totalSamplesPlayed + currentSampleOffset) / SampleRate;
            }
            set
            {
                if (reader.DataSize == 0)
                    return;

                int targetSample = (int)(value * SampleRate);
                long targetBytePos = targetSample * Channels * (BitsPerSample / 8);
                totalSamplesPlayed = targetSample;

                AL.SourceStop(source);
                AL.SourceUnqueueBuffers(source, AL.GetSourcei(source, ALGetSourcei.BuffersQueued), null);

                reader.Seek(targetBytePos);

                Update();
            }
        }

        public override float Volume
        {
            get
            {
                return AL.GetSourcef(source, ALSourcef.Gain);
            }
            set
            {
                AL.Sourcef(source, ALSourcef.Gain, value);
            }
        }


        int bufferCount = 3;
        int[] buffers;

        int source;

        ISoundReader reader;

        bool playing = false;

        object mutex = new object();

        public SoundStream(SoundManager soundManager, ISoundReader reader)
        {
            buffers = new int[bufferCount];
            AL.GenBuffers(bufferCount, buffers);
            source = AL.GenSource();
            SoundManager = soundManager;
            this.reader = reader;

            BitsPerSample = reader.BitsPerSample;
            SampleRate = reader.SampleRate;
            Channels = reader.Channels;
        }

        public void Load(string path)
        {

        }

        public override bool IsStreaming()
        {
            return playing && !paused;
        }

        public override bool IsPaused()
        {
            return paused;
        }

        public override void Play(bool loop = false)
        {
            lock (mutex)
            {
                if (playing)
                    Stop();
                StartOver();
                playing = true;
                isLoop = loop;
                (SoundManager as SoundManager)?.AddStream(this);
            }
        }

        public override void Resume()
        {
            AL.SourcePlay(source);
            paused = false;
            return;
        }

        private void StartOver()
        {
            lock (mutex)
            {
                totalSamplesPlayed = 0;
                reader.Seek(0);
            }
        }

        public override bool Update()
        {
            if (paused)
                return true;
            int qCount;
            AL.GetSourcei(source, ALGetSourcei.BuffersQueued, out qCount);
            int processedCount;
            AL.GetSourcei(source, ALGetSourcei.BuffersProcessed, out processedCount);

            if (processedCount == 0 && qCount == SampleRate)
                return true;

            int b;
            if (qCount == 0 && playing)
            {
                b = ReadBuffer();
                AL.SourceQueueBuffers(source, 1, new int[] { b });
                AL.SourcePlay(source);
                return true;
            }

            ALSourceState state = (ALSourceState)AL.GetSourcei(source, ALGetSourcei.SourceState);
            if (state == ALSourceState.Stopped && playing)
            {
                AL.SourcePlay(source);
            }

            int[] tempBuffers;
            if (processedCount > 0)
            {
                tempBuffers = new int[processedCount];
                if (shouldStop && qCount == processedCount)
                {
                    AL.SourceStop(source);
                    playing = false;
                    return false;
                }
                AL.SourceUnqueueBuffers(source, processedCount, tempBuffers);

                int bytesPerSample = Channels * (BitsPerSample / 8);
                int samplesPerBuffer = (SampleRate * Channels * (BitsPerSample / 8)) / bytesPerSample;
                totalSamplesPlayed += processedCount * samplesPerBuffer;
            }
            else
                tempBuffers = buffers.Skip(qCount).ToArray();

            int buffersCount = 0;
            for (int j = 0; j < tempBuffers.Length; j++)
            {
                b = ReadBuffer(tempBuffers[j]);
                if (b == -1)
                {
                    shouldStop = true;
                    break;
                }
                buffersCount++;
            }

            if (buffersCount > 0)
            {
                AL.SourceQueueBuffers(source, buffersCount, tempBuffers);
            }

            return true;
        }


        internal int ReadBuffer(int bufferId = 0)
        {
            lock (mutex)
            {
                if (bufferId == 0)
                    bufferId = buffers[0];
                byte[] buffer = new byte[SampleRate * Channels * (BitsPerSample / 8) / 4];
                int bytesRead = reader.Read(buffer, 0, buffer.Length);

                if (bytesRead <= 0)
                {
                    if (IsLoop)
                    {
                        StartOver();
                        bytesRead = reader.Read(buffer, 0, buffer.Length);
                        if (bytesRead <= 0) return -1;
                    }
                    else
                    {
                        return -1;
                    }
                }

                AL.BufferData(bufferId, (SoundManager as SoundManager).GetSoundFormat(Channels, BitsPerSample), buffer, buffer.Length, SampleRate);

                return bufferId;
            }
        }

        public override void Stop()
        {
            lock (mutex)
            {
                if (!playing)
                    return;
                AL.SourceStop(source);
                AL.SourceUnqueueBuffers(source, AL.GetSourcei(source, ALGetSourcei.BuffersQueued), null);
                totalSamplesPlayed = 0;
                reader.Seek(0);
                playing = false;
                paused = false;
            }
        }

        public override void Pause()
        {
            lock (mutex)
            {
                if (!playing)
                    return;
                AL.SourcePause(source);
                paused = true;
            }
        }

        protected override void CleanUnmanaged()
        {
            Console.WriteLine("test");
            AL.DeleteSource(source);
            source = 0;
            AL.DeleteBuffers(buffers.Length, buffers);
            reader.Dispose();
            for (int i = 0; i < buffers.Length; i++)
            {
                buffers[i] = 0;
            }

            buffers = null;

            base.CleanUnmanaged();
        }
    }
}
