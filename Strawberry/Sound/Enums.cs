/*
 * Strawberry Game Engine
 * File: Enums.cs
 * Author: Koosha Aabedini Nassab
 *
 * Enumerations used by the audio subsystem.
 */

namespace Strawberry.Sound
{
    /// <summary>
    /// Specifies the attenuation (fall-off) model used for 3D spatialized sounds, 
    /// determining how the volume decreases as the distance between the audio source and the listener increases.
    /// </summary>
    public enum FallOffMode
    {
        /// <summary>
        /// No distance attenuation. The sound's volume remains constant regardless of the distance 
        /// between the source and the listener.
        /// </summary>
        None,

        /// <summary>
        /// Inverse distance model with clamping. Volume attenuates inversely with distance, 
        /// but is clamped so it does not exceed the volume at the reference distance, 
        /// and stops attenuating beyond the maximum distance.
        /// </summary>
        InverseDistanceClamped,

        /// <summary>
        /// Inverse distance model without clamping. Volume attenuates inversely with distance, 
        /// continuing to decrease indefinitely without a maximum distance cutoff.
        /// </summary>
        InverseDistance,

        /// <summary>
        /// Linear distance model without clamping. Volume decreases linearly between the reference distance 
        /// and the maximum distance, reaching zero at the maximum distance.
        /// </summary>
        LinearDistance,

        /// <summary>
        /// Linear distance model with clamping. Volume decreases linearly between the reference distance 
        /// and the maximum distance, and is clamped so it does not exceed the volume at the reference distance.
        /// </summary>
        LinearDistanceClamped,

        /// <summary>
        /// Exponential distance model without clamping. Volume attenuates exponentially with distance, 
        /// continuing to decrease indefinitely.
        /// </summary>
        ExponentDistance,

        /// <summary>
        /// Exponential distance model with clamping. Volume attenuates exponentially with distance, 
        /// but is clamped so it does not exceed the volume at the reference distance, 
        /// and stops attenuating beyond the maximum distance.
        /// </summary>
        ExponentDistanceClamped
    }
}