using System;
using System.Diagnostics;
using System.Threading;
using Android.Views;
using Strawberry.Android.Graphics;
using Strawberry.Android.Helpers;
using Strawberry.Android.Input;
using Strawberry.Graphics;
using Strawberry.Input;
using Strawberry.Sound;
using Strawberry.OpenAL;
using Activity = Android.App.Activity;
using Strawberry.Misc;

namespace Strawberry.Android;

public class GameLauncher : Activity, IGameLauncher
{
    EGLHelper eglHelper;
    public IGraphicsContext GraphicsContext { get; private set; }

    public IInputManager InputManager { get; private set; }

    public ISoundManager SoundManager { get; private set; }

    public IStorage Storage { get; private set; }

    public event Action Initialized;
    public event Action GameLoop;

    Thread gameLoopThread;

    StrawberrySurfaceView surfaceView;

    // Signals between UI thread and game loop thread
    volatile bool running = true;
    volatile bool surfaceAvailable = false;
    volatile bool needsEGLSetup = false;
    bool firstStart = true;
    bool isFinishing = false;
    int w, h;
    object mutex = new object();

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
    }

    public void Initialize(int width, int height)
    {
        w = width;
        h = height;

        eglHelper = new EGLHelper();

        surfaceView = new StrawberrySurfaceView(this);
        surfaceView.OnSurfaceCreated += surfaceView_SurfaceCreated;
        surfaceView.OnSurfaceDestroyed += surfaceView_SurfaceDestroyed;
        SetContentView(surfaceView);

        // Start the game loop thread ONCE — it lives for the lifetime of the activity
        gameLoopThread = new Thread(GameLoopThread);
        gameLoopThread.Start();
    }

    protected override void OnPause()
    {
        if (SoundManager != null)
        {
            (SoundManager as SoundManager).Suspend();
        }
        base.OnPause();
    }

    protected override void OnResume()
    {
        if (SoundManager != null)
        {
            (SoundManager as SoundManager).RestoreState();
        }
        base.OnResume();
    }

    public override void Finish()
    {
        isFinishing = true;
        base.Finish();
    }

    protected override void OnDestroy()
    {
        isFinishing = true;
        running = false;

        if (gameLoopThread != null && gameLoopThread.IsAlive)
        {
            gameLoopThread.Join(2000);
        }

        base.OnDestroy();
    }

    // ── UI Thread Callbacks ──────────────────────────────────────
    // These do NOT touch EGL. They just signal the game loop thread.

    private void surfaceView_SurfaceCreated(ISurfaceHolder holder)
    {
        // Store the new surface for the game loop thread to consume
        eglHelper.NotifySurfaceCreated(holder.Surface);
        lock (mutex)
        {
            surfaceAvailable = true;
            needsEGLSetup = true;
        }
    }

    private void surfaceView_SurfaceDestroyed(ISurfaceHolder holder)
    {
        lock (mutex)
        {
            // Signal that the surface is gone
            surfaceAvailable = false;

            // Tell the EGL helper (doesn't touch EGL, just clears flag)
            eglHelper.NotifySurfaceDestroyed();
        }

        // Give the game loop thread time to stop rendering
        // so Android can safely destroy the surface
        // Thread.Sleep(100);
    }

    // ── Game Loop Thread ─────────────────────────────────────────
    // ALL EGL operations happen here. Never on the UI thread.

    private void GameLoopThread(object obj)
    {
        bool eglReady = false;

        while (running)
        {
            // ── Surface is gone — release EGL and wait ──
            if (!surfaceAvailable)
            {
                if (eglReady)
                {
                    // Release the EGL surface (keeps context alive)
                    eglHelper.ReleaseSurface();
                    eglReady = false;
                }

                Thread.Sleep(16);
                continue;
            }

            // ── Surface is available but EGL not set up yet ──
            if (needsEGLSetup)
            {
                needsEGLSetup = false;

                if (eglHelper.HasContext)
                {
                    // Context might have survived — try to bind the new surface
                    if (eglHelper.BindSurface())
                    {
                        // Context survived — textures are still in GPU memory!
                        // Just reinitialize the graphics context (viewport etc.)
                        eglHelper.MakeCurrent();
                        GraphicsContext.Initialize(eglHelper, w, h, true);
                        eglReady = true;
                        continue;
                    }
                    else
                    {
                        // Context was lost — full rebuild needed
                        eglHelper.CleanUp();
                    }
                }

                // No context — full initialization
                eglHelper.Init(eglHelper.ConsumePendingSurface());
                eglHelper.MakeCurrent();

                if (firstStart)
                {
                    GraphicsContext = new GraphicsContext();
                    GraphicsContext.Initialize(eglHelper, w, h, true);
                    InputManager = new InputManager();
                    SoundManager = new OpenAL.SoundManager();
                    Storage = new StorageManager();
                    Initialized?.Invoke();
                    firstStart = false;
                }
                else
                {
                    GraphicsContext.Initialize(eglHelper, w, h, true);
                    (GraphicsContext as GraphicsContext).RestoreContext();
                }

                eglReady = true;
            }

            // ── Normal frame ──
            lock (mutex)
            {
                if (eglReady && GraphicsContext != null && surfaceAvailable)
                {
                    GameLoop?.Invoke();
                }
            }
        }

        // Thread is exiting — clean up EGL
        eglHelper.CleanUp();
    }

    public void Run()
    {
    }

    public override bool OnTouchEvent(MotionEvent e)
    {
        (this.InputManager.PointingDevice as PointingDevice).OnTouch(surfaceView, e);
        return base.OnTouchEvent(e);
    }
}
