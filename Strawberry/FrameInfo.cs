namespace Strawberry
{
    public static class FrameInfo
    {
        public static IFrameInfoProvider Information { get; private set; }

        public static void Register(IFrameInfoProvider infoContainer)
        {
            Information = infoContainer;
        }
    }
}
