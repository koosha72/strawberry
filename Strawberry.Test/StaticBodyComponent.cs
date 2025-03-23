using Strawberry.Components;
using Strawberry.Core;
using tainicom.Aether.Physics2D.Dynamics;

namespace Strawberry.Test
{
    public class StaticBodyComponent : BaseComponent
    {
        public TransformComponent Transform { get { return Owner.GetComponent<TransformComponent>(); } }

        public SpriteComponent Sprite { get { return Owner.GetComponent<SpriteComponent>(); } }

        public float Direction { get; set; }

        Body body;

        public void Begin()
        {
            body = Scene.PhysicsWorld.CreateBody(Transform.Position / MyGameContext.ppm, 0, BodyType.Static);
            var size = (Sprite.Sprite.Size * Transform.Scale) / MyGameContext.ppm;
            var f = body.CreateRectangle(size.X, size.Y, 1f, new Math.Vector2(2f,0.5f));
            f.Restitution = 0.3f;
            f.Friction = 0.5f;
        }

        public override void OnFixedUpdate()
        {
            body.Position = Transform.Position / MyGameContext.ppm;
        }
    }
}
