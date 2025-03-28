using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Strawberry.Web;

[assembly: SupportedOSPlatform("browser")]

public static class Program
{
	public static void Main(string[] args)
	{
		Console.WriteLine($"Hello from dotnet!");

		Interop.Initialize();
		GameLauncher launcher = new GameLauncher();
	}
}