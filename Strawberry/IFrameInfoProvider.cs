/*
 * Strawberry Game Engine
 * File: IFrameInfoProvider.cs
 * Author: Koosha Aabedini Nassab
 *
 * Interface for providing frame timing and update statistics.
 */

namespace Strawberry;

/// <summary>
/// Provides information about the FPS and other frame related information
/// </summary>
public interface IFrameInfoProvider
{
    /// <summary>
    /// Gets the speed on based on which the game is running
    /// </summary>
    int GameSpeed { get; set; }

    /// <summary>
    /// Gets the number of Frames Per Second
    /// </summary>
    int FPS { get; }

    /// <summary>
    /// Gets the real number of game updates per second.
    /// </summary>
    int RealGameSpeed { get; }

    /// <summary>
    /// Gets the minimum number of Frames Per Second during game.
    /// </summary>
    int MinFPS { get; }

    /// <summary>
    /// Gets the maximum number of Frames Per Second during game.
    /// </summary>
    int MaxFPS { get; }

    /// <summary>
    /// Gets the last time in which game scene has been rendered.
    /// </summary>
    float LastTime { get; }
    /// <summary>
    /// Gets the time difference between two frames
    /// </summary>
    float DeltaTime { get; }
    /// <summary>
    /// Gets the fixed time difference between two frames. This should always be based on GameSpeed
    /// </summary>
    float FixedDeltaTime { get; }
    /// <summary>
    /// Gets the time elapsed since the game started.
    /// </summary>
    TimeSpan ElapsedTime { get; }
    /// <summary>
    /// Gets a value indicating whether the game should run a fixed update.
    /// </summary>
    bool ShouldFixedUpdate { get; }

    void Initialize();

    void BeginUpdate();

    void FixedUpdate();

    void EndUpdate();
}