/*
 * Strawberry Game Engine
 * File: AdvancedSpriteComponent.cs
 * Author: Koosha Aabedini Nassab
 *
 * Advanced sprite component with keyframe animations and event firing.
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
    public struct AdvancedAnimationEndEvent : IStrawberryEvent
    {
        /// <summary>
        /// The sprite component that ended its animation (The event caller).
        /// </summary>
        public AdvancedSpriteComponent Sprite;
    }

    /// <summary>
    /// An advanced implementation of the sprite component, which supports keyframe animations.
    /// </summary>
    public class AdvancedSpriteComponent : BaseComponent
    {
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
        /// <summary>
        /// Gets or sets the layer on which the sprite will be drawn.
        /// </summary>
        public SpriteLayer Layer { get; set; }
        /// <summary>
        /// Gets the transform component of the owner.
        /// </summary>
        public TransformComponent Transform { get; private set; }
        /// <summary>
        /// Gets the origin of the sprite. It is the same as the origin of the owner's transform component.
        /// </summary>
        public Vector2 Origin { get { return Transform.Origin; } }
        /// <summary>
        /// Gets or sets the visibility of the sprite. Default is true.
        /// </summary>
        public bool Visible { get; set; } = true;
        /// <summary>
        /// Gets or sets the color using which the sprite is drawn. Default is white.
        /// </summary>
        public Color Color { get; set; } = Color.White;

        [DoNotSerialize]
        public Vector2 CustomPosition { get; set; } = Vector2.Zero;

        /// <summary>
        /// Gets a list of viewports on which the sprite will not be visible. If empty, it will be visible on all viewports.
        /// </summary>
        public List<string> DisabledViewports { get; } = new List<string>();

        private Dictionary<string, AnimationInfo> animations = new Dictionary<string, AnimationInfo>();
        private AnimationInfo currentAnimation;
        private bool isInitialized = false;

        /// <summary>
        /// Gets or sets the current animation of the sprite.
        /// </summary>
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
        /// <summary>
        /// Gets or sets current frame of the playing animation.
        /// </summary>
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

        /// <summary>
        /// Gets the current sprite of the animation. It will change if you change the animation
        /// </summary>
        public Sprite CurrentSprite => currentAnimation?.Sprite;

        public override void OnBegin()
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
            currentAnimation.ImageIndex += (currentAnimation.ImageSpeed * currentAnimation.ImageCount) * FrameInfo.Information.DeltaTime;

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

        /// <summary>
        /// Adds an animation key to the component.
        /// </summary>
        /// <param name="animationKey">The key of the animation.</param>
        /// <param name="sprite">The sprite to use for the animation.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
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
        /// <summary>
        /// Plays an animation by key
        /// </summary>
        /// <param name="animationKey">The key of the animation to play</param>
        /// <param name="loop">Indicates whether the animation should loop. Default is true</param>
        /// <exception cref="KeyNotFoundException">If the animation is not found</exception>
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
        /// <summary>
        /// Plays an animation by key using the provided speed (1 = a complete cycle of animation per second)
        /// </summary>
        /// <param name="animationKey">The key of the animation to play</param>
        /// <param name="speed">The speed of the animation (1 = a complete cycle of animation per second)</param>
        /// <param name="loop">Indicates whether the animation should loop. Default is true</param>
        /// <exception cref="KeyNotFoundException">If the animation is not found</exception>
        public void PlayAnimation(string animationKey, float speed, bool loop = true)
        {
            PlayAnimation(animationKey, loop);
            currentAnimation.ImageSpeed = speed;
        }
        /// <summary>
        /// Plays an animation by key using the provided speed (1 = a complete cycle of animation per second) and starting at a specific image index
        /// </summary>
        /// <param name="animationKey">The key of the animation to play</param>
        /// <param name="speed">The speed of the animation (1 = a complete cycle of animation per second)</param>
        /// <param name="imageIndex">The starting image index of the animation</param>
        /// <param name="loop">Indicates whether the animation should loop. Default is true</param>
        /// <exception cref="KeyNotFoundException">If the animation is not found</exception>
        public void PlayAnimation(string animationKey, float speed, int imageIndex, bool loop = true)
        {
            PlayAnimation(animationKey, speed, loop);
            currentAnimation.RealImageIndex = System.Math.Clamp(imageIndex, 0, currentAnimation.ImageCount - 1);
            currentAnimation.ImageIndex = currentAnimation.RealImageIndex;
        }

        /// <summary>
        /// Returns the image index of an animation by key
        /// </summary>
        /// <param name="animationKey">The key of the animation</param>
        /// <returns>The current frame of the animation</returns>
        public int ImageIndexOf(string animationKey)
            => animations.TryGetValue(animationKey, out var anim) ? anim.RealImageIndex : -1;

        /// <summary>
        /// Returns the total number of frames in an animation by key
        /// </summary>
        /// <param name="animationKey">The animation key</param>
        /// <returns>The total number of frames in the animation</returns>
        public int ImageCountOf(string animationKey)
            => animations.TryGetValue(animationKey, out var anim) ? anim.ImageCount : -1;
        /// <summary>
        /// Sets the playing speed of current playing animation (1 = a complete cycle of animation per second)
        /// </summary>
        /// <param name="speed">The new playing speed (1 = a complete cycle of animation per second)</param>
        public void SetAnimationSpeed(float speed)
        {
            if (currentAnimation != null)
                currentAnimation.ImageSpeed = speed;
        }
        /// <summary>
        /// Sets whether the current playing animation should loop or not
        /// </summary>
        /// <param name="loop"></param>
        public void SetLoop(bool loop)
        {
            if (currentAnimation != null)
                currentAnimation.Loop = loop;
        }
    }
}