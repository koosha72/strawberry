/*
 * Strawberry Game Engine
 * File: Game.cs
 * Author: Koosha Aabedini Nassab
 *
 * Main game controller that coordinates the update and render loop.
 */

using Strawberry.Core;
using Strawberry.EventSystem;


namespace Strawberry
{
    /// <summary>
    /// Runs a game using a game context in which the game is happening and a platform dependent game launcher.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Gets the current Game Context
        /// </summary>
        public IGameContext GameContext { get; private set; }

        IGameLauncher launcher;

        IFrameInfoProvider frameInfoProvider;

        bool initialized = false;

        float cleanUpTimer = 0;

        /// <summary>
        /// The current game instance running
        /// </summary>
        public static Game Instance { get; private set; }

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

        /// <summary>
        /// Runs the game
        /// </summary>
        /// <param name="context">The game context in which the actual game is happening</param>
        /// <param name="launcher">The launcher to be used to initialize the game.</param>
        /// <param name="frameInfoProvider">The frame info provider to be used to store frame information</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Run(IGameContext context, IGameLauncher launcher, IFrameInfoProvider frameInfoProvider)
        {

            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (launcher == null)
                throw new ArgumentNullException(nameof(launcher));

            GameContext = context;
            this.frameInfoProvider = frameInfoProvider;
            FrameInfo.Register(frameInfoProvider);
            //CurrentGame = this;
            this.launcher = launcher;
            launcher.Initialized += Initialize;
            launcher.Initialize(context.Width, context.Height);
            launcher.GameLoop += Update;
            launcher.Run();
        }

        /// <summary>
        /// Happens when launcher is initialized
        /// </summary>
        void Initialize()
        {
            Instance = this;
            launcher.InputManager?.Initialize();
            GameContext.OnInitialize(launcher);
            initialized = true;
            frameInfoProvider.Initialize();
        }
        /// <summary>
        /// Calls the OnFixedUpdate method of the game context and then calls the FixedUpdate method on the frame info provider
        /// Happens at fixed intervals, may be called more than once per frame
        /// </summary>
        void FixedUpdate()
        {
            while (frameInfoProvider.ShouldFixedUpdate)
            {
                GameContext.OnFixedUpdate();
                frameInfoProvider.FixedUpdate();
            }
            EventManager.Execute(EventCallTime.OnFixedUpdate);
        }
        /// <summary>
        /// Calls the OnUpdate method of the game context if OnBeginUpdate returns true and OnEndUpdate is called after the update method has been called
        /// Happens as fast as possible, may be called less than fixed update
        /// </summary>
        void VariableUpdate()
        {
            if (GameContext.OnBeginUpdate())
            {
                EventManager.Execute(EventCallTime.OnBeginUpdate);
                GameContext.OnUpdate();
                EventManager.Execute(EventCallTime.OnUpdate);
                GameContext.OnEndUpdate();
                EventManager.Execute(EventCallTime.OnEndUpdate);
                launcher.InputManager?.Update();
            }
            cleanUpTimer += frameInfoProvider.DeltaTime;
            if (cleanUpTimer >= 10)
            {
                cleanUpTimer -= 10;
                ReferenceObject.CleanDeadReferences();
            }
        }
        /// <summary>
        /// Calls the OnRender method of the game context if OnBeginRender returns true and OnEndRender is called after the render method has been called
        /// Happens as fast as possible
        /// </summary>
        void Render()
        {
            if (GameContext.OnBeginRender())
            {
                EventManager.Execute(EventCallTime.OnBeginRender);
                GameContext.OnRender();
                EventManager.Execute(EventCallTime.OnRender);
                GameContext.OnEndRender();
                EventManager.Execute(EventCallTime.OnEndRender);
            }
        }

        /// <summary>
        /// The main game loop.
        /// </summary>
        void Update()
        {
            if (!initialized)
                return;
            frameInfoProvider.BeginUpdate();
            FixedUpdate();
            VariableUpdate();
            Render();

            GameContext?.SoundManager?.Update();
            frameInfoProvider.EndUpdate();
        }

        public void Exit()
        {
            launcher.Exit();
        }
    }
}
