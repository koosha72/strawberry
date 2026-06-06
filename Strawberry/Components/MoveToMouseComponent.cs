/*
 * Strawberry Game Engine
 * File: MoveToMouseComponent.cs
 * Author: Koosha Aabedini Nassab
 *
 * Component that moves an entity to the current mouse pointer position each update.
 */

using Strawberry.Core;

namespace Strawberry.Components
{
    /// <summary>
    /// A simple component that jumps the owner to the mouse position
    /// </summary>
    public class MoveToMouseComponent : BaseComponent
    {
        /// <summary>
        /// Gets the transform of the owner
        /// </summary>
        public TransformComponent Transform { get { return Owner.GetComponent<TransformComponent>(); } }

        public override void OnUpdate()
        {
            if (Transform != null)
            {
                Transform.Position = GameContext.InputManager.PointingDevice.GetPosition(0);
            }
        }
    }
}
