/*
 * Strawberry Game Engine
 * File: Enums.cs
 * Author: Koosha Aabedini Nassab
 *
 * Core engine enums, including pause state flags.
 */

namespace Strawberry.Core
{
    /// <summary>
    /// Do not use this! this is old
    /// </summary>
    [Flags]
    public enum PauseStateFlags
    {
        None = 0,
        Render = 1,
        Update = 2,
        GuiRender = 4,
    }
}
