/*
 * Strawberry Game Engine
 * File: MoveToMouseComponent.cs
 * Author: Koosha Aabedini Nassab
 *
 * Component that moves an entity to the current mouse pointer position each update.
 */

using Strawberry.Core;

namespace Strawberry.Components.Movement
{
    /// <summary>
    /// A simple component that jumps the owner to the mouse position
    /// </summary>
    public class JumpToMouseComponent : BaseComponent
    {
        /// <summary>
        /// Gets the transform of the owner
        /// </summary>
        public TransformComponent Transform { get; private set; }

        public override void OnComponentAdded(BaseComponent component)
        {
            base.OnComponentAdded(component);
            if (component is TransformComponent transform && Transform == null)
                Transform = transform;
        }

        public override void OnUpdate()
        {
            if (Transform != null)
            {
                Transform.Position = GameContext.InputManager.PointingDevice.GetPosition(0);
            }
        }
    }
}
