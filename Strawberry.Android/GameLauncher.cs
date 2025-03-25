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

    bool running = true;
    int w, h;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
    }

    public void Initialize(int width, int height)
    {
        w = width;
        h = height;

        StrawberrySurfaceView surfaceView = new StrawberrySurfaceView(this);
        surfaceView.OnSurfaceCreated += surfaceView_SurfaceCreated;
        surfaceView.OnSurfaceDestroyed += surfaceView_SurfaceDestroyed;
        SetContentView(surfaceView);

        gameLoop = new Thread(GameLoopThread);
    }

    private void surfaceView_SurfaceCreated(ISurfaceHolder holder)
    {
        eglHelper = new EGLHelper();
        eglHelper.Init(holder.Surface);

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

    private void GameLoopThread(object? obj)
    {
        eglHelper.MakeCurrent();
        GraphicsContext = new GraphicsContext();
        GraphicsContext.Initialize(eglHelper, w, h, true);
        InputManager = new InputManager();
        SoundManager = new SoundManager();
        Storage = new StorageManager();
        Initialized?.Invoke();

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

    public override bool OnTouchEvent(MotionEvent? e)
    {
        (this.InputManager.PointingDevice as PointingDevice).OnTouch(e);

        return base.OnTouchEvent(e);
    }
}
