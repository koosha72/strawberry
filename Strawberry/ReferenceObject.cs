namespace Strawberry
{
    /// <summary>
    /// Base class of all refernce objects. All entities or components are derrived from this class. It is used for serilization and deserialization.
    /// </summary>
    public class ReferenceObject
    {
        static ulong counter = 1;

        ulong uniqueId = 0;
        static Dictionary<ulong, WeakReference<ReferenceObject>> objects = new Dictionary<ulong, WeakReference<ReferenceObject>>();

        public ReferenceObject()
        {
            uniqueId = counter++;
            objects.Add(uniqueId, new WeakReference<ReferenceObject>(this));
        }

        /// <summary>
        /// The unique id of the object in the game.
        /// </summary>
        public ulong UniqueId
        {
            get
            {
                return uniqueId;
            }
            private set
            {
                uniqueId = value;
                if (counter < uniqueId)
                    counter = uniqueId;
            }
        }

        /// <summary>
        /// Finds an object using its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ReferenceObject FindObjectById(ulong id)
        {
            ReferenceObject obj = null;
            if (objects[id].TryGetTarget(out obj) == false)
                objects.Remove(id);
            return obj;
        }

        /// <summary>
        /// Removes the object from the object list
        /// </summary>
        public virtual void Destroy()
        {
            objects.Remove(uniqueId);
        }
    }
}
