/*
 * Strawberry Game Engine
 * File: Base.cs
 * Author: Koosha Aabedini Nassab
 *
 * Base disposable class for objects that manage unmanaged resources.
 */

namespace Strawberry
{
    /// <summary>
    /// The Base Class for any object containing unmanaged references.
    /// </summary>
    public class Base : IDisposable
    {
        public bool IsDisposed { get; protected set; }

        protected virtual void CleanUnmanaged()
        {

        }

        protected virtual void CleanManaged()
        {

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    CleanManaged();
                }
                CleanUnmanaged();
                IsDisposed = true;
            }
        }

        ~Base()
        {
            Dispose(false);
        }
    }
}
