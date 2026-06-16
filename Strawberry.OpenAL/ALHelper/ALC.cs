using System.Runtime.InteropServices;

namespace Strawberry.OpenAL;

internal class ALC
{
    internal const string Lib = "libopenal";

    static ALC()
    {
        if (RuntimeInformation.RuntimeIdentifier != "browser-wasm")
        {
            NativeLibrary.SetDllImportResolver(typeof(ALC).Assembly, ResolveOpenAL);
        }
    }

    private static IntPtr ResolveOpenAL(string libraryName, System.Reflection.Assembly assembly, DllImportSearchPath? searchPath)
    {
        // Intercept the request when it asks for our const string "libopenal"
        if (libraryName == Lib)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return NativeLibrary.Load("openal.dll", typeof(object).Assembly, searchPath);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return NativeLibrary.Load("libopenal.so.1", typeof(object).Assembly, searchPath);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return NativeLibrary.Load("libopenal.1.dylib", typeof(object).Assembly, searchPath);
        }

        // For anything else, bypass our resolver to prevent infinite loops 
        // by delegating to the default system loader.
        return NativeLibrary.Load("libopenal.so", typeof(object).Assembly, searchPath);
    }

    // Context management
    [DllImport(Lib, EntryPoint = "alcOpenDevice", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr OpenDevice(string deviceName);

    [DllImport(Lib, EntryPoint = "alcCloseDevice", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool CloseDevice(IntPtr device);

    [DllImport(Lib, EntryPoint = "alcCreateContext", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreateContext(IntPtr device, int[] attrList);

    [DllImport(Lib, EntryPoint = "alcMakeContextCurrent", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool MakeContextCurrent(IntPtr context);

    [DllImport(Lib, EntryPoint = "alcDestroyContext", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DestroyContext(IntPtr context);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport(Lib, EntryPoint = "alcGetContextsDevice", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetContextsDevice(IntPtr context);
}