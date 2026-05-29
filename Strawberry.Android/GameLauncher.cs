using System;
using System.Diagnostics;
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

    Thread gameLoop;

    StrawberrySurfaceView surfaceView;

    bool running = true;
    bool firstStart = true;
    int w, h;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
    }

    public void Initialize(int width, int height)
    {
        w = width;
        h = height;

        surfaceView = new StrawberrySurfaceView(this);
        surfaceView.OnSurfaceCreated += surfaceView_SurfaceCreated;
        surfaceView.OnSurfaceDestroyed += surfaceView_SurfaceDestroyed;
        SetContentView(surfaceView);

        gameLoop = new Thread(GameLoopThread);
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

    private void surfaceView_SurfaceCreated(ISurfaceHolder holder)
    {
        eglHelper = new EGLHelper();
        eglHelper.Init(holder.Surface);
        if (!running && gameLoop.ThreadState == System.Threading.ThreadState.Stopped)
        {
            gameLoop = new Thread(GameLoopThread);
            running = true;
        }
        gameLoop.Start();
    }

    private void surfaceView_SurfaceDestroyed(ISurfaceHolder holder)
    {
        running = false;
        try
        {
            gameLoop.Join();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.StackTrace);
        }
        eglHelper.CleanUp();
    }

    private void GameLoopThread(object obj)
    {
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
        }

        while (running)
        {
            if (GraphicsContext != null)
                GameLoop?.Invoke();
        }
    }

    public void Run()
    {
        //gameLoop.Start();
    }

    public override bool OnTouchEvent(MotionEvent e)
    {
        (this.InputManager.PointingDevice as PointingDevice).OnTouch(surfaceView, e);

        return base.OnTouchEvent(e);
    }
}
