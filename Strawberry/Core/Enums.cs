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
    /// Represents a set of independent pause flags for a scene, entity, or component.
    /// Supports bitwise operations (<see cref="FlagsAttribute"/>). Each flag controls a 
    /// specific event pipeline; setting a flag pauses <strong>only</strong> that corresponding pipeline.
    /// </summary>
    /// <remarks>
    /// To check if a specific pipeline is paused, use a bitwise AND comparison, e.g., 
    /// <c>if ((flags &amp; PauseStateFlags.Update) != 0)</c>. Combining all flags 
    /// effectively pauses the entire target.
    /// </remarks>
    [Flags]
    public enum PauseStateFlags
    {
        /// <summary>
        /// No pause state is active. All update and render pipelines will execute normally.
        /// </summary>
        None = 0,

        /// <summary>
        /// Pauses the rendering pipeline. The target will be skipped during the draw/render pass.
        /// </summary>
        Render = 1,

        /// <summary>
        /// Pauses the standard update pipeline (e.g., game logic, AI, animation state machines, 
        /// and non-physics timers).
        /// </summary>
        Update = 1 << 1,

        /// <summary>
        /// Pauses the fixed-step update pipeline, which is typically used for physics simulations 
        /// and deterministic logic.
        /// </summary>
        FixedUpdate = 1 << 2,
    }
}
