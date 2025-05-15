using System.Runtime.InteropServices;
using Strawberry.Graphics;
using Strawberry.Input;
using Strawberry.Misc;
using Strawberry.OpenAL;
using Strawberry.Sound;
using Strawberry.Web.Graphics;
using Strawberry.Web.Helpers;
using Strawberry.Web.Input;

namespace Strawberry.Web;

public class GameLauncher : IGameLauncher
{
    static GameLauncher instance = null;

    public IGraphicsContext GraphicsContext { get; private set; }

    public IInputManager InputManager { get; private set; }

    public ISoundManager SoundManager { get; private set; }

    public IStorage Storage { get; private set; }

    public event Action Initialized;
    public event Action GameLoop;

    string rootUrl = null;

    [UnmanagedCallersOnly]
    public static int JSGameLoop(double time, nint userData)
    {
        if (instance.GraphicsContext != null)
            instance.GameLoop?.Invoke();
        return 1;
    }
    public GameLauncher()
    {
        instance = this;
        Storage = new StorageManager();
        
        SetRootUrl(Interop.RequestRootURL());
        Console.WriteLine("Root URL: " + rootUrl);
    }

    public void Initialize(int width, int height)
    {
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

        GraphicsContext = new GraphicsContext();
        GraphicsContext.Initialize(new EGLDisplayHolder(display, surface), width, height, true);

        SoundManager = new OpenAL.SoundManager();
        InputManager = new InputManager();

        Initialized?.Invoke();
    }

    public void SetRootUrl(string rootUrl)
    {
        if (Storage != null)
            ((StorageManager)Storage).RootUrl = rootUrl;

        this.rootUrl = rootUrl;
    }

    public async Task AOTDownload(string path)
    {
        if (rootUrl == null)
            return;

        await ((StorageManager)Storage).AOTDownload(path);
    }

    public void Run()
    {
        Interop.Initialize();
        unsafe
        {
            Emscripten.RequestAnimationFrameLoop((delegate* unmanaged<double, nint, int>)&JSGameLoop, nint.Zero);
        }
    }
}
