using Strawberry.Test;

namespace Strawberry.Desktop.Test
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
            game.Run(stdGameContext, new GameLauncher(false));
        }
    }
}