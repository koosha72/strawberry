using System;
using System.Runtime.InteropServices;
using Strawberry.Web.Helpers;
using Silk.NET.OpenGLES;

namespace Strawberry.Web;

public class GameLauncher
{
    static GameLauncher instance = null;
    GL gl;

    [UnmanagedCallersOnly]
    public static int GameLoop(double time, nint userData)
    {
        instance.GameLoop();
        return 1;
    }

    public GameLauncher()
    {
        instance = this;

        var display = EGL.GetDisplay(IntPtr.Zero);
        if (display == IntPtr.Zero)
            throw new Exception("Display was null");

        if (!EGL.Initialize(display, out int major, out int minor))
            throw new Exception("Initialize() returned false.");

        int[] attributeList = new int[]
        {
            EGL.EGL_RED_SIZE  , 8,
            EGL.EGL_GREEN_SIZE, 8,
            EGL.EGL_BLUE_SIZE , 8,
            EGL.EGL_DEPTH_SIZE, 24,
            EGL.EGL_STENCIL_SIZE, 8,
            EGL.EGL_SURFACE_TYPE, EGL.EGL_WINDOW_BIT,
            EGL.EGL_RENDERABLE_TYPE, EGL.EGL_OPENGL_ES3_BIT,
            EGL.EGL_SAMPLES, 16, //MSAA, 16 samples
			EGL.EGL_NONE
        };

        var config = IntPtr.Zero;
        var numConfig = IntPtr.Zero;
        if (!EGL.ChooseConfig(display, attributeList, ref config, (IntPtr)1, ref numConfig))
            throw new Exception("ChoseConfig() failed");
        if (numConfig == IntPtr.Zero)
            throw new Exception("ChoseConfig() returned no configs");

        if (!EGL.BindApi(EGL.EGL_OPENGL_ES_API))
            throw new Exception("BindApi() failed");

        int[] ctxAttribs = new int[] { EGL.EGL_CONTEXT_CLIENT_VERSION, 3, EGL.EGL_NONE };
        var context = EGL.CreateContext(display, config, (IntPtr)EGL.EGL_NO_CONTEXT, ctxAttribs);
        if (context == IntPtr.Zero)
            throw new Exception("CreateContext() failed");

        // now create the surface
        var surface = EGL.CreateWindowSurface(display, config, IntPtr.Zero, IntPtr.Zero);
        if (surface == IntPtr.Zero)
            throw new Exception("CreateWindowSurface() failed");

        if (!EGL.MakeCurrent(display, surface, surface, context))
            throw new Exception("MakeCurrent() failed");

        TrampolineFuncs.ApplyWorkaroundFixingInvocations();

        gl = GL.GetApi(EGL.GetProcAddress);
        gl.Viewport(0, 0, 1280, 720);


        unsafe
        {
            Emscripten.RequestAnimationFrameLoop((delegate* unmanaged<double, nint, int>)&GameLoop, nint.Zero);
        }
    }

    public void GameLoop()
    {
        Console.WriteLine("Clearing");
        gl.ClearColor(0.392f, 0.584f, 0.929f, 1.0f);
        gl.Clear(ClearBufferMask.ColorBufferBit);
    }
}
