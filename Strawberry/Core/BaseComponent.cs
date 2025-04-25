namespace Strawberry.Core
{
    /// <summary>
    /// Base class of all components in Strawberry.
    /// You should define your own component classes inheriting from this class.
    /// </summary>
    public abstract class BaseComponent : ReferenceObject
    {
        /// <summary>
        /// The owner of the component
        /// </summary>
        public Entity? Owner
        {
            get;
            set;
        }

        /// <summary>
        /// The scene in which the entity is living
        /// </summary>
        public Scene? Scene { get { return Owner?.Scene; } }

        /// <summary>
        ///  The current game context
        /// </summary>
        public IGameContext? GameContext { get { return Owner?.Scene?.GameContext; } }

        /// <summary>
        /// This method happens when the component is intialized. You can override it in your own components.
        /// </summary>
        /// <param name="owner">The owner of the component</param>
        public virtual void Initialize(Entity owner)
        {
        }

        public virtual void OnFinished()
        {

        }

        public virtual void OnBeginUpdate()
        {

        }

        public virtual void OnUpdate()
        {

        }

        public virtual void OnEndUpdate()
        {

        }

        public virtual void OnFixedUpdate()
        {

        }

        public virtual void OnRender()
        {

        }


        /// <summary>
        /// This method happens when the component is removed. You can override it in your own components.
        /// </summary>
        public virtual void Removed()
        {
            Owner = null;
        }
    }
}
