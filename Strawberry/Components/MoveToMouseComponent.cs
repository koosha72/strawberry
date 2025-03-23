using Strawberry.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strawberry.Components
{
    public class MoveToMouseComponent : BaseComponent
    {
        public TransformComponent Transform { get { return Owner.GetComponent<TransformComponent>(); } }

        public float Direction { get; set; }

        public void Begin()
        {
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
