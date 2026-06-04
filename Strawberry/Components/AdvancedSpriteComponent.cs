using Strawberry.Core;
using Strawberry.Graphics.Layers;
using Strawberry.Graphics;
using Strawberry.Serialization;
using Strawberry.Math;
using Strawberry.EventSystem;

namespace Strawberry.Components
{
    public class AdvancedSpriteComponent : BaseComponent
    {
        public struct AdvancedAnimationEndEvent : IStrawberryEvent
        {
            public AdvancedSpriteComponent Sprite;
        }

        private class AnimationInfo
        {
            public Sprite Sprite;
            public float ImageSpeed;
            public float ImageIndex;
            public int RealImageIndex;
            public bool Loop;
            public string Name;

            public int ImageCount => Sprite?.ImageCount ?? 0;

            public AnimationInfo(string name, Sprite sprite)
            {
                Sprite = sprite;
                ImageSpeed = 1.0f;
                ImageIndex = 0;
                RealImageIndex = 0;
                Name = name;
            }
        }

        public SpriteLayer Layer { get; set; }
        public TransformComponent Transform { get; private set; }
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public bool Visible { get; set; } = true;
        public Color Color { get; set; } = Color.White;
        public Vector2 CustomPosition { get; set; } = Vector2.Zero;
        public List<string> DisabledViewports { get; } = new List<string>();

        private Dictionary<string, AnimationInfo> animations = new Dictionary<string, AnimationInfo>();
        private AnimationInfo currentAnimation;
        private bool isInitialized = false;

        public string CurrentAnimation
        {
            get => currentAnimation?.Name ?? "";
            set
            {
                if (animations.TryGetValue(value, out var anim))
                {
                    currentAnimation = anim;
                }
            }
        }

        public int ImageIndex
        {
            get => currentAnimation?.RealImageIndex ?? 0;
            set
            {
                if (currentAnimation != null)
                {
                    currentAnimation.RealImageIndex = System.Math.Clamp(value, 0, currentAnimation.ImageCount - 1);
                    currentAnimation.ImageIndex = currentAnimation.RealImageIndex;
                }
            }
        }

        public Sprite CurrentSprite => currentAnimation?.Sprite;

        public void Begin()
        {
            if (!isInitialized)
            {
                Transform = Owner.GetComponent<TransformComponent>();
                isInitialized = true;
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (currentAnimation == null) return;

            // Update animation frame
            currentAnimation.ImageIndex += currentAnimation.ImageSpeed * FrameInfo.Information.DeltaTime;

            // Handle frame boundaries
            if (currentAnimation.ImageIndex >= currentAnimation.ImageCount)
            {
                if (currentAnimation.Loop)
                {
                    currentAnimation.ImageIndex -= currentAnimation.ImageCount;
                }
                else
                {
                    currentAnimation.ImageIndex = currentAnimation.ImageCount - 1;
                    EventManager.Invoke<AdvancedAnimationEndEvent>(this, new AdvancedAnimationEndEvent()
                    {
                        Sprite = this
                    });
                }
            }

            currentAnimation.RealImageIndex = (int)currentAnimation.ImageIndex;
        }

        public override void OnRender()
        {
            if (CurrentSprite == null || !Visible) return;

            if (Layer != null && !DisabledViewports.Contains(GameContext.GraphicsContext.ActiveViewport.Name))
            {
                Vector2 position = CustomPosition != Vector2.Zero ? CustomPosition : Transform.Position;
                Layer.Push(
                    CurrentSprite,
                    position,
                    Origin,
                    Transform.Scale,
                    Color,
                    currentAnimation.RealImageIndex,
                    Transform.Angle
                );
            }
        }

        public void AddSprite(string animationKey, Sprite sprite)
        {
            if (string.IsNullOrEmpty(animationKey))
                throw new ArgumentException("Animation key cannot be empty");

            if (sprite == null)
                throw new ArgumentNullException(nameof(sprite));

            animations[animationKey] = new AnimationInfo(animationKey, sprite);

            // Set first animation as current if none exists
            if (currentAnimation == null)
            {
                CurrentAnimation = animationKey;
            }
        }

        public void PlayAnimation(string animationKey, bool loop = true)
        {
            if (animations.TryGetValue(animationKey, out var anim))
            {
                currentAnimation = anim;
                currentAnimation.Loop = loop;
            }
            else
            {
                throw new KeyNotFoundException($"Animation '{animationKey}' not found");
            }
        }

        public void PlayAnimation(string animationKey, float speed, bool loop = true)
        {
            PlayAnimation(animationKey, loop);
            currentAnimation.ImageSpeed = speed;
        }

        public void PlayAnimation(string animationKey, float speed, int imageIndex, bool loop = true)
        {
            PlayAnimation(animationKey, speed, loop);
            currentAnimation.RealImageIndex = System.Math.Clamp(imageIndex, 0, currentAnimation.ImageCount - 1);
            currentAnimation.ImageIndex = currentAnimation.RealImageIndex;
        }

        public int ImageIndexOf(string animationKey)
            => animations.TryGetValue(animationKey, out var anim) ? anim.RealImageIndex : -1;

        public int ImageCountOf(string animationKey)
            => animations.TryGetValue(animationKey, out var anim) ? anim.ImageCount : -1;

        public void SetAnimationSpeed(float speed)
        {
            if (currentAnimation != null)
                currentAnimation.ImageSpeed = speed;
        }

        public void SetLoop(bool loop)
        {
            if (currentAnimation != null)
                currentAnimation.Loop = loop;
        }
    }
}