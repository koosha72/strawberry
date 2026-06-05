namespace Strawberry.Core
{
    /// <summary>
    /// Base class for all components in the Strawberry engine.
    /// Custom components should inherit from this class to integrate with the entity-component system.
    /// </summary>
    public abstract class BaseComponent : ReferenceObject
    {
        /// <summary>
        /// Gets or sets the <see cref="Entity"/> that owns this component.
        /// </summary>
        public Entity Owner
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="Scene"/> in which the owning entity currently resides.
        /// Returns <c>null</c> if the entity is not part of any scene.
        /// </summary>
        public Scene Scene { get { return Owner?.Scene; } }

        /// <summary>
        /// Gets the current <see cref="IGameContext"/>.
        /// Returns <c>null</c> if the entity or scene is not available.
        /// </summary>
        public IGameContext GameContext { get { return Owner?.Scene?.GameContext; } }

        /// <summary>
        /// Called when the component is initialized. Override this method to set up the component's initial state.
        /// </summary>
        /// <param name="owner">The <see cref="Entity"/> that owns this component.</param>
        public virtual void Initialize(Entity owner)
        {
        }

        public virtual void OnBegin()
        {
            
        }

        public virtual void OnEnabled()
        {
            
        }

        /// <summary>
        /// Called when a new component is added to the owning entity.
        /// </summary>
        /// <param name="component">The newly added component.</param>
        public virtual void OnComponentAdded(BaseComponent component)
        {
            
        }

        public virtual void OnDisabled()
        {
            
        }

        /// <summary>
        /// Called when the component or its owning entity is finishing its lifecycle. 
        /// Override to perform cleanup or final actions.
        /// </summary>
        public virtual void OnFinished()
        {

        }

        /// <summary>
        /// Called at the beginning of the update cycle, before <see cref="OnUpdate"/>.
        /// </summary>
        public virtual void OnBeginUpdate()
        {

        }

        /// <summary>
        /// Called every frame during the main update cycle. 
        /// Override to implement frame-by-frame logic.
        /// </summary>
        public virtual void OnUpdate()
        {

        }

        /// <summary>
        /// Called at the end of the update cycle, after <see cref="OnUpdate"/>.
        /// </summary>
        public virtual void OnEndUpdate()
        {

        }

        /// <summary>
        /// Called at a fixed time interval, independent of the frame rate. 
        /// Typically used for physics calculations and other time-sensitive logic.
        /// </summary>
        public virtual void OnFixedUpdate()
        {

        }

        /// <summary>
        /// Called during the rendering phase. 
        /// Override to implement drawing or rendering logic for the component.
        /// </summary>
        public virtual void OnRender()
        {

        }


        /// <summary>
        /// Called when the component is removed from its owner. 
        /// Override to perform cleanup and release resources. Sets the <see cref="Owner"/> to <c>null</c>.
        /// </summary>
        public virtual void Removed()
        {
            Owner = null;
        }
    }
}