namespace Strawberry.Graphics
{
    public abstract class DisplayInfo
    {
        public abstract int Width { get; }

        public abstract int Height { get; }

        public static DisplayInfo? GetDisplayInfo()
        {
            return null;
        }
    }
}
