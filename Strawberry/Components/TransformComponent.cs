using Strawberry.Core;
using Strawberry.Math;
using Strawberry.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strawberry.Components
{
    public enum PositionType
    {
        RelativeToParent,
        Global
    }

    public class TransformComponent : BaseComponent
    {
        Vector2 localPosition = new Vector2();

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

        public Vector2 Scale { get; set; }

        float angle = 0;

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

        public double Direction
        {
            get { return Angle; }
        }

        public bool HeadAsParent { get; set; }

        public PositionType PositionType { get; set; } = PositionType.RelativeToParent;

        public Vector2 Origin { get; set; }

        public bool RotateAroundParent { get; set; }

        /*[SceneEditorBounds]
        public RotatedRectangle Bounds { get { return new RotatedRectangle(Position, new Vector2(16f, 16f), Angle); } }*/

        public TransformComponent()
        {
            this.Scale = new Vector2(1f, 1f);
            this.PositionType = PositionType.Global;
        }
    }
}
