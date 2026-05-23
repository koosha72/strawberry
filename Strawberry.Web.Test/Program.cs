using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Strawberry;
using Strawberry.Test;
using Strawberry.Web;

[assembly: SupportedOSPlatform("browser")]

public static class Program
{
	public static async Task Main(string[] args)
	{
		MyGameContext stdGameContext = new MyGameContext();
		Game game = new Game();
		var l = new Strawberry.Web.GameLauncher();
		await l.AOTDownload("timeup.wav");
		await l.AOTDownload("timeup_mono.wav");
		await l.AOTDownload("music.mid");
		await l.AOTDownload("8MBGMSFX.SF2");
		await l.AOTDownload("BYagut.font");
		game.Run(stdGameContext, l);
	}
}