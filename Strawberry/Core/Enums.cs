namespace Strawberry.Core
{
    [Flags]
    public enum PauseStateFlags
    {
        None = 0,
        Render = 1,
        Update = 2,
        GuiRender = 4,
    }
}
