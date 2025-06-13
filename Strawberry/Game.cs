using Strawberry.Core;


namespace Strawberry
{
    /// <summary>
    /// Runs a game using a game context in which the game is happening and a platform dependent game launcher.
    /// </summary>
    public class Game
    {
        public IGameContext GameContext { get; private set; }

        IGameLauncher launcher;

        IFrameInfoProvider frameInfoProvider;

        bool initialized = false;


        //public static Game CurrentGame { get; private set; }

        /// <summary>
        /// Runs the game
        /// </summary>
        /// <param name="context">The game context in which the actual game is happening</param>
        /// <param name="launcher">The launcher to be used to initialize the game.</param>
        public void Run(IGameContext context, IGameLauncher launcher)
        {
            Run(context, launcher, new GameTimer());
        }

        public void Run(IGameContext context, IGameLauncher launcher, IFrameInfoProvider frameInfoProvider)
        {
            GameContext = context;
            this.frameInfoProvider = frameInfoProvider;
            FrameInfo.Register(frameInfoProvider);
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
            frameInfoProvider.Initialize();
            launcher.InputManager?.Initialize();
            GameContext.OnInitialize(launcher);
            initialized = true;
        }

        void FixedUpdate()
        {
            while (frameInfoProvider.ShouldFixedUpdate)
            {
                GameContext.OnFixedUpdate();
                frameInfoProvider.FixedUpdate();
            }
        }

        void VariableUpdate()
        {
            if (GameContext.OnBeginUpdate())
            {
                GameContext.OnUpdate();
                GameContext.OnEndUpdate();
                launcher.InputManager?.Update();
            }
        }

        void Render()
        {
            if (GameContext.OnBeginRender())
            {
                GameContext.OnRender();
                GameContext.OnEndRender();
            }
        }

        /// <summary>
        /// This method is called every frame.
        /// </summary>
        void Update()
        {
            if (!initialized)
                return;
            frameInfoProvider.BeginUpdate();
            FixedUpdate();
            VariableUpdate();
            Render();
            frameInfoProvider.EndUpdate();
        }
    }
}
