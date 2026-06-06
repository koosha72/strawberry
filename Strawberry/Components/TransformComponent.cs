/*
 * Strawberry Game Engine
 * File: TransformComponent.cs
 * Author: Koosha Aabedini Nassab
 *
 * Handles entity transform state for position, rotation, scaling, and parent-relative movement.
 */

using Strawberry.Core;
using Strawberry.Math;
using Strawberry.Serialization;

namespace Strawberry.Components
{
    /// <summary>
    /// Specifies the coordinate space used to determine the position of a transform.
    /// </summary>
    public enum PositionType
    {
        /// <summary>
        /// The position is relative to the parent entity's transform.
        /// </summary>
        RelativeToParent,
        
        /// <summary>
        /// The position is in global (world) coordinates.
        /// </summary>
        Global
    }

    /// <summary>
    /// Manages the 2D transformation of an entity, including position, rotation, scale, and origin.
    /// Supports parent-child hierarchies, allowing transformations to be inherited or orbit around a parent.
    /// </summary>
    public class TransformComponent : BaseComponent
    {
        Vector2 localPosition = new Vector2();

        /// <summary>
        /// Gets or sets the world position of the entity. 
        /// When getting, this calculates the final world position based on the <see cref="PositionType"/> and parent hierarchy.
        /// When setting, this updates the <see cref="LocalPosition"/>.
        /// </summary>
        [DoNotSerialize]
        public Vector2 Position
        {
            set
            {
                LocalPosition = new Vector2(value.X, value.Y);
            }
            get
            {
                if (PositionType == PositionType.RelativeToParent)
                {
                    if (Owner.Parent != null)
                    {
                        TransformComponent trans = Owner.Parent.GetComponent<TransformComponent>();
                        if (trans != null)
                        {
                            Vector2 temp = new Vector2(LocalPosition);
                            if (RotateAroundParent)
                                temp.Direction += trans.Angle;
                            return new Vector2(temp.X + trans.Position.X, temp.Y + trans.Position.Y);
                        }
                        else
                            return LocalPosition;
                    }
                    else
                        return LocalPosition;
                }
                else
                    return LocalPosition;
            }
        }


        /// <summary>
        /// Gets or sets the local position of the entity, relative to its parent.
        /// If the entity has no parent, this is equivalent to the global position.
        /// </summary>
        public Vector2 LocalPosition
        {
            get
            {
                return localPosition;
            }
            set
            {
                localPosition = value;
            }
        }

        /// <summary>
        /// Gets or sets the 2D scale of the entity.
        /// </summary>
        public Vector2 Scale { get; set; }

        float angle = 0;

        /// <summary>
        /// Gets or sets the rotation angle of the entity.
        /// If <see cref="HeadAsParent"/> is true and a parent exists, getting this property returns the parent's angle.
        /// </summary>
        public float Angle
        {
            get
            {
                if (HeadAsParent)
                {
                    if (Owner.Parent != null)
                    {
                        TransformComponent trans = Owner.Parent.GetComponent<TransformComponent>();
                        if (trans != null)
                            return trans.Angle;
                    }
                }
                return angle;
            }
            set
            {
                angle = value;
            }
        }

        /// <summary>
        /// Gets the rotation direction of the entity as a double, equivalent to <see cref="Angle"/>.
        /// </summary>
        public double Direction
        {
            get { return Angle; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this entity should inherit the rotation angle from its parent entity.
        /// </summary>
        public bool HeadAsParent { get; set; }

        /// <summary>
        /// Gets or sets the coordinate space used for this transform's position.
        /// Defaults to <see cref="PositionType.Global"/> upon construction.
        /// </summary>
        public PositionType PositionType { get; set; } = PositionType.RelativeToParent;

        /// <summary>
        /// Gets or sets the origin point (pivot) for rotation and scaling.
        /// </summary>
        public Vector2 Origin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the local position should be rotated by the parent's angle 
        /// before applying the parent's offset. This creates an orbiting effect around the parent.
        /// </summary>
        public bool RotateAroundParent { get; set; }

        /*[SceneEditorBounds]
        public RotatedRectangle Bounds { get { return new RotatedRectangle(Position, new Vector2(16f, 16f), Angle); } }*/

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformComponent"/> class.
        /// Sets the default scale to (1, 1) and the <see cref="PositionType"/> to <see cref="PositionType.Global"/>.
        /// </summary>
        public TransformComponent()
        {
            this.Scale = new Vector2(1f, 1f);
            this.PositionType = PositionType.Global;
        }
    }
}