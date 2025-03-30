using Strawberry.Components;
using Strawberry.Core;
using Strawberry.Graphics.Text;
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
            var f = body.CreateRectangle(size.X, size.Y, 1f, new Math.Vector2(2f, 0.5f));
            f.Restitution = 0.3f;
            f.Friction = 0.5f;
        }

        public override void OnFixedUpdate()
        {
            body.Position = Transform.Position / MyGameContext.ppm;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (GameContext.InputManager.PointingDevice.IsButtonPressed(0, Input.PointerButtons.Primary))
            {
                var pos = GameContext.InputManager.PointingDevice.GetPosition(0);
                if (pos.X > 320)
                    GameContext.InputManager.Keyboard.FirePressed(Input.Keys.Space);
            }

            if (GameContext.InputManager.PointingDevice.IsButtonReleased(0, Input.PointerButtons.Primary))
            {
                if (GameContext.InputManager.Keyboard.IsKeyDown(Input.Keys.Space))
                {
                    GameContext.InputManager.Keyboard.FireReleased(Input.Keys.Space);
                }
            }

            if (GameContext.InputManager.Keyboard.IsKeyDown(Input.Keys.Space))
            {
                Transform.Position += new Math.Vector2(0f, 1f * MyGameContext.ppm) * FrameInfo.Information.DeltaTime;
            }

            var tc = Owner.GetComponent<TextRendererComponent>();
            if (tc != null)
            {
                tc.Text = "فریم بر ثانیه: " + FrameInfo.Information.FPS;
            }
        }
    }
}
