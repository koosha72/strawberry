using Strawberry.Sound;

namespace Strawberry.OpenAL
{
    public class SoundStream : Base, ISoundStream
    {
        public ISoundManager SoundManager { get; private set; }

        public int BitsPerSample { get; private set; }

        public int SampleRate { get; private set; }

        public int Channels { get; private set; }

        public bool IsLoop { get; private set; }


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
        }

        public void Load(string path)
        {

        }

        public void Load(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Load(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void Unload()
        {
            throw new NotImplementedException();
        }

        public bool IsStreaming()
        {
            return playing;
        }

        public void Play(bool loop = false)
        {
            lock (mutex)
            {
                if (playing)
                    return;
                StartOver();
                playing = true;
                IsLoop = loop;
            }
        }

        private void StartOver()
        {
            lock (mutex)
            {
                reader.Seek(0);
                BitsPerSample = reader.Channels;
                SampleRate = reader.SampleRate;
                Channels = reader.Channels;
            }
        }

        public bool Update()
        {
            int qCount;
            AL.GetSourcei(source, ALGetSourcei.BuffersQueued, out qCount);
            int processedCount;
            AL.GetSourcei(source, ALGetSourcei.BuffersProcessed, out processedCount);

            if (processedCount == 0 && qCount == SampleRate)
                return true;

            int b;
            if (processedCount == 0 && qCount == 0 && playing)
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
                AL.SourceUnqueueBuffers(source, processedCount, tempBuffers);
            }
            else
                tempBuffers = buffers.Skip(qCount).ToArray();

            for (int j = 0; j < tempBuffers.Length; j++)
            {
                b = ReadBuffer(tempBuffers[j]);
                if (b == -1)
                {
                    if (!IsLoop)
                    {
                        AL.SourceStop(source);
                        return false;
                    }
                    else
                        StartOver();

                    break;
                }
            }

            AL.SourceQueueBuffers(source, tempBuffers.Length, tempBuffers);

            return true;
        }

        internal int ReadBuffer(int bId = 0)
        {
            lock (mutex)
            {
                if (bId == 0)
                    bId = buffers[0];
                byte[] buffer = new byte[SampleRate];
                int s = reader.Read(buffer, 0, SampleRate);


                AL.BufferData(bId, (SoundManager as SoundManager).GetSoundFormat(Channels, BitsPerSample), buffer, s, SampleRate);

                if (s != SampleRate)
                {
                    if (!IsLoop)
                        playing = false;
                    return -1;
                }

                return bId;
            }
        }
    }
}
