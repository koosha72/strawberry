/*
 * Strawberry Game Engine
 * File: IBase.cs
 * Author: Koosha Aabedini Nassab
 *
 * Interface defining disposable base behavior.
 */

namespace Strawberry
{
    public interface IBase : IDisposable
    {
        bool IsDisposed { get; }

        void Dispose(bool disposing);
    }
}
