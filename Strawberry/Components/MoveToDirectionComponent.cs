using Strawberry.Core;
using Strawberry.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strawberry.Components
{
    public class MoveToDirectionComponent : BaseComponent
    {
        public float Speed { get; set; }

        public bool HeadToGoal { get; set; }

        public TransformComponent Transform { get { return Owner.GetComponent<TransformComponent>(); } }

        public float Direction { get; set; }

        public void Begin()
        {
        }

        public void Update()
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
