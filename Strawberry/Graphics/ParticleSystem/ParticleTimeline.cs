/*
 * Strawberry Game Engine
 * File: ParticleTimeline.cs
 * Author: Koosha Aabedini Nassab
 *
 * Timeline that updates particles using a sequence of affectors.
 */

using System.Collections.Generic;

namespace Strawberry.Graphics.ParticleSystem
{
    /// <summary>
    /// Timeline that updates particles using a sequence of affectors
    /// </summary>
    public class ParticleTimeline
    {
        List<IParticleAffector> affectors = new List<IParticleAffector>();

        /// <summary>
        /// The number of affectors in this timeline
        /// </summary>
        public int AffectorCount => affectors.Count;

        /// <summary>
        /// Adds the given affector to this timeline
        /// </summary>
        /// <param name="affector">The affector to add</param>
        public void AddAffector(IParticleAffector affector)
        {
            affectors.Add(affector);
        }

        /// <summary>
        /// Removes the given affector from this timeline, if it exists
        /// </summary>
        /// <param name="affector">The affector to remove</param>
        /// <returns>Returns true if the affector was removed</returns>
        public bool RemoveAffector(IParticleAffector affector)
        {
            return affectors.Remove(affector);
        }

        /// <summary>
        /// Removes all affectors from this timeline
        /// </summary>
        public void ClearAffectors()
        {
            affectors.Clear();
        }

        /// <summary>
        /// Updates the given particle using the timeline's affectors in order of addition to this timeline.
        /// </summary>
        /// <param name="particle">The particle to update</param>
        /// <param name="dt">The delta time</param>
        public void Update(ref Particle particle, float dt)
        {
            for (int i = 0; i < affectors.Count; i++)
            {
                affectors[i].Update(ref particle, dt);
            }
        }
    }
}
