/*
 * Strawberry Game Engine
 * File: DisposableReferenceObject.cs
 * Author: Koosha Aabedini Nassab
 *
 * Reference object with disposable support for managed and unmanaged cleanup.
 */

using System;
using System.Collections.Concurrent;

namespace Strawberry;

public class DisposableReferenceObject : ReferenceObject
{
    public bool IsDisposed { get; protected set; }

    private static readonly ConcurrentQueue<DisposableReferenceObject> cleanupQueue = new();

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
        if (IsDisposed) return;
        IsDisposed = true;
        if (disposing)
        {
            CleanManaged();
            GC.SuppressFinalize(this);
        }
        cleanupQueue.Enqueue(this);
    }

    ~DisposableReferenceObject()
    {
        Dispose(false);
    }

    public static void ProcessCleanupQueue()
    {
        while (cleanupQueue.TryDequeue(out var obj))
        {
            obj.CleanUnmanaged();
        }
    }
}
