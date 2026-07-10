/*
 * Strawberry Game Engine
 * File: ParticleLayer.cs
 * Author: Koosha Aabedini Nassab
 *
 * Layer that manages particle emitters and updates/renders particles.
 */

using Strawberry.Core;
using Strawberry.Graphics.ParticleSystem;
using Strawberry.Math;

namespace Strawberry.Graphics.Layers
{
    /// <summary>
    /// A layer responsible for managing and rendering particle emitters within a scene.
    /// </summary>
    public class ParticleLayer : Layer
    {
        int maxBatchCount = 2048;

        // Particle pool
        Particle[] particles;
        int maxParticles = 10000;
        int aliveCount = 0;

        // Emitters
        List<ParticleEmitter> emitters = new List<ParticleEmitter>();

        // Rendering
        List<SpriteQuad> quadList;
        Geometry<VertexPositionTexColor> geometry;
        VertexPositionTexColor[] vertices;
        uint[] indices;
        BasicShader shader;
        string blendName = "Default";

        /// <summary>
        /// Gets the graphics context associated with the current scene.
        /// </summary>
        public IGraphicsContext GraphicsContext { get { return Scene.GameContext.GraphicsContext; } }

        /// <summary>
        /// Gets or sets the number of draw calls performed during the last render.
        /// </summary>
        public int DrawCalls { get; set; }

        /// <summary>
        /// Gets the number of live particles currently active in the layer.
        /// </summary>
        public int AliveParticleCount => aliveCount;

        /// <summary>
        /// Gets the number of particle emitters attached to this layer.
        /// </summary>
        public int EmitterCount => emitters.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleLayer"/> class.
        /// </summary>
        public ParticleLayer() { }

        /// <summary>
        /// Initializes the particle layer, allocates buffers, and prepares the geometry.
        /// </summary>
        /// <param name="scene">The scene that owns the layer.</param>
        public override void Initialize(Scene scene)
        {
            base.Initialize(scene);

            particles = new Particle[maxParticles];
            quadList = new List<SpriteQuad>();

            vertices = new VertexPositionTexColor[maxBatchCount * 4];
            indices = new uint[maxBatchCount * 6];
            int index = 0;
            for (uint i = 0; i < maxBatchCount * 4; i += 4)
            {
                indices[index] = i;
                indices[index + 1] = i + 1;
                indices[index + 2] = i + 2;
                indices[index + 3] = i + 2;
                indices[index + 4] = i + 3;
                indices[index + 5] = i;
                index += 6;
            }

            geometry = GraphicsContext.CreateGeometry<VertexPositionTexColor>(vertices, indices,
                GeometryType.Dynamic, GeometryType.Static);

            shader = new BasicShader(GraphicsContext, VertexElementContainer.VertexPositionTexColor);
        }

        // ── Emitter Management ─────────────────────────────────────

        /// <summary>
        /// Creates and adds a new particle emitter to the layer.
        /// </summary>
        /// <param name="sprite">The sprite used by the emitter.</param>
        /// <param name="initiator">The particle initiator that generates particles.</param>
        /// <param name="timeline">The particle timeline that updates particle properties.</param>
        /// <returns>The created emitter.</returns>
        public ParticleEmitter AddEmitter(Sprite sprite, ParticleInitiator initiator, ParticleTimeline timeline)
        {
            var emitter = new ParticleEmitter(sprite, initiator, timeline)
            {
                EmitterIndex = emitters.Count
            };
            emitters.Add(emitter);
            return emitter;
        }

        /// <summary>
        /// Adds an existing particle emitter to the layer.
        /// </summary>
        /// <param name="emitter">The emitter to add.</param>
        /// <returns>The emitter that was added.</returns>
        public ParticleEmitter AddEmitter(ParticleEmitter emitter)
        {
            emitter.EmitterIndex = emitters.Count;
            emitters.Add(emitter);
            return emitter;
        }

        /// <summary>
        /// Removes a particle emitter from the layer.
        /// </summary>
        /// <param name="emitter">The emitter to remove.</param>
        /// <returns>True if the emitter was removed; otherwise false.</returns>
        public bool RemoveEmitter(ParticleEmitter emitter)
        {
            bool removed = emitters.Remove(emitter);
            if (removed)
                RebuildEmitterIndices();
            return removed;
        }

        /// <summary>
        /// Gets the particle emitter at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the emitter.</param>
        /// <returns>The emitter if found; otherwise null.</returns>
        public ParticleEmitter GetEmitter(int index)
        {
            if (index >= 0 && index < emitters.Count)
                return emitters[index];
            return null;
        }

        /// <summary>
        /// Removes all emitters and deactivates all live particles.
        /// </summary>
        public void ClearEmitters()
        {
            emitters.Clear();
            aliveCount = 0;
            for (int i = 0; i < maxParticles; i++)
                particles[i].Alive = false;
        }

        void RebuildEmitterIndices()
        {
            for (int i = 0; i < emitters.Count; i++)
                emitters[i].EmitterIndex = i;

            // Update all living particles to point to the correct emitter
            for (int i = 0; i < maxParticles; i++)
            {
                if (particles[i].Alive)
                {
                    // If the emitter was removed, mark the particle as dead
                    if (particles[i].EmitterIndex < 0 || particles[i].EmitterIndex >= emitters.Count)
                        particles[i].Alive = false;
                }
            }
        }

        // ── Blend Mode ──────────────────────────────────────────────

        /// <summary>
        /// Sets the blend mode name used when rendering particles.
        /// </summary>
        /// <param name="name">The blend mode identifier.</param>
        public void SetBlendMode(string name)
        {
            blendName = name;
        }

        // ── Update ──────────────────────────────────────────────────

        /// <summary>
        /// Updates all particle emitters and active particles.
        /// </summary>
        public override void Update()
        {
            if (!Enabled)
                return;

            float dt = FrameInfo.Information.DeltaTime;

            // Emit new particles from each emitter
            for (int e = 0; e < emitters.Count; e++)
                emitters[e].Emit(particles, maxParticles);

            // Update all alive particles and compact the array
            UpdateAndCompact(dt);

            // Remove finished auto-destroy emitters
            for (int i = emitters.Count - 1; i >= 0; i--)
            {
                if (emitters[i].AutoDestroy && emitters[i].IsFinished)
                {
                    emitters.RemoveAt(i);
                    RebuildEmitterIndices();
                }
            }
        }

        void UpdateAndCompact(float dt)
        {
            aliveCount = 0;

            for (int i = 0; i < maxParticles; i++)
            {
                if (!particles[i].Alive)
                    continue;

                // Advance age
                particles[i].Age += dt;

                // Kill expired particles
                if (particles[i].Age >= particles[i].Lifetime)
                {
                    particles[i].Alive = false;
                    // Notify emitter
                    int emitterIdx = particles[i].EmitterIndex;
                    if (emitterIdx >= 0 && emitterIdx < emitters.Count)
                        emitters[emitterIdx].OnParticleDied();
                    continue;
                }

                // Apply timeline from the owning emitter
                int idx = particles[i].EmitterIndex;
                if (idx >= 0 && idx < emitters.Count)
                    emitters[idx].UpdateParticle(ref particles[i], dt);

                // Compact: swap alive particle to the front
                if (i != aliveCount)
                {
                    particles[aliveCount] = particles[i];
                    particles[i].Alive = false;
                }
                aliveCount++;
            }
        }

        // ── Render ──────────────────────────────────────────────────

        /// <summary>
        /// Renders all active particles in the layer.
        /// </summary>
        public override void Render()
        {
            if (!Enabled)
                return;
            if (!Viewports.Contains(GraphicsContext.ActiveViewport.Name))
                return;
            if (aliveCount == 0)
                return;

            // Build quads from alive particles
            quadList.Clear();
            for (int i = 0; i < aliveCount; i++)
            {
                BuildQuad(ref particles[i]);
            }

            if (quadList.Count == 0)
                return;

            // Sort if a sorter is assigned
            if (Sorter != null)
                Sorter.Sort(quadList);

            // Batch render (same pattern as SpriteLayer)
            int count = 0;
            int v = 0;
            SpriteQuad temp = null;

            foreach (SpriteQuad spr in quadList)
            {
                if (temp == null)
                {
                    temp = spr;
                    temp.Shader.Activate();
                    temp.Shader.Projection = Matrix4.CreateOrthographic(
                        GraphicsContext.ActiveViewport.ScenePos.X,
                        GraphicsContext.ActiveViewport.SceneSize.X + GraphicsContext.ActiveViewport.ScenePos.X,
                        GraphicsContext.ActiveViewport.SceneSize.Y + GraphicsContext.ActiveViewport.ScenePos.Y,
                        GraphicsContext.ActiveViewport.ScenePos.Y, 0, 1);
                    temp.Shader.SetTexture(spr.Texture);
                    GraphicsContext.ActivateBlendMode(spr.BlendName);
                }
                else if (temp != spr)
                {
                    // Flush current batch
                    geometry.UpdateVB(vertices);
                    geometry.Render();
                    DrawCalls++;
                    count = 0;
                    v = 0;
                    Array.Clear(vertices, 0, vertices.Length);

                    temp = spr;
                    spr.Shader.Activate();
                    spr.Shader.Projection = Matrix4.CreateOrthographic(
                        GraphicsContext.ActiveViewport.ScenePos.X,
                        GraphicsContext.ActiveViewport.SceneSize.X + GraphicsContext.ActiveViewport.ScenePos.X,
                        GraphicsContext.ActiveViewport.SceneSize.Y + GraphicsContext.ActiveViewport.ScenePos.Y,
                        GraphicsContext.ActiveViewport.ScenePos.Y, 0, 1);
                    GraphicsContext.ActivateBlendMode(spr.BlendName);
                    temp.Shader.SetTexture(spr.Texture);
                }

                vertices[v++] = new VertexPositionTexColor(spr.XYUV1, spr.Color);
                vertices[v++] = new VertexPositionTexColor(spr.XYUV2, spr.Color);
                vertices[v++] = new VertexPositionTexColor(spr.XYUV3, spr.Color);
                vertices[v++] = new VertexPositionTexColor(spr.XYUV4, spr.Color);

                if (count == maxBatchCount - 1)
                {
                    geometry.UpdateVB(vertices);
                    geometry.Render();
                    DrawCalls++;
                    count = 0;
                    v = 0;
                    Array.Clear(vertices, 0, vertices.Length);
                }
                count++;
            }

            if (count > 0)
            {
                geometry.UpdateVB(vertices);
                geometry.Render();
                DrawCalls++;
                Array.Clear(vertices, 0, vertices.Length);
            }

            quadList.Clear();
            GraphicsContext.ActivateBlendMode("Default");
        }

        // ── Quad Building ───────────────────────────────────────────

        void BuildQuad(ref Particle particle)
        {
            int idx = particle.EmitterIndex;
            if (idx < 0 || idx >= emitters.Count)
                return;

            Sprite sprite = emitters[idx].Sprite;
            if (sprite == null)
                return;

            Vector2 origin = new Vector2(0f, 0f);
            Vector2 scale = new Vector2(particle.Scale, particle.Scale);
            float angle = MathHelper.ToDegrees(particle.Rotation);

            float x = (float)System.Math.Round(particle.Position.X);
            float y = (float)System.Math.Round(particle.Position.Y);
            float w = sprite.Size.X * scale.X;
            float h = sprite.Size.Y * scale.Y;

            x -= origin.X * scale.X;
            y -= origin.Y * scale.Y;

            Vector2 pos1 = new Vector2(x, y);
            Vector2 pos2 = new Vector2(x + w, y);
            Vector2 pos3 = new Vector2(x + w, y + h);
            Vector2 pos4 = new Vector2(x, y + h);

            // Rotation
            if (angle != 0f)
            {
                double ang = MathHelper.DegToRad(angle);
                float origX = origin.X * scale.X;
                float origY = origin.Y * scale.Y;
                float sin = -(float)System.Math.Sin(ang);
                float cos = (float)System.Math.Cos(ang);

                float x1 = -origX * cos - (-origY * sin);
                float y1 = -origX * sin + (-origY * cos);
                float x2 = (w - origX) * cos - (-origY * sin);
                float y2 = (w - origX) * sin + (-origY * cos);
                float x3 = (w - origX) * cos - (h - origY) * sin;
                float y3 = (w - origX) * sin + (h - origY) * cos;
                float x4 = (-origX * cos) - (h - origY) * sin;
                float y4 = (-origX * sin) + (h - origY) * cos;

                x += (float)System.Math.Round(origin.X * scale.X);
                y += (float)System.Math.Round(origin.Y * scale.Y);

                pos1.X = (float)System.Math.Round(x + x1);
                pos1.Y = (float)System.Math.Round(y + y1);
                pos2.X = (float)System.Math.Round(x + x2);
                pos2.Y = (float)System.Math.Round(y + y2);
                pos3.X = (float)System.Math.Round(x + x3);
                pos3.Y = (float)System.Math.Round(y + y3);
                pos4.X = (float)System.Math.Round(x + x4);
                pos4.Y = (float)System.Math.Round(y + y4);
            }

            // UV coordinates
            float u = 0;
            float v = 0;
            if (sprite.FrameMap == null)
            {
                u = (sprite.TopLeft.X + ((sprite.TexSize.X + sprite.Skip.X) * particle.ImageIndex)) / sprite.Texture.Width;
                v = sprite.TopLeft.Y / sprite.Texture.Height;
            }
            else
            {
                u = (sprite.FrameMap[particle.ImageIndex % sprite.ImageCount].X) / sprite.Texture.Width;
                v = (sprite.FrameMap[particle.ImageIndex % sprite.ImageCount].Y) / sprite.Texture.Height;
            }

            Vector2 uv1 = new Vector2(u, v);
            Vector2 uv2 = new Vector2(sprite.TexSize.X / sprite.Texture.Width + uv1.X, uv1.Y);
            Vector2 uv3 = new Vector2(uv2.X, sprite.TexSize.Y / sprite.Texture.Height + uv1.Y);
            Vector2 uv4 = new Vector2(uv1.X, uv3.Y);

            SpriteQuad spr = new SpriteQuad(sprite.Texture,
                new Vector4(pos1, uv1), new Vector4(pos2, uv2),
                new Vector4(pos3, uv3), new Vector4(pos4, uv4),
                particle.Color, shader, blendName);

            quadList.Add(spr);
        }
    }
}
