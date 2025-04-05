using System.Runtime.InteropServices;
using Strawberry.Math;

namespace Strawberry.OpenAL;

public class AL
{
    [DllImport(ALC.Lib, EntryPoint = "alListener3f")]
    public static extern void Listener3f(ALListener3f param, float value1, float value2, float value3);

    [DllImport(ALC.Lib, EntryPoint = "alListenerfv")]
    public static extern void Listenerfv(ALListenerfv param, float[] values);

    // Source management
    [DllImport(ALC.Lib, EntryPoint = "alGenSources")]
    public static extern void GenSources(int n, int[] sources);

    [DllImport(ALC.Lib, EntryPoint = "alDeleteSources")]
    public static extern void DeleteSources(int n, int[] sources);

    [DllImport(ALC.Lib, EntryPoint = "alSourcef")]
    public static extern void Sourcef(int source, ALSourcef param, float value);

    [DllImport(ALC.Lib, EntryPoint = "alSource3f")]
    public static extern void Source3f(int source, ALSource3f param, float v1, float v2, float v3);

    [DllImport(ALC.Lib, EntryPoint = "alSourcei")]
    public static extern void Sourcei(int source, ALSourcei param, int value);

    [DllImport(ALC.Lib, EntryPoint = "alSourcei")] // Using alSourcei since there's no alSourceb in OpenAL
    public static extern void Sourceb(int source, ALSourceb param, bool value);

    [DllImport(ALC.Lib, EntryPoint = "alSourcePlay")]
    public static extern void SourcePlay(int source);

    [DllImport(ALC.Lib, EntryPoint = "alSourceStop")]
    public static extern void SourceStop(int source);

    [DllImport(ALC.Lib, EntryPoint = "alSourcePause")]
    public static extern void SourcePause(int source);

    [DllImport(ALC.Lib, EntryPoint = "alSourceQueueBuffers")]
    public static extern void SourceQueueBuffers(int source, int numEntries, int[] buffers);

    [DllImport(ALC.Lib, EntryPoint = "alSourceUnqueueBuffers")]
    public static extern void SourceUnqueueBuffers(int source, int numEntries, int[] buffers);

    [DllImport(ALC.Lib, EntryPoint = "alGetSourcei")]
    public static extern ALSourceState GetSourcei(int source, ALGetSourcei param, out int result);

    [DllImport(ALC.Lib, EntryPoint = "alGetSourcef")]
    public static extern ALSourceState GetSourcef(int source, ALSourcef param, out float result);

    [DllImport(ALC.Lib, EntryPoint = "alIsSource")]
    public static extern bool IsSource(int source);

    // Buffer management
    [DllImport(ALC.Lib, EntryPoint = "alGenBuffers")]
    public static extern void GenBuffers(int n, int[] buffers);

    [DllImport(ALC.Lib, EntryPoint = "alDeleteBuffers")]
    public static extern void DeleteBuffers(int n, int[] buffers);

    [DllImport(ALC.Lib, EntryPoint = "alBufferData")]
    public static extern void BufferData(int buffer, ALFormat format, IntPtr data, int size, int freq);

    [DllImport(ALC.Lib, EntryPoint = "alGetBufferi")]
    public static extern ALBufferState GetBufferi(int id, ALGetBufferi param, out int result);

    [DllImport(ALC.Lib, EntryPoint = "alDistanceModel")]
    public static extern void DistanceModel(ALDistanceModel distancemodel);

    // Error handling
    [DllImport(ALC.Lib, EntryPoint = "alGetError")]
    public static extern ALError GetError();

    // Extension for buffer data with byte[]
    public static void BufferData(int buffer, ALFormat format, byte[] data, int size, int freq)
    {
        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(data, 0, ptr, size);
        BufferData(buffer, format, ptr, size, freq);
        Marshal.FreeHGlobal(ptr);
    }

    // Extension for buffer data with short[]
    public static void BufferData(int buffer, ALFormat format, short[] data, int size, int freq)
    {
        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(data, 0, ptr, data.Length);
        BufferData(buffer, format, ptr, size, freq);
        Marshal.FreeHGlobal(ptr);
    }

    public static int GenBuffer()
    {
        int[] buffers = new int[1];
        GenBuffers(1, buffers);
        return buffers[0];
    }

    public static int GetBufferi(int id, ALGetBufferi param)
    {
        int result = 0;
        GetBufferi(id, param, out result);
        return result;
    }

    public static int GetSourcei(int id, ALGetSourcei param)
    {
        int result = 0;
        GetSourcei(id, param, out result);
        return result;
    }

    public static float GetSourcef(int id, ALSourcef param)
    {
        float result = 0;
        GetSourcef(id, param, out result);
        return result;
    }

    public static int GenSource()
    {
        int[] sources = new int[1];
        GenSources(1, sources);
        return sources[0];
    }

    public static void DeleteBuffer(int buffer)
    {
        DeleteBuffers(1, new int[] { buffer });
    }

    public static void DeleteSource(int source)
    {
        DeleteSources(1, new int[] { source });
    }

    public static void Listenerfv(ALListenerfv param, ref Vector3 at, ref Vector3 up)
    {
        float[] temp = new float[6];

        temp[0] = at.X;
        temp[1] = at.Y;
        temp[2] = at.Z;

        temp[3] = up.X;
        temp[4] = up.Y;
        temp[5] = up.Z;

        Listenerfv(param, temp);
    }
}