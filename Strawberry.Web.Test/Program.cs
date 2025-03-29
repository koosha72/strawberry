using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Strawberry;
using Strawberry.Test;
using Strawberry.Web;

[assembly: SupportedOSPlatform("browser")]

public static class Program
{
	public static void Main(string[] args)
	{
		MyGameContext stdGameContext = new MyGameContext();
		Game game = new Game();
		game.Run(stdGameContext, new Strawberry.Web.GameLauncher());
	}
}