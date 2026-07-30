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

                int currentSampleOffset = AL.GetSourcei(SourceInd, ALGetSourcei.SampleOffset);

                return (float)(totalSamplesPlayed + currentSampleOffset) / SampleRate;
            }
            set
            {
                if (reader.DataSize == 0)
                    return;

                int targetSample = (int)(value * SampleRate);
                long targetBytePos = targetSample * Channels * (BitsPerSample / 8);
                totalSamplesPlayed = targetSample;

                AL.SourceStop(SourceInd);
                AL.SourceUnqueueBuffers(SourceInd, AL.GetSourcei(SourceInd, ALGetSourcei.BuffersQueued), null);

                reader.Seek(targetBytePos);

                Update();
            }
        }

        public override float Volume
        {
            get
            {
                return AL.GetSourcef(SourceInd, ALSourcef.Gain);
            }
            set
            {
                AL.Sourcef(SourceInd, ALSourcef.Gain, value);
            }
        }


        int bufferCount = 3;
        int[] buffers;

        public int SourceInd { get; private set; }

        ISoundReader reader;

        bool playing = false;

        object mutex = new object();

        byte[] buffer;

        ALFormat alFormat;

        public SoundStream(SoundManager soundManager, ISoundReader reader, int source)
        {
            buffers = new int[bufferCount];
            AL.GenBuffers(bufferCount, buffers);
            SourceInd = source;
            SoundManager = soundManager;
            this.reader = reader;

            BitsPerSample = reader.BitsPerSample;
            SampleRate = reader.SampleRate;
            Channels = reader.Channels;
            buffer = new byte[SampleRate * Channels * (BitsPerSample / 8) / 4];

            alFormat = (SoundManager as SoundManager).GetSoundFormat(Channels, BitsPerSample);
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
            //lock (mutex)
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
            AL.SourcePlay(SourceInd);
            paused = false;
            return;
        }

        private void StartOver()
        {
            //lock (mutex)
            {
                totalSamplesPlayed = 0;
                reader.Seek(0);
            }
        }

        public override bool Update()
        {
            if (paused)
                return true;

            int processedCount;
            AL.GetSourcei(SourceInd, ALGetSourcei.BuffersProcessed, out processedCount);
            int qCount = AL.GetSourcei(SourceInd, ALGetSourcei.BuffersQueued);

            if (qCount == 0 && playing)
            {
                for (int i = 0; i < buffers.Length; i++)
                {
                    if (ReadBuffer(buffers[i]) == -1)
                    {
                        shouldStop = true;
                        break;
                    }
                }
                AL.SourceQueueBuffers(SourceInd, buffers.Length, buffers);
                AL.SourcePlay(SourceInd);
                return true;
            }

            var ss = (ALSourceState)AL.GetSourcei(SourceInd, ALGetSourcei.SourceState);

            if (ss == ALSourceState.Stopped && qCount > 0)
            {
                AL.SourcePlay(SourceInd);
                return true;
            }


            if (processedCount == 0)
            {
                ALSourceState state = (ALSourceState)AL.GetSourcei(SourceInd, ALGetSourcei.SourceState);
                if (state == ALSourceState.Stopped && playing)
                {
                    AL.SourcePlay(SourceInd);
                }
                return true;
            }

            int[] tempBuffers = new int[processedCount];

            if (shouldStop)
            {
                AL.SourceUnqueueBuffers(SourceInd, processedCount, tempBuffers);
                AL.SourceStop(SourceInd);
                playing = false;
                return false;
            }

            AL.SourceUnqueueBuffers(SourceInd, processedCount, tempBuffers);

            int buffersCount = 0;
            for (int j = 0; j < tempBuffers.Length; j++)
            {
                if (ReadBuffer(tempBuffers[j]) == -1)
                {
                    shouldStop = true;
                    break;
                }
                buffersCount++;
            }

            if (buffersCount > 0)
            {
                AL.SourceQueueBuffers(SourceInd, buffersCount, tempBuffers);
            }

            return true;
        }


        internal int ReadBuffer(int bufferId = 0)
        {
            //lock (mutex)
            {
                if (bufferId == 0)
                    bufferId = buffers[0];
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

                AL.BufferData(bufferId, alFormat, buffer, bytesRead, SampleRate);

                return bufferId;
            }
        }

        public override void Stop()
        {
            //lock (mutex)
            {
                if (!playing)
                    return;
                AL.SourceStop(SourceInd);
                AL.SourceUnqueueBuffers(SourceInd, AL.GetSourcei(SourceInd, ALGetSourcei.BuffersQueued), null);
                totalSamplesPlayed = 0;
                reader.Seek(0);
                playing = false;
                paused = false;
            }
        }

        public override void Pause()
        {
            //lock (mutex)
            {
                if (!playing)
                    return;
                AL.SourcePause(SourceInd);
                paused = true;
            }
        }

        protected override void CleanUnmanaged()
        {
            if (SourceInd != -1)
            {
                (SoundManager as SoundManager)?.ReturnStreamingSource(SourceInd);
                (SoundManager as SoundManager)?.RemoveStream(this);
            }
            SourceInd = -1;
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
