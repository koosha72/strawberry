using Strawberry.Test;
using Android.Content.PM;
using Android.Views;

namespace Strawberry.Android.Test;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : GameLauncher
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        RequestedOrientation = ScreenOrientation.Landscape;
        RequestWindowFeature(WindowFeatures.NoTitle);
        Window.SetFlags(WindowManagerFlags.Fullscreen,
            WindowManagerFlags.Fullscreen);
        MyGameContext stdGameContext = new MyGameContext();
        Game game = new Game();

        View decorView = Window.DecorView;
        var uiOptions = View.SystemUiFlagHideNavigation | View.SystemUiFlagImmersiveSticky;
        decorView.SystemUiFlags = uiOptions;

        game.Run(stdGameContext, this);
    }
}