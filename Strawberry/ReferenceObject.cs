/*
 * Strawberry Game Engine
 * File: ReferenceObject.cs
 * Author: Koosha Aabedini Nassab
 *
 * Base class for reference-tracked objects used by the engine.
 */

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Strawberry
{
    /// <summary>
    /// Base class of all reference objects. All entities or components are derived from this class. It is used for serialization and deserialization.
    /// </summary>
    public class ReferenceObject
    {
        static long counter = 1;

        ulong uniqueId = 0;
        static readonly ConcurrentDictionary<ulong, WeakReference<ReferenceObject>> objects =
            new ConcurrentDictionary<ulong, WeakReference<ReferenceObject>>();

        public ReferenceObject()
        {
            uniqueId = (ulong)Interlocked.Increment(ref counter);
            objects.TryAdd(uniqueId, new WeakReference<ReferenceObject>(this));
        }

        /// <summary>
        /// The unique id of the object in the game.
        /// </summary>
        public ulong UniqueId
        {
            get { return Volatile.Read(ref uniqueId); }
            private set
            {
                Volatile.Write(ref uniqueId, value);

                long newValue = (long)value;
                long currentCounter;
                do
                {
                    currentCounter = Interlocked.Read(ref counter);
                    if (currentCounter >= newValue)
                        break;
                }
                while (Interlocked.CompareExchange(ref counter, newValue, currentCounter) != currentCounter);
            }
        }

        /// <summary>
        /// Finds an object using its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ReferenceObject FindObjectById(ulong id)
        {
            if (objects.TryGetValue(id, out var weak))
            {
                if (weak.TryGetTarget(out var obj))
                    return obj;
                // Reference is dead; remove it
                objects.TryRemove(id, out _);
            }
            return null;
        }

        /// <summary>
        /// Removes the object from the object list
        /// </summary>
        public virtual void Destroy()
        {
            objects.TryRemove(uniqueId, out _);
        }

        /// <summary>
        /// Cleans the objects list of dead references
        /// </summary>
        public static void CleanDeadReferences()
        {
            foreach (var kvp in objects)
            {
                if (!kvp.Value.TryGetTarget(out _))
                    objects.TryRemove(kvp.Key, out _);
            }
        }
    }
}