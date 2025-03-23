namespace Strawberry
{
    public interface IFrameInfoProvider
    {
        /// <summary>
        /// The speed on based on which the game is running
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
        /// Time difference between two frames
        /// </summary>
        float DeltaTime { get; }
        /// <summary>
        /// Time difference between two frames. This should always be based on GameSpeed
        /// </summary>
        float FixedDeltaTime { get; }

        TimeSpan ElapsedTime { get; }

        bool ShouldFixedUpdate { get; }

        void Initialize();

        void BeginUpdate();

        void FixedUpdate();

        void EndUpdate();
    }

    public static class FrameInfo
    {
        public static IFrameInfoProvider Information { get; private set; }

        public static void Register(IFrameInfoProvider infoContainer)
        {
            Information = infoContainer;
        }
    }
}
