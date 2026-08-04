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
using Strawberry.Platform;
using System.Collections.Concurrent;
using Android.Runtime;
using Android.Graphics;

namespace Strawberry.Android;

/// <summary>
/// The game launcher for android platform. It represents the main activity.
/// </summary>
public class GameLauncher : Activity, IGameLauncher
{
    EGLHelper eglHelper;
    public IGraphicsContext GraphicsContext { get; private set; }

    public IInputManager InputManager { get; private set; }

    public ISoundManager SoundManager { get; private set; }

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

    private readonly ConcurrentQueue<bool> pendingFocusEvents = new();
    private readonly ConcurrentQueue<(int w, int h)> pendingResizeEvents = new();

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
        surfaceView.OnSurfaceChanged += surfaceView_SurfaceChanged;
        SetContentView(surfaceView);

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

    public override void OnWindowFocusChanged(bool hasFocus)
    {
        pendingFocusEvents.Enqueue(hasFocus);
        base.OnWindowFocusChanged(hasFocus);
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
        eglHelper.NotifySurfaceCreated(holder.Surface);
        surfaceAvailable = true;
        needsEGLSetup = true;
    }

    private void surfaceView_SurfaceDestroyed(ISurfaceHolder holder)
    {
        surfaceAvailable = false;
        eglHelper.NotifySurfaceDestroyed();
    }

    private void surfaceView_SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
    {
        pendingResizeEvents.Enqueue((width, height));
    }

    // ── Game Loop Thread ─────────────────────────────────────────
    // ALL EGL operations happen here. Never on the UI thread.

    private void GameLoopThread(object obj)
    {
        bool eglReady = false;

        while (running)
        {
            // Call focus callbacks for the latest focus changes on the game loop thread
            while (pendingFocusEvents.TryDequeue(out bool hasFocus))
            {
                if (hasFocus)
                    Game.Instance?.GameContext?.OnFocusGained();
                else
                    Game.Instance?.GameContext?.OnFocusLost();
            }

            // Call resize callbacks for the latest focus changes on the game loop thread
            while (pendingResizeEvents.TryDequeue(out var r))
            {
                w = r.w;
                h = r.h;
                Game.Instance?.GameContext?.OnResized(new Math.Vector2(r.w, r.h));
            }

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
                        GraphicsContext.Initialize(eglHelper, w, h);
                        eglReady = true;
                        continue;
                    }
                    else
                    {
                        Game.Instance?.GameContext?.OnGraphicsContextLost();
                        eglHelper.CleanUp();
                    }
                }

                // No context — full initialization
                eglHelper.Init(eglHelper.ConsumePendingSurface());
                eglHelper.MakeCurrent();

                if (firstStart)
                {
                    GraphicsContext = new GraphicsContext();
                    GraphicsContext.Initialize(eglHelper, w, h);
                    InputManager = new InputManager();
                    SoundManager = new OpenAL.SoundManager();
                    PlatformServices.RegisterService<IAssetStorage>(new AssetStorageManager());
                    PlatformServices.RegisterService<IUserDataStorage>(new UserDataStorage());
                    Initialized?.Invoke();
                    firstStart = false;
                }
                else
                {
                    GraphicsContext.Initialize(eglHelper, w, h);
                    (GraphicsContext as GraphicsContext).RestoreContext();
                    Game.Instance?.GameContext?.OnGraphicsContextRestored();
                }

                eglReady = true;
            }


            if (eglReady && GraphicsContext != null && surfaceAvailable)
            {
                GameLoop?.Invoke();
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
        if (InputManager?.PointingDevice is PointingDevice pd)
            pd.OnTouch(surfaceView, e);
        return base.OnTouchEvent(e);
    }

    public void Exit()
    {
        Finish();
    }
}
