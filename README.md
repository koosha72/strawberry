<p align="center">
  <img src="https://docs.sb-engine.ir/sb-icon.png" alt="StrawBerry Game Engine"/>
</p>
<h1 align="center" text-decoration="none">
Strawberry Game Engine
</h1>

## Description
The Strawberry Game Engine is a multi-platform 2D game engine built entirely on .NET.
The core of this engine is provided completely open source to users.
To use the engine core, you must be fully proficient in the C# programming language.

## Visual Environment and Editor
Of course, the visual environment of this engine is under construction. You can register for the initial private version (Private Alpha Version).
Registration approval is based on the resume you submit.

## Initial Setup
Setting up a project for different platforms has some differences. Normally, you should have multiple projects.
### 1- Main Game Project (Multi-platform)
In this project, you should avoid using platform-specific code as much as possible. The code is shared, and the game uses the same code across all platforms.
### 2- Platform Launchers
For each platform, a launcher project must be created. The launcher project code is generally very simple.
The launcher project depends on the main game project and runs it.

For example, the desktop platform launcher code is as follows:
```csharp
using MyGame;

namespace MyGame.Desktop
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MyGameContext gameContext = new MyGameContext();
            Strawberry.Game game = new Strawberry.Game();
            game.Run(gameContext, new Strawberry.Desktop.GameLauncher(Platform.DisplayMode.Windowed));
        }
    }
}
```
In the code above, `MyGameContext` is the main game context called from the multi-platform project. The game is executed using a `GameLauncher` specific to the desktop platform.
Launching and running on other platforms is slightly different, which you can refer to in the Strawberry guide.

[Setup and Running the Game](https://docs.sb-engine.ir/docs/latest/en/Documentation/Getting-Started)

## Guide
The Strawberry Engine guide consists of two parts:
### User Manual
This guide is written in an educational style and guides you step by step through performing various tasks with the engine.

[User Manual](https://docs.sb-engine.ir/docs/latest/en/Documentation/)

### API Reference
This section fully lists all structures and functions of the engine and provides a brief description of each one's functionality.

[API Reference](https://docs.sb-engine.ir/docs/latest/en/API/)

## Support

## Bug Reports
One of the biggest ways you can help us build this engine is by using it and reporting bugs or even suggesting structural changes.

Help us develop Strawberry better by using the link below:
[Report Bugs and Suggestions](https://git.sb-engine.ir/koosha_ab/strawberry-main/issues)
