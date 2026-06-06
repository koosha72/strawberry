/*
 * Strawberry Game Engine
 * File: IParticleAffector.cs
 * Author: Koosha Aabedini Nassab
 *
 * Interface for particle affectors that modify particles each update.
 */

namespace Strawberry.Graphics.ParticleSystem
{
    /// <summary>
    /// Interface for particle affectors that modify particles each update.
    /// </summary>
    public interface IParticleAffector
    {
        void Update(ref Particle particle, float dt);
    }
}
