/*
 * Strawberry Game Engine
 * File: StdGameContext.cs
 * Author: Koosha Aabedini Nassab
 *
 * Standard game context implementation with scene management.
 */

using Strawberry.Graphics;
using Strawberry.Input;
using Strawberry.Sound;

namespace Strawberry.Core
{
    /// <summary>
    /// A standard game context that can be used for games. For special situations like editors or custom game logic,
    /// you may need to implement your own game context using the <see cref="IGameContext"/> interface.
    /// </summary>
    public class StdGameContext : IGameContext
    {
        /// <summary>
        /// Gets the width of the game context.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height of the game context.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the collection of scenes managed by this game context.
        /// </summary>
        public SceneCollection Scenes { get; private set; }

        private int sceneIndex = 0;

        /// <summary>
        /// Gets or sets the index of the current scene. When set, the current scene is finalized and the new scene is initialized.
        /// </summary>
        public int CurrentSceneIndex
        {
            get
            {
                return sceneIndex;
            }
            set
            {
                SetScene(value);
            }
        }

        /// <summary>
        /// Gets the current scene being rendered and updated.
        /// </summary>
        public Scene CurrentScene
        {
            get { return Scenes[sceneIndex]; }
        }

        private IGameLauncher launcher;

        /// <summary>
        /// Gets the graphics context associated with this game context.
        /// </summary>
        public IGraphicsContext GraphicsContext { get { return launcher.GraphicsContext; } }

        /// <summary>
        /// Gets the input manager associated with this game context.
        /// </summary>
        public IInputManager InputManager { get { return launcher.InputManager; } }

        /// <summary>
        /// Gets the sound manager associated with this game context.
        /// </summary>
        public ISoundManager SoundManager { get { return launcher.SoundManager; } }

        AssetManager assets;
        public AssetManager Assets { get { return assets; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="StdGameContext"/> class with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the game context.</param>
        /// <param name="height">The height of the game context.</param>
        public StdGameContext(int width, int height)
        {
            Width = width;
            Height = height;
            assets = new AssetManager(null);
            Scenes = new SceneCollection();
        }

        /// <summary>
        /// Adds a scene to the game context and initializes it.
        /// </summary>
        /// <param name="scene">The scene to add.</param>
        public void AddScene(Scene scene)
        {
            Scenes.Add(scene);
            scene.Initialize(this);
        }

        /// <summary>
        /// Sets the current scene by its name.
        /// </summary>
        /// <param name="name">The name of the scene to set as current.</param>
        public void SetScene(string name)
        {
            SetScene(Scenes.IndexOf(Scenes[name]));
        }

        /// <summary>
        /// Sets the current scene by its name.
        /// </summary>
        /// <param name="index">The index of the scene to set as current.</param>
        public void SetScene(int index)
        {
            if (index < 0 || index >= Scenes.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Scene index is out of range.");

            Scenes[sceneIndex].OnFinish();
            sceneIndex = index;
            Scenes[sceneIndex].OnInitInstances();
        }

        /// <summary>
        /// Initializes the game context with the provided game launcher.
        /// </summary>
        /// <param name="launcher">The game launcher providing graphics, input, and sound contexts.</param>
        public virtual void OnInitialize(IGameLauncher launcher)
        {
            this.launcher = launcher;
        }

        /// <summary>
        /// Called at the beginning of the update cycle. Returns true if the update should proceed.
        /// </summary>
        /// <returns>True if the update should proceed, otherwise false.</returns>
        public virtual bool OnBeginUpdate()
        {
            if (Scenes.Count > 0)
                CurrentScene.OnBeginUpdate();
            return true;
        }

        /// <summary>
        /// Called to update the game logic.
        /// </summary>
        public virtual void OnUpdate()
        {
            if (Scenes.Count > 0)
                CurrentScene.OnUpdate();
        }

        /// <summary>
        /// Called to update the game logic at a fixed time step.
        /// </summary>
        public virtual void OnFixedUpdate()
        {
            if (Scenes.Count > 0)
                CurrentScene.OnFixedUpdate();
        }

        /// <summary>
        /// Called at the end of the update cycle.
        /// </summary>
        public virtual void OnEndUpdate()
        {
            if (Scenes.Count > 0)
                CurrentScene.OnEndUpdate();
        }

        /// <summary>
        /// Called at the beginning of the render cycle. Returns true if rendering should proceed.
        /// </summary>
        /// <returns>True if rendering should proceed, otherwise false.</returns>
        public virtual bool OnBeginRender()
        {
            GraphicsContext.BeginRender();

            if (Scenes.Count > 0)
            {
                if (CurrentScene.OnBeginRender())
                    GraphicsContext.Clear(CurrentScene.ClearColor);
            }
            else
                GraphicsContext.Clear(Color.Black);
            return true;
        }

        /// <summary>
        /// Called to render the game.
        /// </summary>
        public virtual void OnRender()
        {
            if (Scenes.Count > 0)
                CurrentScene.OnRender();
        }

        /// <summary>
        /// Called at the end of the render cycle.
        /// </summary>
        public virtual void OnEndRender()
        {
            GraphicsContext.EndRender();
        }

        public virtual void OnResized(int width, int height)
        {
            GraphicsContext.Resize(width, height);
        }

        public virtual void OnFocusLost()
        {
        }

        public virtual void OnFocusGained()
        {
        }

        public virtual void OnClosing()
        {
        }

        public virtual void OnGraphicsContextLost()
        {
        }

        public virtual void OnGraphicsContextRestored()
        {
        }
    }
}