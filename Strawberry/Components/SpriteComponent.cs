using Strawberry.Core;
using Strawberry.Graphics.Layers;
using Strawberry.Graphics;
using Strawberry.Serialization;
using Strawberry.Math;
using Strawberry.EventSystem;

namespace Strawberry.Components
{
    public struct AnimationEndEvent : IStrawberryEvent
    {
        public SpriteComponent Sprite;
    }

    public class SpriteComponent : BaseComponent
    {
        float imageIndex = 0f;

        int realImageIndex = 0;

        public Sprite Sprite { get; set; }

        public SpriteLayer Layer { get; set; }

        public TransformComponent Transform { get; private set; }

        public float ImageSpeed { get; set; } = 1;

        public float ImageCount => Sprite != null ? Sprite.ImageCount : 0;

        public bool Loop { get; set; } = true;

        public Vector2 Origin { get { return Transform.Origin; } }

        public bool Visible { get; set; } = true;

        public int ImageIndex
        {
            get { return this.realImageIndex; }
            set
            {
                realImageIndex = value; imageIndex = value;
                if (realImageIndex >= ImageCount)
                {
                    realImageIndex = 0;
                    imageIndex = realImageIndex;
                }
                if (realImageIndex < 0)
                {
                    realImageIndex = (int)ImageCount - 1;
                    imageIndex = realImageIndex;
                }
            }
        }

        public Color Color { get; set; } = Color.White;

        [DoNotSerialize]
        public Vector2 CustomPosition { get; set; } = new Vector2();

        public List<string> DisabledViewports { get; private set; } = new List<string>();


        /*[SceneEditorBounds]
        public RotatedRectangle Bounds
        {
            get
            {
                if (Sprite != null)
                {
                    return new RotatedRectangle(Transform.Position,
                            new Vector2(Sprite.Size.X * Transform.Scale.X, Sprite.Size.Y * Transform.Scale.Y),
                            Transform.Angle, new Vector2(Origin.X * Transform.Scale.X, Origin.Y * Transform.Scale.Y));
                }
                else
                    return Transform.Bounds;
            }
        }*/

        public SpriteComponent()
        {
        }

        public void Setup(Sprite sprite, float imageSpeed, Vector2 origin, Color color, SpriteLayer layer)
        {
            this.Sprite = sprite;
            this.ImageSpeed = imageSpeed;
            Transform.Origin = origin;
            this.Color = color;
            this.Layer = layer;
        }

        public void Begin()
        {
            if (Transform == null)
                Transform = Owner.GetComponent<TransformComponent>();
            Owner.RegisterEvent<Action>("AnimationEnd");
        }

        public void ComponentAdded(BaseComponent component)
        {
            if (Transform == null)
                Transform = Owner.GetComponent<TransformComponent>();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (Sprite != null)
            {
                if (realImageIndex <= Sprite.ImageCount - 1)
                {
                    imageIndex += (ImageSpeed * Sprite.ImageCount) * FrameInfo.Information.DeltaTime;
                }

                if (imageIndex >= Sprite.ImageCount)
                {
                    if (Loop)
                    {
                        imageIndex -= Sprite.ImageCount;
                    }
                    else
                    {
                        imageIndex = Sprite.ImageCount - 1;
                    }

                    Owner.InvokeEvents("AnimationEnd");
                    EventManager.Invoke<AnimationEndEvent>(this, new AnimationEndEvent()
                    {
                        Sprite = this
                    });
                }

                realImageIndex = (int)(imageIndex);
            }
        }

        public override void OnRender()
        {
            if (Sprite != null && Visible)
            {
                if (Layer != null)
                {
                    if (!DisabledViewports.Contains(GameContext.GraphicsContext.ActiveViewport.Name))
                    {
                        if (CustomPosition.X == 0 && CustomPosition.Y == 0)
                        {
                            Layer.Push(Sprite, Transform.Position, Origin,
                                Transform.Scale, Color, realImageIndex, Transform.Angle);
                        }
                        else
                        {
                            Layer.Push(Sprite, CustomPosition, Origin,
                                Transform.Scale, Color, realImageIndex, Transform.Angle);
                        }
                    }
                }
            }
        }
    }
}
