/*
 * Strawberry Game Engine
 * File: DisposableReferenceObject.cs
 * Author: Koosha Aabedini Nassab
 *
 * Reference object with disposable support for managed and unmanaged cleanup.
 */

using System;

namespace Strawberry;

public class DisposableReferenceObject : ReferenceObject
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

    ~DisposableReferenceObject()
    {
        Dispose(false);
    }
}
