using Strawberry.Test;

namespace Strawberry.Android.Test;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : GameLauncher
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        MyGameContext stdGameContext = new MyGameContext();
        Game game = new Game();
        game.Run(stdGameContext, this);
    }
}