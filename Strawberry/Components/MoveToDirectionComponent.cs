/*
 * Strawberry Game Engine
 * File: MoveToDirectionComponent.cs
 * Author: Koosha Aabedini Nassab
 *
 * Component that moves an entity in a fixed direction at a configurable speed.
 */

using Strawberry.Core;
using Strawberry.Math;

namespace Strawberry.Components
{
    /// <summary>
    /// A simple component that moves the owner to a direction at a given speed.
    /// </summary>
    public class MoveToDirectionComponent : BaseComponent
    {
        /// <summary>
        /// Gets or sets speed of the movement per second
        /// </summary>
        public float Speed { get; set; }
        /// <summary>
        /// Gets or sets the heading of the owner. If true the owner will rotate to face its direction of movement
        /// </summary>
        public bool HeadToGoal { get; set; }
        /// <summary>
        /// Gets the transform of the owner
        /// </summary>
        public TransformComponent Transform { get { return Owner.GetComponent<TransformComponent>(); } }
        /// <summary>
        /// Gets or sets the direction of movement in degrees.
        /// </summary>
        public float Direction { get; set; }

        public override void OnUpdate()
        {
            if (Transform != null)
            {
                Vector2 towards = new Vector2(1f, 0f);
                towards.Direction = Direction;
                Transform.Position += towards * Speed * FrameInfo.Information.DeltaTime;
                if (HeadToGoal)
                    Transform.Angle = (float)towards.Direction;
            }
        }
    }
}
