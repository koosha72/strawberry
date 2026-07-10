using OpenTK.Windowing.Desktop;
using Strawberry.Graphics;
using Strawberry.Input;
using Strawberry.Desktop.Graphics;
using Strawberry.Desktop.Input;
using Strawberry.Sound;
using Strawberry.OpenAL;
using Strawberry.Platform;


namespace Strawberry.Desktop
{
    /// <summary>
    /// Launches a game using an opengl renderer.
    /// </summary>
    public class GameLauncher : IGameLauncher
    {
        public IGraphicsContext GraphicsContext { get; private set; }

        public IInputManager InputManager { get; private set; }

        public ISoundManager SoundManager { get; private set; }

        public event Action Initialized;
        public event Action GameLoop;
        OpenTK.Windowing.Desktop.GameWindow wnd;

        DisplayMode displayMode;

        string windowTitle = "Strawberry";

        public GameLauncher(DisplayMode displayMode, string windowTitle = "Strawberry")
        {
            this.displayMode = displayMode;
            this.windowTitle = windowTitle;
        }

        public void Initialize(int width, int height)
        {
            NativeWindowSettings s = new NativeWindowSettings();
            var monitor = Monitors.GetPrimaryMonitor();
            switch (displayMode)
            {
                case DisplayMode.Windowed:
                    s.WindowBorder = OpenTK.Windowing.Common.WindowBorder.Fixed;
                    s.ClientSize = new OpenTK.Mathematics.Vector2i(width, height);
                    s.WindowState = OpenTK.Windowing.Common.WindowState.Normal;
                    s.Location = monitor.ClientArea.HalfSize - new OpenTK.Mathematics.Vector2i(width / 2, height / 2);
                    break;
                case DisplayMode.Borderless:
                    s.WindowBorder = OpenTK.Windowing.Common.WindowBorder.Hidden;
                    s.ClientSize = monitor.ClientArea.Size + new OpenTK.Mathematics.Vector2i(1);
                    s.WindowState = OpenTK.Windowing.Common.WindowState.Normal;
                    s.Location = monitor.ClientArea.Min;
                    break;
                case DisplayMode.Fullscreen:
                    s.WindowBorder = OpenTK.Windowing.Common.WindowBorder.Hidden;
                    s.WindowState = OpenTK.Windowing.Common.WindowState.Fullscreen;
                    s.ClientSize = new OpenTK.Mathematics.Vector2i(width, height);
                    break;
            }
            s.Title = windowTitle;
            s.APIVersion = Version.Parse("4.1.0");
            s.Profile = OpenTK.Windowing.Common.ContextProfile.Any;
            var displayConfig = new DisplayConfig(width, height, displayMode, this);
            PlatformServices.RegisterService<IDisplayConfig>(displayConfig);

            GameWindowSettings g = new GameWindowSettings();
            g.UpdateFrequency = 500;

            wnd = new GameWindow(g, s);

            wnd.VSync = OpenTK.Windowing.Common.VSyncMode.Off;

            wnd.Load += Wnd_Load1;
            wnd.UpdateFrame += Wnd_UpdateFrame;
            GraphicsContext = new GraphicsContext();
            GraphicsContext.Initialize(wnd, width, height);

            InputManager = new Input.InputManager();
            SoundManager = new OpenAL.SoundManager();
            PlatformServices.RegisterService<IStorage>(new StorageManager());
        }

        private void Wnd_Load1()
        {
            Initialized?.Invoke();

            if (InputManager.PointingDevice != null)
            {
                wnd.MouseMove += (InputManager.PointingDevice as PointingDevice).MouseMove;
                wnd.MouseDown += (InputManager.PointingDevice as PointingDevice).MousePressed;
                wnd.MouseUp += (InputManager.PointingDevice as PointingDevice).MouseReleased;
                wnd.KeyDown += (InputManager.Keyboard as Keyboard).KeyPressed;
                wnd.KeyUp += (InputManager.Keyboard as Keyboard).KeyReleased;
            }
        }

        private void Wnd_UpdateFrame(OpenTK.Windowing.Common.FrameEventArgs obj)
        {
            GameLoop?.Invoke();
        }

        public void Run()
        {
            wnd.Run();
        }

        public void Exit()
        {
            wnd.UpdateFrame -= Wnd_UpdateFrame;
            wnd.Close();
        }

        public void ChangeDisplayMode(DisplayMode displayMode)
        {
            var displayConfig = PlatformServices.GetService<IDisplayConfig>();
            int width = displayConfig.Width;
            int height = displayConfig.Height;
            var monitor = Monitors.GetPrimaryMonitor();
            switch (displayMode)
            {
                case DisplayMode.Windowed:
                    wnd.WindowBorder = OpenTK.Windowing.Common.WindowBorder.Fixed;
                    wnd.ClientSize = new OpenTK.Mathematics.Vector2i(width, height);
                    wnd.WindowState = OpenTK.Windowing.Common.WindowState.Normal;
                    wnd.Location = monitor.ClientArea.HalfSize - new OpenTK.Mathematics.Vector2i(width / 2, height / 2);
                    break;
                case DisplayMode.Borderless:
                    wnd.WindowBorder = OpenTK.Windowing.Common.WindowBorder.Hidden;
                    wnd.ClientSize = monitor.ClientArea.Size + new OpenTK.Mathematics.Vector2i(1);
                    wnd.WindowState = OpenTK.Windowing.Common.WindowState.Normal;
                    wnd.Location = monitor.ClientArea.Min;
                    break;
                case DisplayMode.Fullscreen:
                    wnd.WindowBorder = OpenTK.Windowing.Common.WindowBorder.Hidden;
                    wnd.WindowState = OpenTK.Windowing.Common.WindowState.Fullscreen;
                    wnd.ClientSize = new OpenTK.Mathematics.Vector2i(width, height);
                    break;
            }
        }

        public void SetSize(int width, int height)
        {
            var displayConfig = PlatformServices.GetService<IDisplayConfig>();
            if (displayConfig.DisplayMode == DisplayMode.Windowed || displayConfig.DisplayMode == DisplayMode.Fullscreen)
                wnd.ClientSize = new OpenTK.Mathematics.Vector2i(width, height);
        }
    }
}
