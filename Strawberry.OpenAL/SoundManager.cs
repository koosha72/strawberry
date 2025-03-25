using Strawberry.Sound;

namespace Strawberry.OpenAL
{
    public class SoundManager : Base, ISoundManager
    {
        nint device;
        nint context;

        List<Voice> sources = new List<Voice>();

        public IEnumerable<Voice> Sources { get { return sources; } }

        Thread streamBuffersThread;

        bool streaming = true;

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
            //streamBuffersThread = new Thread(StreamThread);
            //streamBuffersThread.IsBackground = true;
            //streamBuffersThread.Start();
            streams = new List<SoundStream>();
        }

        void StreamThread()
        {
            while (streaming)
            {
                Thread.Sleep(100);
            }
        }


        public ISoundBuffer CreateSoundBuffer(string fileName)
        {
            var bytes = File.ReadAllBytes(fileName);

            return CreateSoundBuffer(bytes);
        }

        public ISoundBuffer CreateSoundBuffer(string fileName, SoundFormat format)
        {
            throw new NotImplementedException();
        }

        public ISoundBuffer CreateSoundBuffer(byte[] data)
        {
            int buffer = AL.GenBuffer();

            int channels, bits_per_sample, sample_rate;
            MemoryStream mem = new MemoryStream(data);
            byte[] sound_data = LoadWave(mem, out channels, out bits_per_sample, out sample_rate);

            AL.BufferData(buffer, GetSoundFormat(channels, bits_per_sample), sound_data, sound_data.Length, sample_rate);

            return new SoundBuffer(buffer, this);
        }

        public byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");


            using (BinaryReader reader = new BinaryReader(stream))
            {
                // RIFF header 
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");


                int riff_chunck_size = reader.ReadInt32();


                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                string format_signature;
                string junk = new string(reader.ReadChars(4));
                if (junk == "JUNK")
                {
                    int size = reader.ReadInt32();
                    reader.ReadBytes(size);
                    format_signature = new string(reader.ReadChars(4));
                    if (format_signature == "bext")
                    {
                        size = reader.ReadInt32();
                        reader.ReadBytes(size);
                        format_signature = new string(reader.ReadChars(4));
                    }
                }
                else
                {
                    format_signature = junk;
                }
                if (format_signature == "bext")
                {
                    int size = reader.ReadInt32();
                    reader.ReadBytes(size);
                    format_signature = new string(reader.ReadChars(4));
                }

                // WAVE header 
                if (format_signature != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");


                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();
                reader.ReadBytes(format_chunk_size - 16);


                string data_signature = new string(reader.ReadChars(4));
                while (data_signature != "data")
                {
                    reader.ReadBytes(reader.ReadInt32());
                    data_signature = new string(reader.ReadChars(4));
                }
                if (data_signature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");


                int data_chunk_size = reader.ReadInt32();


                channels = num_channels;
                bits = bits_per_sample;
                rate = sample_rate;


                return reader.ReadBytes(data_chunk_size);
            }
        }

        public ISoundStream CreateStream(Stream stream)
        {
            throw new NotImplementedException();
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

        private ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
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
