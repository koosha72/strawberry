/*
 * Strawberry Game Engine
 * File: SpriteComponent.cs
 * Author: Koosha Aabedini Nassab
 *
 * Renders a sprite and drives animation playback for entities.
 */

using Strawberry.Core;
using Strawberry.Graphics.Layers;
using Strawberry.Graphics;
using Strawberry.Serialization;
using Strawberry.Math;
using Strawberry.EventSystem;

namespace Strawberry.Components
{
    /// <summary>
    /// This event occurs every time an animation ends.
    /// </summary>
    public struct AnimationEndEvent : IStrawberryEvent
    {
        /// <summary>
        /// The sprite component that ended its animation (The event caller).
        /// </summary>
        public SpriteComponent Sprite;
    }
    /// <summary>
    /// The sprite component is used to render a single sprite on the screen.
    /// </summary>
    public class SpriteComponent : BaseComponent
    {
        float imageIndex = 0f;

        int realImageIndex = 0;
        /// <summary>
        /// Gets or sets the sprite to render on the screen. If null, no image will be rendered.
        /// </summary>
        public Sprite Sprite { get; set; }
        /// <summary>
        /// Gets or sets the layer on which the sprite will be rendered.
        /// </summary>
        public SpriteLayer Layer { get; set; }
        /// <summary>
        /// Gets the transform of the owner.
        /// </summary>
        public TransformComponent Transform { get; private set; }
        /// <summary>
        /// Gets or sets the speed of playing animations (1 = a complete cycle per second).  Default is 1.
        /// </summary>
        public float ImageSpeed { get; set; } = 1;
        /// <summary>
        /// Gets the number of images in the sprite.
        /// </summary>
        public float ImageCount => Sprite != null ? Sprite.ImageCount : 0;
        /// <summary>
        /// Gets or sets whether the sprite should loop or not.
        /// </summary>
        public bool Loop { get; set; } = true;
        /// <summary>
        /// Gets the origin of the sprite. It is the same as the origin of the owner's transform component.
        /// </summary>
        public Vector2 Origin { get { return Transform.Origin; } }
        /// <summary>
        /// Gets or sets the visibility of the sprite. Default is true.
        /// </summary>
        public bool Visible { get; set; } = true;
        /// <summary>
        /// Gets or sets the current frame of the animation.
        /// </summary>
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
        /// <summary>
        /// Gets or sets color using which the sprite is drawn. Default is white.
        /// </summary>
        public Color Color { get; set; } = Color.White;

        [DoNotSerialize]
        public Vector2 CustomPosition { get; set; } = new Vector2();
        /// <summary>
        /// Gets a list of viewports on which the sprite will not be visible. If empty, it will be visible on all viewports.
        /// </summary>
        public List<string> DisabledViewports { get; private set; } = new List<string>();

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

        public override void OnBegin()
        {
            if (Transform == null)
                Transform = Owner.GetComponent<TransformComponent>();
        }

        public override void OnComponentAdded(BaseComponent component)
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
