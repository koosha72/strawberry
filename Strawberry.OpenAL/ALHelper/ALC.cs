using System.Runtime.InteropServices;

namespace Strawberry.OpenAL;

public class ALC
{
    internal const string Lib = "libopenal";

    // Context management
    [DllImport(Lib, EntryPoint = "alcOpenDevice")]
    public static extern IntPtr OpenDevice(string deviceName);

    [DllImport(Lib, EntryPoint = "alcCloseDevice")]
    public static extern bool CloseDevice(IntPtr device);

    [DllImport(Lib, EntryPoint = "alcCreateContext")]
    public static extern IntPtr CreateContext(IntPtr device, int[] attrList);

    [DllImport(Lib, EntryPoint = "alcMakeContextCurrent")]
    public static extern bool MakeContextCurrent(IntPtr context);

    [DllImport(Lib, EntryPoint = "alcDestroyContext")]
    public static extern void DestroyContext(IntPtr context);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport(Lib, EntryPoint = "alcGetContextsDevice")]
    public static extern IntPtr GetContextsDevice(IntPtr context);
}