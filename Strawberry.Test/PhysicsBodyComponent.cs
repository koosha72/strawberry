using Strawberry.Components;
using Strawberry.Core;
using Strawberry.Math;
using tainicom.Aether.Physics2D.Dynamics;

namespace Strawberry.Test
{
    public class PhysicsBodyComponent : BaseComponent
    {

        public TransformComponent Transform { get { return Owner.GetComponent<TransformComponent>(); } }

        public SpriteComponent Sprite { get { return Owner.GetComponent<SpriteComponent>(); } }

        public float Direction { get; set; }

        Body body;

        public void Begin()
        {
            body = Scene.PhysicsWorld.CreateBody(Transform.Position / MyGameContext.ppm, -(float)MathHelper.DegToRad(Transform.Angle), BodyType.Dynamic);
            var size = (Sprite.Sprite.Size * Transform.Scale) / MyGameContext.ppm;
            var f = body.CreateRectangle(size.X, size.Y, 1f, new Vector2(0.5f,0.5f));
            body.Mass = 0.2f;
            body.SleepingAllowed = false;
        }

        public override void OnFixedUpdate()
        {
            Transform.Position = body.Position * MyGameContext.ppm;
            Transform.Angle = -(float)MathHelper.RadToDeg(body.Rotation);
        }
    }
}
