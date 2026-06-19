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

        bool fullscreen;

        public GameLauncher(bool fullscreen)
        {
            this.fullscreen = fullscreen;
        }

        public void Initialize(int width, int height)
        {
            NativeWindowSettings s = new NativeWindowSettings();
            s.WindowState = fullscreen ? OpenTK.Windowing.Common.WindowState.Fullscreen : OpenTK.Windowing.Common.WindowState.Normal;
            s.ClientSize = new OpenTK.Mathematics.Vector2i(width, height);
            s.Title = "Strawberry";
            s.APIVersion = Version.Parse("4.1.0");
            GameWindowSettings g = new GameWindowSettings();
            g.UpdateFrequency = 500;

            s.WindowBorder = OpenTK.Windowing.Common.WindowBorder.Fixed;
            s.Profile = OpenTK.Windowing.Common.ContextProfile.Any;
            wnd = new OpenTK.Windowing.Desktop.GameWindow(g, s);

            wnd.VSync = OpenTK.Windowing.Common.VSyncMode.Off;

            wnd.Load += Wnd_Load1;
            wnd.UpdateFrame += Wnd_UpdateFrame;
            GraphicsContext = new GraphicsContext();
            GraphicsContext.Initialize(wnd, width, height, true);

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
    }
}
