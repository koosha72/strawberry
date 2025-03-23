namespace Strawberry.Test
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MyGameContext stdGameContext = new MyGameContext();
            Game game = new Game();
            game.Run(stdGameContext, new Strawberry.OpenGL.GameLauncher());
        }
    }
}