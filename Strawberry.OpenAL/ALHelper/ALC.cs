using System.Runtime.InteropServices;

namespace Strawberry.OpenAL;

internal class ALC
{
    internal const string Lib = "libopenal";

    // Context management
    [DllImport(Lib, EntryPoint = "alcOpenDevice" , CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr OpenDevice(string deviceName);

    [DllImport(Lib, EntryPoint = "alcCloseDevice" , CallingConvention = CallingConvention.Cdecl)]
    public static extern bool CloseDevice(IntPtr device);

    [DllImport(Lib, EntryPoint = "alcCreateContext" , CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreateContext(IntPtr device, int[] attrList);

    [DllImport(Lib, EntryPoint = "alcMakeContextCurrent" , CallingConvention = CallingConvention.Cdecl)]
    public static extern bool MakeContextCurrent(IntPtr context);

    [DllImport(Lib, EntryPoint = "alcDestroyContext" , CallingConvention = CallingConvention.Cdecl)]
    public static extern void DestroyContext(IntPtr context);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport(Lib, EntryPoint = "alcGetContextsDevice" , CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetContextsDevice(IntPtr context);
}