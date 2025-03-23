using Strawberry.Core;
using Strawberry.Graphics;
using System.Diagnostics;


namespace Strawberry
{
    /// <summary>
    /// Runs a game using a game context in which the game is happening and a platform dependent game launcher.
    /// </summary>
    public class Game : IFrameInfoProvider
    {
        public IGameContext GameContext { get; private set; }

        public IGraphicsContext GraphicsContext { get; private set; }
        IGameLauncher launcher;


        //public static Game CurrentGame { get; private set; }


        #region FPS and GameTime
        int gameSpeed;

        Stopwatch timer;
        long lastMs;
        long nextTime;
        float totalFrameTime;
        int frame;
        int second;
        long msSkip;
        int loop = 0;

        public TimeSpan ElapsedTime
        {
            get
            {
                return timer.Elapsed;
            }
        }


        /// <summary>
        /// Gets or sets the number of game fixed updates per second.
        /// </summary>
        public int GameSpeed
        {
            get { return gameSpeed; }
            set
            {
                gameSpeed = value;
                msSkip = 1000 / gameSpeed;
                if (Stopwatch.IsHighResolution)
                {
                    msSkip = Stopwatch.Frequency / gameSpeed;
                }
            }
        }

        /// <summary>
        /// Gets the number of Frames Per Second
        /// </summary>
        public int FPS { get; private set; }

        /// <summary>
        /// Gets the real number of game updates per second.
        /// </summary>
        public int RealGameSpeed { get; private set; }

        /// <summary>
        /// Gets the minimum number of Frames Per Second during game.
        /// </summary>
        public int MinFPS { get; private set; }

        /// <summary>
        /// Gets the maximum number of Frames Per Second during game.
        /// </summary>
        public int MaxFPS { get; private set; }

        /// <summary>
        /// Gets the last time in which game scene has been rendered.
        /// </summary>
        public float LastTime { get; private set; }

        /// <summary>
        /// The fraction of seconds by which the scene is rendered. 
        /// You can use this in your Update or Render events to fix your movements, animations, etc. to real seconds.
        /// </summary>
        public float DeltaTime
        {
            get { return LastTime / Stopwatch.Frequency; }
        }

        /// <summary>
        /// The fraction of seconds by which the scene is rendered. This is completely bound to GameSpeed.
        /// You can use this in your FixedUpdate events to fix your movements, animations, etc. to real seconds.
        /// </summary>
        public float FixedDeltaTime
        {
            get { return 1.0f / GameSpeed; }
        }

        #endregion

        /// <summary>
        /// Runs the game
        /// </summary>
        /// <param name="context">The game context in which the actual game is happening</param>
        /// <param name="launcher">The launcher to be used to initialize the game.</param>
        public void Run(IGameContext context, IGameLauncher launcher)
        {
            GameContext = context;
            FrameInfo.Register(this);
            //CurrentGame = this;
            this.launcher = launcher;
            launcher.Initialized += Initialize;
            launcher.Initialize(context.Width, context.Height);
            launcher.GameLoop += Update;
            launcher.Run();

            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (launcher == null)
                throw new ArgumentNullException(nameof(launcher));
        }

        /// <summary>
        /// Happens when launcher is initialized
        /// </summary>
        void Initialize()
        {
            gameSpeed = 60;
            timer = Stopwatch.StartNew();

            msSkip = 1000 / GameSpeed;
            if (Stopwatch.IsHighResolution)
            {
                msSkip = Stopwatch.Frequency / GameSpeed;
            }

            nextTime = Stopwatch.GetTimestamp();
            launcher.InputManager?.Initialize();
            GameContext.OnInitialize(launcher);

            MaxFPS = int.MinValue;
            MinFPS = int.MaxValue;
            frame = 0;
            totalFrameTime = 0;
        }

        /// <summary>
        /// This method is called every frame.
        /// </summary>
        void Update()
        {
            //while (launcher.GraphicsContext.IsApplicationIdle())
            {
                if (second == (int)timer.ElapsedMilliseconds / 1000)
                {
                }
                else
                {
                    RealGameSpeed = frame;
                    second = (int)timer.ElapsedMilliseconds / 1000;
                    frame = 0;
                }

                FPS = (int)(((float)Stopwatch.Frequency / LastTime) + 0.5f);
                if (FPS < MinFPS && FPS > 0 && second > 0)
                    MinFPS = FPS;
                if (FPS > MaxFPS)
                    MaxFPS = FPS;
                if (MaxFPS > 1000)
                    MaxFPS = 0;
                loop = 0;
                /*if (InputManager != null)
                    InputManager.BeginUpdate();*/
                float tempLastTime = LastTime;
                while (Stopwatch.GetTimestamp() > nextTime && loop < 5)
                {
                    LastTime = 1000f / GameSpeed;
                    launcher.InputManager?.Update();
                    GameContext.OnFixedUpdate();
                    frame++;
                    loop++;
                    nextTime += msSkip;
                }
                LastTime = tempLastTime;
                if (GameContext.OnBeginUpdate())
                {
                    GameContext.OnUpdate();
                    GameContext.OnEndUpdate();
                }
                /*if (InputManager != null)
                    InputManager.EndUpdate();*/
                if (GameContext.OnBeginRender())
                {
                    GameContext.OnRender();
                    GameContext.OnEndRender();
                }
                totalFrameTime = timer.ElapsedTicks - lastMs;
                lastMs = timer.ElapsedTicks;
                LastTime = totalFrameTime * 0.9f + LastTime * 0.1f;
                //Debug.WriteLine(FPS);
            }
            //Thread.Sleep(1);
        }
    }
}
