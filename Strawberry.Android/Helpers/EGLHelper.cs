using Android.Opengl;
using Android.Views;
using Strawberry.Math;

namespace Strawberry.Android.Helpers;

public class EGLHelper
{
    private EGLDisplay eglDisplay;
    private EGLContext eglContext;
    private EGLSurface eglSurface;
    private EGLConfig eglConfig;
    private bool initialized = false;
    private bool contextPreserved = false;

    // Pending surface from surfaceCreated — consumed by the game loop thread
    private Surface pendingSurface;
    private bool surfaceReady = false;

    public bool HasContext => eglContext != null && eglContext != EGL14.EglNoContext;
    public bool HasDisplay => eglDisplay != null && eglDisplay != EGL14.EglNoDisplay;
    public bool IsInitialized => initialized;

    /// <summary>
    /// True if the context survived the last surface destruction.
    /// Check this after BindSurface to know if GPU resources need restoration.
    /// </summary>
    public bool ContextWasPreserved => contextPreserved;

    /// <summary>
    /// Full initialization — creates display, config, context, and surface.
    /// Call this on the game loop thread only.
    /// </summary>
    public void Init(Surface surface)
    {
        eglDisplay = EGL14.EglGetDisplay(EGL14.EglDefaultDisplay);
        int[] version = new int[2];
        EGL14.EglInitialize(eglDisplay, version, 0, version, 1);

        int[] configAttribs = {
                EGL14.EglRedSize, 8,
                EGL14.EglGreenSize, 8,
                EGL14.EglBlueSize, 8,
                EGL14.EglAlphaSize, 8,
                EGL14.EglDepthSize, 16,
                EGL14.EglRenderableType, EGL14.EglOpenglEs2Bit,
                EGL14.EglNone
        };
        EGLConfig[] configs = new EGLConfig[1];
        int[] numConfigs = new int[1];
        EGL14.EglChooseConfig(eglDisplay, configAttribs, 0, configs, 0, 1, numConfigs, 0);
        eglConfig = configs[0];

        int[] contextAttribs = {
                EGL14.EglContextClientVersion, 3,
                EGL14.EglNone
        };
        eglContext = EGL14.EglCreateContext(eglDisplay, eglConfig, EGL14.EglNoContext, contextAttribs, 0);

        int[] surfaceAttribs = {
                EGL14.EglNone
        };
        eglSurface = EGL14.EglCreateWindowSurface(eglDisplay, eglConfig, surface, surfaceAttribs, 0);

        initialized = true;
        contextPreserved = false;  // Fresh context — resources must be created
    }

    public void MakeCurrent()
    {
        EGL14.EglMakeCurrent(eglDisplay, eglSurface, eglSurface, eglContext);
    }

    public void SwapBuffers()
    {
        EGL14.EglSwapBuffers(eglDisplay, eglSurface);
    }

    /// <summary>
    /// Called from the UI thread's surfaceDestroyed.
    /// Does NOT touch EGL — just sets a flag for the game loop thread.
    /// The game loop thread will see running=false and stop itself.
    /// </summary>
    public void NotifySurfaceDestroyed()
    {
        surfaceReady = false;
    }

    /// <summary>
    /// Called from the UI thread's surfaceCreated.
    /// Stores the new surface for the game loop thread to consume.
    /// Does NOT touch EGL — no MakeCurrent on the UI thread.
    /// </summary>
    public void NotifySurfaceCreated(Surface surface)
    {
        pendingSurface = surface;
        surfaceReady = true;
    }

    /// <summary>
    /// Returns the pending surface and clears it.
    /// Called on the game loop thread when doing a full Init().
    /// </summary>
    public Surface ConsumePendingSurface()
    {
        var surface = pendingSurface;
        pendingSurface = null;
        return surface;
    }

    /// <summary>
    /// Called on the game loop thread to bind the pending surface to the existing context.
    /// Returns true if context survived and surface was bound.
    /// Returns false if context was lost — you must call Init() + restore resources.
    /// </summary>
    public bool BindSurface()
    {
        if (pendingSurface == null)
            return false;

        if (!HasContext)
            return false;

        // Release any old surface first
        if (eglSurface != null && eglSurface != EGL14.EglNoSurface)
        {
            // Make sure nothing is current before destroying surface
            EGL14.EglMakeCurrent(eglDisplay, EGL14.EglNoSurface, EGL14.EglNoSurface, EGL14.EglNoContext);
            EGL14.EglDestroySurface(eglDisplay, eglSurface);
            eglSurface = EGL14.EglNoSurface;
        }

        // Create new surface from the pending surface
        int[] surfaceAttribs = { EGL14.EglNone };
        eglSurface = EGL14.EglCreateWindowSurface(eglDisplay, eglConfig, pendingSurface, surfaceAttribs, 0);

        if (eglSurface == null || eglSurface == EGL14.EglNoSurface)
        {
            contextPreserved = false;
            return false;
        }

        // Make context current on THIS thread (game loop thread)
        if (!EGL14.EglMakeCurrent(eglDisplay, eglSurface, eglSurface, eglContext))
        {
            int error = EGL14.EglGetError();
            // Context is dead
            EGL14.EglDestroySurface(eglDisplay, eglSurface);
            eglSurface = EGL14.EglNoSurface;
            contextPreserved = false;
            return false;
        }

        pendingSurface = null;
        contextPreserved = true;  // Context survived — GPU resources still intact
        return true;
    }

    /// <summary>
    /// Releases the surface on the game loop thread.
    /// Call this when the game loop thread detects surface destruction.
    /// </summary>
    public void ReleaseSurface()
    {
        if (!initialized) return;

        EGL14.EglMakeCurrent(eglDisplay, EGL14.EglNoSurface, EGL14.EglNoSurface, EGL14.EglNoContext);

        if (eglSurface != null && eglSurface != EGL14.EglNoSurface)
        {
            EGL14.EglDestroySurface(eglDisplay, eglSurface);
            eglSurface = EGL14.EglNoSurface;
        }
    }

    /// <summary>
    /// Full cleanup — destroys surface, context, and display.
    /// Only call this on the game loop thread when the activity is truly finishing.
    /// </summary>
    public void CleanUp()
    {
        if (!initialized) return;

        EGL14.EglMakeCurrent(eglDisplay, EGL14.EglNoSurface, EGL14.EglNoSurface, EGL14.EglNoContext);

        if (eglSurface != null && eglSurface != EGL14.EglNoSurface)
        {
            EGL14.EglDestroySurface(eglDisplay, eglSurface);
            eglSurface = EGL14.EglNoSurface;
        }
        if (eglContext != null && eglContext != EGL14.EglNoContext)
        {
            EGL14.EglDestroyContext(eglDisplay, eglContext);
            eglContext = EGL14.EglNoContext;
        }
        if (eglDisplay != null && eglDisplay != EGL14.EglNoDisplay)
        {
            EGL14.EglTerminate(eglDisplay);
            eglDisplay = EGL14.EglNoDisplay;
        }

        initialized = false;
        contextPreserved = false;
    }

    public Vector2 GetScreenSize()
    {
        int[] w = new int[1];
        EGL14.EglQuerySurface(eglDisplay, eglSurface, EGL14.EglWidth, w, 0);
        int[] h = new int[1];
        EGL14.EglQuerySurface(eglDisplay, eglSurface, EGL14.EglHeight, h, 0);

        return new Vector2(w[0], h[0]);
    }
}
