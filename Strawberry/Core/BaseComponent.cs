/*
 * Strawberry Game Engine
 * File: BaseComponent.cs
 * Author: Koosha Aabedini Nassab
 *
 * Base class for game components in the entity-component system.
 */

using Strawberry.Input;

namespace Strawberry.Core
{
    /// <summary>
    /// Base class for all components in the Strawberry engine.
    /// Custom components should inherit from this class to integrate with the entity-component system.
    /// </summary>
    public abstract class BaseComponent : ReferenceObject
    {
        /// <summary>
        /// Gets the <see cref="Entity"/> that owns this component.
        /// </summary>
        public Entity Owner
        {
            get;
            internal set;
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
        /// Gets the Keyboard instance of GameContext.InputManager
        /// </summary>
        public IKeyboard Keyboard { get { return GameContext?.InputManager?.Keyboard; } }

        /// <summary>
        /// Gets the PointingDevice instance of GameContext.InputManager
        /// </summary>
        public IPointingDevice PointingDevice { get { return GameContext?.InputManager?.PointingDevice; } }

        /// <summary>
        /// Gets whether or not this component is currently enabled.
        /// </summary>
        public bool Enabled { get; private set; } = true;

        /// <summary>
        /// Gets the asset manager object of the current scene.
        /// Returns <c>null</c> if the entity is not part of any scene.
        /// </summary>
        public AssetManager AssetManager { get => Scene?.Assets; }


        /// <summary>
        /// Called when the component is initialized. Override this method to set up the component's initial state.
        /// </summary>
        /// <param name="owner">The <see cref="Entity"/> that owns this component.</param>
        public virtual void Initialize(Entity owner)
        {
        }
        /// <summary>
        /// Called when the component is added to an entity.
        /// </summary>
        public virtual void OnBegin()
        {

        }
        /// <summary>
        /// Called when the component is enabled
        /// </summary>
        public virtual void OnEnabled()
        {

        }

        /// <summary>
        /// Called when a new component is added to the owning entity. This is also called on the new component for every existing component in the entity.
        /// </summary>
        /// <param name="component">The newly added component.</param>
        public virtual void OnComponentAdded(BaseComponent component)
        {

        }
        /// <summary>
        /// Called when the component is disabled.
        /// </summary>
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

        /// <summary>
        /// Disables the component.
        /// </summary>
        public void Enable()
        {
            Enabled = true;
            OnEnabled();
        }

        /// <summary>
        /// Enables the component.
        /// </summary>
        public void Disable()
        {
            Enabled = false;
            OnDisabled();
        }
    }
}