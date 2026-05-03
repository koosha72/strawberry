using System.Diagnostics;

namespace Strawberry
{

    public class GameTimer : IFrameInfoProvider
    {
        int gameSpeed;

        Stopwatch timer;
        long lastMs;
        long nextTime;
        float totalFrameTime;
        int frame;
        int second;
        long msSkip;
        int loop = 0;

        public TimeSpan ElapsedTime
        {
            get
            {
                return timer.Elapsed;
            }
        }


        /// <summary>
        /// Gets or sets the number of game fixed updates per second.
        /// </summary>
        public int GameSpeed
        {
            get { return gameSpeed; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                gameSpeed = value;
                msSkip = 1000 / gameSpeed;
                if (Stopwatch.IsHighResolution)
                {
                    msSkip = Stopwatch.Frequency / gameSpeed;
                }
            }
        }

        /// <summary>
        /// Gets the number of Frames Per Second
        /// </summary>
        public int FPS { get; private set; }

        /// <summary>
        /// Gets the real number of game updates per second.
        /// </summary>
        public int RealGameSpeed { get; private set; }

        /// <summary>
        /// Gets the minimum number of Frames Per Second during game.
        /// </summary>
        public int MinFPS { get; private set; }

        /// <summary>
        /// Gets the maximum number of Frames Per Second during game.
        /// </summary>
        public int MaxFPS { get; private set; }

        /// <summary>
        /// Gets the last time in which game scene has been rendered.
        /// </summary>
        public float LastTime { get; private set; }

        /// <summary>
        /// The fraction of seconds by which the scene is rendered. 
        /// You can use this in your Update or Render events to fix your movements, animations, etc. to real seconds.
        /// </summary>
        public float DeltaTime
        {
            get { return LastTime / Stopwatch.Frequency; }
        }

        /// <summary>
        /// The fraction of seconds by which the scene is rendered. This is completely bound to GameSpeed.
        /// You can use this in your FixedUpdate events to fix your movements, animations, etc. to real seconds.
        /// </summary>
        public float FixedDeltaTime
        {
            get { return 1.0f / GameSpeed; }
        }

        public bool ShouldFixedUpdate => Stopwatch.GetTimestamp() > nextTime && loop < 5;

        public void Initialize()
        {
            gameSpeed = 60;
            timer = Stopwatch.StartNew();

            msSkip = 1000 / GameSpeed;
            if (Stopwatch.IsHighResolution)
            {
                msSkip = Stopwatch.Frequency / GameSpeed;
            }

            nextTime = Stopwatch.GetTimestamp();

            MaxFPS = int.MinValue;
            MinFPS = int.MaxValue;
            frame = 0;
            totalFrameTime = 0;
        }

        public void BeginUpdate()
        {
            if (timer == null)
                return;
            if (second == (int)timer.ElapsedMilliseconds / 1000)
            {
            }
            else
            {
                RealGameSpeed = frame;
                second = (int)timer.ElapsedMilliseconds / 1000;
                frame = 0;
            }

            FPS = (int)(((float)Stopwatch.Frequency / LastTime) + 0.5f);
            if (FPS < MinFPS && FPS > 0 && second > 0)
                MinFPS = FPS;
            if (FPS > MaxFPS)
                MaxFPS = FPS;
            if (MaxFPS > 1000)
                MaxFPS = 0;
            loop = 0;
        }

        public void FixedUpdate()
        {
            if (timer == null)
                return;
            frame++;
            loop++;
            nextTime += msSkip;
        }


        public void EndUpdate()
        {
            if (timer == null)
                return;
            totalFrameTime = timer.ElapsedTicks - lastMs;
            lastMs = timer.ElapsedTicks;
            LastTime = totalFrameTime * 0.9f + LastTime * 0.1f;
        }
    }
}