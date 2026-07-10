/*
 * Strawberry Game Engine
 * File: BackgroundLayer.cs
 * Author: Koosha Aabedini Nassab
 *
 * Background layer rendering for tiled and scrolling scene backdrops.
 */

using Strawberry.Common;
using Strawberry.Core;
using Strawberry.Math;

namespace Strawberry.Graphics.Layers
{
    /// <summary>
    /// Background layer. Renders a single sprite as a tiled or scrolling background.
    /// </summary>
    public class BackgroundLayer : Layer
    {
        const int DefaultMaxBatchCount = 128;

        readonly int maxBatchCount;
        readonly List<SpriteQuad> quadList = new List<SpriteQuad>();

        Geometry<VertexPositionTexColor> geometry;
        VertexPositionTexColor[] vertices;
        uint[] indices;

        BasicShader defaultShader;
        string activeBlendName = "Default";

        float imageIndex = 0f;
        int realImageIndex = 0;

        /// <summary>
        /// Gets the currently active shader used for rendering.
        /// </summary>
        public BasicShader ActiveShader { get; private set; }

        public IGraphicsContext GraphicsContext => Scene.GameContext.GraphicsContext;

        /// <summary>
        /// The sprite to render as background.
        /// </summary>
        public Sprite Sprite { get; set; }

        /// <summary>
        /// Current frame of the sprite.
        /// </summary>
        public int ImageIndex
        {
            get { return realImageIndex; }
            set { realImageIndex = value; imageIndex = value; }
        }

        /// <summary>
        /// The speed of the animation in frames (1 = a cycle of animation per second).
        /// </summary>
        public int ImageSpeed { get; set; }

        /// <summary>
        /// Gets or sets the number of draw calls performed during the most recent render pass.
        /// </summary>
        public int DrawCalls { get; set; }

        /// <summary>
        /// The top left position of the background layer.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// The scale of the sprite rendered by background layer.
        /// </summary>
        public Vector2 Scale { get; set; } = Vector2.One;

        /// <summary>
        /// The number of repeats in the x direction. Set to -1 for infinite scrolling based on viewport.
        /// </summary>
        public int TileH { get; set; } = 1;

        /// <summary>
        /// The number of repeats in the y direction. Set to -1 for infinite scrolling based on viewport.
        /// </summary>
        public int TileV { get; set; } = 1;

        /// <summary>
        /// The size of the background layer.
        /// </summary>
        public Vector2 Size { get; set; }

        /// <summary>
        /// The color used to render the background layer.
        /// </summary>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// The parallax scrolling factor. Vector2.One scrolls normally with the camera, 
        /// Vector2.Zero stays fixed relative to the camera.
        /// </summary>
        public Vector2 ParallaxFactor { get; set; } = Vector2.One;

        /// <summary>
        /// Gets or sets the speed at which the background layer scrolls automatically.
        /// </summary>
        public Vector2 AutoScrollSpeed { get; set; }

        public BackgroundLayer() : this(DefaultMaxBatchCount) { }

        public BackgroundLayer(int maxBatchCount)
        {
            this.maxBatchCount = maxBatchCount;
        }

        public override void Initialize(Scene scene)
        {
            base.Initialize(scene);

            vertices = new VertexPositionTexColor[maxBatchCount * 4];
            indices = BuildQuadIndices(maxBatchCount);

            geometry = GraphicsContext.CreateGeometry<VertexPositionTexColor>(
                vertices, indices, GeometryType.Dynamic, GeometryType.Static);

            defaultShader = new BasicShader(GraphicsContext, VertexElementContainer.VertexPositionTexColor);
            ActiveShader = defaultShader;
        }

        private static uint[] BuildQuadIndices(int quadCount)
        {
            var result = new uint[quadCount * 6];
            int writeIdx = 0;

            for (uint vertIdx = 0; vertIdx < quadCount * 4; vertIdx += 4)
            {
                result[writeIdx++] = vertIdx;
                result[writeIdx++] = vertIdx + 1;
                result[writeIdx++] = vertIdx + 2;
                result[writeIdx++] = vertIdx + 2;
                result[writeIdx++] = vertIdx + 3;
                result[writeIdx++] = vertIdx;
            }

            return result;
        }

        public override void Update()
        {
            if (Sprite == null || Sprite.ImageCount == 0)
                return;

            imageIndex += (ImageSpeed * Sprite.ImageCount) * FrameInfo.Information.DeltaTime;

            if (Sprite.ImageCount > 0)
            {
                imageIndex %= Sprite.ImageCount;
                if (imageIndex < 0)
                    imageIndex += Sprite.ImageCount;
            }

            realImageIndex = (int)imageIndex;

            if (AutoScrollSpeed != Vector2.Zero)
            {
                Position += AutoScrollSpeed * FrameInfo.Information.DeltaTime;
            }
        }

        #region Rendering

        public override void Render()
        {
            if (!Enabled || !Viewports.Contains(GraphicsContext.ActiveViewport.Name))
            {
                quadList.Clear();
                return;
            }

            PushBackground();

            if (quadList.Count == 0)
                return;

            int quadCount = 0;
            int vertexIndex = 0;
            SpriteQuad temp = null;

            foreach (SpriteQuad spr in quadList)
            {
                if (temp == null)
                {
                    temp = spr;
                    ActivateBatchState(spr);
                }

                AppendQuadVertices(spr, ref vertexIndex);
                quadCount++;

                if (quadCount >= maxBatchCount)
                    FlushBatch(ref quadCount, ref vertexIndex);
            }

            if (quadCount > 0)
                FlushBatch(ref quadCount, ref vertexIndex);

            quadList.Clear();
            GraphicsContext.ActivateBlendMode("Default");
        }

        private void ActivateBatchState(SpriteQuad spr)
        {
            spr.Shader.Activate();
            spr.Shader.Projection = CreateProjectionMatrix();
            spr.Shader.SetTexture(spr.Texture);
            GraphicsContext.ActivateBlendMode(spr.BlendName);
        }

        private Matrix4 CreateProjectionMatrix()
        {
            var vp = GraphicsContext.ActiveViewport;
            // Note: Casting to int as in original code
            return Matrix4.CreateOrthographic(
                vp.ScenePos.X,
                vp.SceneSize.X + vp.ScenePos.X,
                vp.SceneSize.Y + vp.ScenePos.Y,
                vp.ScenePos.Y, 0, 1);
        }

        private void FlushBatch(ref int quadCount, ref int vertexIndex)
        {
            geometry.UpdateVB(vertices);
            geometry.Render();
            DrawCalls++;

            Array.Clear(vertices, 0, vertices.Length);
            quadCount = 0;
            vertexIndex = 0;
        }

        private void AppendQuadVertices(SpriteQuad spr, ref int vertexIndex)
        {
            vertices[vertexIndex++] = new VertexPositionTexColor(spr.XYUV1, spr.Color);
            vertices[vertexIndex++] = new VertexPositionTexColor(spr.XYUV2, spr.Color);
            vertices[vertexIndex++] = new VertexPositionTexColor(spr.XYUV3, spr.Color);
            vertices[vertexIndex++] = new VertexPositionTexColor(spr.XYUV4, spr.Color);
        }

        #endregion

        #region Background Generation

        private void PushBackground()
        {
            if (Sprite == null)
                return;

            Size = Sprite.Size;

            if (Scale.X == 0 || Scale.Y == 0)
                return;

            Vector2[] uvs = CalculateUVs();

            int tileH = TileH;
            int tileV = TileV;

            Vector2 pos = new Vector2(Round(Position.X), Round(Position.Y));

            if (tileH == -1)
                tileH = CalculateHorizontalTiling(ref pos);

            if (tileV == -1)
                tileV = CalculateVerticalTiling(ref pos);

            GenerateTiles(tileH, tileV, pos, uvs);
        }

        private Vector2[] CalculateUVs()
        {
            float u = 0;
            float v = 0;

            if (Sprite.FrameMap == null)
            {
                u = (Sprite.TopLeft.X + ((Sprite.TexSize.X + Sprite.Skip.X) * ImageIndex)) / Sprite.Texture.Width;
                v = Sprite.TopLeft.Y / Sprite.Texture.Height;
            }
            else
            {
                int frameIdx = ((ImageIndex % Sprite.ImageCount) + Sprite.ImageCount) % Sprite.ImageCount;
                u = (Sprite.FrameMap[frameIdx].X) / Sprite.Texture.Width;
                v = (Sprite.FrameMap[frameIdx].Y) / Sprite.Texture.Height;
            }

            Vector2 uv1 = new Vector2(u, v);
            Vector2 uv2 = new Vector2((Sprite.TexSize.X / Sprite.Texture.Width + uv1.X), uv1.Y);
            Vector2 uv3 = new Vector2(uv2.X, Sprite.TexSize.Y / Sprite.Texture.Height + uv1.Y);
            Vector2 uv4 = new Vector2(uv1.X, uv3.Y);

            return new[] { uv1, uv2, uv3, uv4 };
        }

        private int CalculateHorizontalTiling(ref Vector2 pos)
        {
            float tileWidth = Size.X * Scale.X;
            float viewportX = GraphicsContext.ActiveViewport.ScenePos.X * ParallaxFactor.X;

            int tileH = (int)(GraphicsContext.ActiveViewport.SceneSize.X / tileWidth) + 2;
            pos.X = pos.X % tileWidth;

            float dx = viewportX % tileWidth;

            if (dx < 0)
            {
                dx += tileWidth;
            }

            pos.X += GraphicsContext.ActiveViewport.ScenePos.X - dx - tileWidth;

            return tileH;
        }

        private int CalculateVerticalTiling(ref Vector2 pos)
        {
            float tileHeight = Size.Y * Scale.Y;
            float viewportY = GraphicsContext.ActiveViewport.ScenePos.Y * ParallaxFactor.Y;

            int tileV = (int)(GraphicsContext.ActiveViewport.SceneSize.Y / tileHeight) + 3;
            pos.Y = pos.Y % tileHeight;

            float dy = viewportY % tileHeight;

            if (dy < 0)
            {
                dy += tileHeight;
            }

            pos.Y += GraphicsContext.ActiveViewport.ScenePos.Y - dy - tileHeight;

            return tileV;
        }

        private void GenerateTiles(int tileH, int tileV, Vector2 pos, Vector2[] uvs)
        {
            Rectangle vr = new Rectangle(GraphicsContext.ActiveViewport.ScenePos, GraphicsContext.ActiveViewport.SceneSize);

            for (int i = 0; i < tileV; i++)
            {
                for (int j = 0; j < tileH; j++)
                {
                    float x = Round(Round(pos.X) + j * (Size.X * Scale.X));
                    float y = Round(Round(pos.Y) + i * (Size.Y * Scale.Y));
                    float w = Size.X * Scale.X;
                    float h = Size.Y * Scale.Y;

                    Rectangle r = new Rectangle(x, y, w, h);
                    if (!vr.Overlap(r))
                        continue;

                    Vector2 pos1 = new Vector2(x, y);
                    Vector2 pos2 = new Vector2(x + w, y);
                    Vector2 pos3 = new Vector2(x + w, y + h);
                    Vector2 pos4 = new Vector2(x, y + h);

                    quadList.Add(new SpriteQuad(
                        Sprite.Texture,
                        new Vector4(pos1, uvs[0]),
                        new Vector4(pos2, uvs[1]),
                        new Vector4(pos3, uvs[2]),
                        new Vector4(pos4, uvs[3]),
                        Color, ActiveShader, activeBlendName));
                }
            }
        }

        private static float Round(float value) => (float)System.Math.Round(value);

        #endregion
    }
}