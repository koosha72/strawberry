/*
 * Strawberry Game Engine
 * File: SpriteLayer.cs
 * Author: Koosha Aabedini Nassab
 *
 * Layer implementation that batches and renders sprites efficiently.
 */

using Strawberry.Common;
using Strawberry.Core;
using Strawberry.Graphics.Text;
using Strawberry.Math;

namespace Strawberry.Graphics.Layers
{
    /// <summary>
    /// A layer responsible for batching and rendering sprites within a scene.
    /// </summary>
    public class SpriteLayer : Layer
    {
        const int DefaultMaxBatchCount = 2048;

        readonly int maxBatchCount;
        readonly List<SpriteQuad> quadList = new List<SpriteQuad>();

        Geometry<VertexPositionTexColor> geometry;
        VertexPositionTexColor[] vertices;
        uint[] indices;

        BasicShader defaultShader;
        TextShader textShader;
        Sprite pixelSprite;
        string activeBlendName = "Default";

        /// <summary>
        /// Gets the currently active shader used for sprite rendering.
        /// </summary>
        public BasicShader ActiveShader { get; private set; }

        /// <summary>
        /// Gets the graphics context for the current scene.
        /// </summary>
        public IGraphicsContext GraphicsContext => Scene.GameContext.GraphicsContext;

        /// <summary>
        /// Gets or sets the number of draw calls performed during the most recent render pass.
        /// </summary>
        public int DrawCalls { get; set; }

        public SpriteLayer() : this(DefaultMaxBatchCount) { }

        public SpriteLayer(int maxBatchCount)
        {
            this.maxBatchCount = maxBatchCount;
        }

        /// <summary>
        /// Initializes the sprite layer and allocates rendering resources.
        /// </summary>
        public override void Initialize(Scene scene)
        {
            base.Initialize(scene);

            vertices = new VertexPositionTexColor[maxBatchCount * 4];
            indices = BuildQuadIndices(maxBatchCount);

            geometry = GraphicsContext.CreateGeometry<VertexPositionTexColor>(
                vertices, indices, GeometryType.Dynamic, GeometryType.Static);

            defaultShader = new BasicShader(GraphicsContext, VertexElementContainer.VertexPositionTexColor);
            textShader = new TextShader(GraphicsContext, VertexElementContainer.VertexPositionTexColor);
            ActiveShader = defaultShader;

            pixelSprite = new Sprite(
                GraphicsContext.PixelTexture, 1,
                new Vector2(1f, 1f), Vector2.Zero,
                new Vector2(1f, 1f), Vector2.Zero);
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

        #region State Management

        public void SetShader(BasicShader shader)
        {
            ActiveShader = shader;
        }

        public void ResetShader()
        {
            ActiveShader = defaultShader;
        }

        public void SetBlendMode(string name)
        {
            activeBlendName = name;
        }

        #endregion

        #region Quad Construction

        public SpriteQuad GetQuad(Sprite sprite, Vector2 position, Vector2 origin, Vector2 scale, Color color, int imageIndex, float angle)
        {
            Vector2[] corners = CalculateCorners(sprite.Size, position, origin, scale, angle);
            Vector2[] uvs = CalculateUVs(sprite, imageIndex);

            return new SpriteQuad(
                sprite.Texture,
                new Vector4(corners[0], uvs[0]),
                new Vector4(corners[1], uvs[1]),
                new Vector4(corners[2], uvs[2]),
                new Vector4(corners[3], uvs[3]),
                color, ActiveShader, activeBlendName);
        }

        private Vector2[] CalculateCorners(Vector2 spriteSize, Vector2 position, Vector2 origin, Vector2 scale, float angle)
        {
            float w = spriteSize.X * scale.X;
            float h = spriteSize.Y * scale.Y;
            float pivotX = origin.X * scale.X;
            float pivotY = origin.Y * scale.Y;

            Vector2 center = new Vector2(Round(position.X), Round(position.Y));

            var corners = new Vector2[4];

            if (angle != 0f)
            {
                float rad = MathHelper.DegToRad(angle);
                float sin = -(float)System.Math.Sin(rad);
                float cos = (float)System.Math.Cos(rad);

                corners[0] = RotateOffset(-pivotX, -pivotY, sin, cos, center);
                corners[1] = RotateOffset(w - pivotX, -pivotY, sin, cos, center);
                corners[2] = RotateOffset(w - pivotX, h - pivotY, sin, cos, center);
                corners[3] = RotateOffset(-pivotX, h - pivotY, sin, cos, center);
            }
            else
            {
                corners[0] = new Vector2(center.X - pivotX, center.Y - pivotY);
                corners[1] = new Vector2(center.X - pivotX + w, center.Y - pivotY);
                corners[2] = new Vector2(center.X - pivotX + w, center.Y - pivotY + h);
                corners[3] = new Vector2(center.X - pivotX, center.Y - pivotY + h);
            }

            return corners;
        }

        private static Vector2 RotateOffset(float localX, float localY, float sin, float cos, Vector2 center)
        {
            float rx = localX * cos - localY * sin;
            float ry = localX * sin + localY * cos;

            return new Vector2(Round(center.X + rx), Round(center.Y + ry));
        }

        private static Vector2[] CalculateUVs(Sprite sprite, int imageIndex)
        {
            float u, v;

            if (sprite.FrameMap != null)
            {
                int frameIdx = ((imageIndex % sprite.ImageCount) + sprite.ImageCount) % sprite.ImageCount;
                u = sprite.FrameMap[frameIdx].X / sprite.Texture.Width;
                v = sprite.FrameMap[frameIdx].Y / sprite.Texture.Height;
            }
            else
            {
                u = (sprite.TopLeft.X + (sprite.TexSize.X + sprite.Skip.X) * imageIndex) / sprite.Texture.Width;
                v = sprite.TopLeft.Y / sprite.Texture.Height;
            }

            float uvW = sprite.TexSize.X / sprite.Texture.Width;
            float uvH = sprite.TexSize.Y / sprite.Texture.Height;

            return new[]
            {
                new Vector2(u, v),
                new Vector2(u + uvW, v),
                new Vector2(u + uvW, v + uvH),
                new Vector2(u, v + uvH)
            };
        }

        private static float Round(float value) => (float)System.Math.Round(value);

        #endregion

        #region Batching

        public void Push(Sprite sprite, Vector2 position, Vector2 origin, Vector2 scale, Color color, int imageIndex, float angle)
        {
            quadList.Add(GetQuad(sprite, position, origin, scale, color, imageIndex, angle));
        }

        public void Push(Texture texture, Vector2 position, Vector2 texSize, Vector2 size, Vector2 topLeft, Color color)
        {
            float x = Round(position.X);
            float y = Round(position.Y);
            float w = Round(size.X);
            float h = Round(size.Y);

            var corners = new[]
            {
                new Vector2(x, y),
                new Vector2(x + w, y),
                new Vector2(x + w, y + h),
                new Vector2(x, y + h)
            };

            float u = topLeft.X / texture.Width;
            float v = topLeft.Y / texture.Height;
            float uvW = texSize.X / texture.Width;
            float uvH = texSize.Y / texture.Height;

            var uvs = new[]
            {
                new Vector2(u, v),
                new Vector2(u + uvW, v),
                new Vector2(u + uvW, v + uvH),
                new Vector2(u, v + uvH)
            };

            quadList.Add(new SpriteQuad(
                texture,
                new Vector4(corners[0], uvs[0]),
                new Vector4(corners[1], uvs[1]),
                new Vector4(corners[2], uvs[2]),
                new Vector4(corners[3], uvs[3]),
                color, ActiveShader, activeBlendName));
        }

        #endregion

        #region Rendering

        public override void Render()
        {
            if (!Enabled || !Viewports.Contains(GraphicsContext.ActiveViewport.Name))
            {
                quadList.Clear();
                return;
            }

            if (quadList.Count == 0)
                return;

            if (Sorter != null)
                Sorter.Sort(quadList);

            DrawCalls = 0;

            SpriteQuad temp = null;
            int quadCount = 0;
            int vertexIndex = 0;

            foreach (SpriteQuad spr in quadList)
            {
                if (temp == null)
                {
                    temp = spr;
                    ActivateBatchState(spr);
                }
                else if (temp != spr) // Reverted: Relies on SpriteQuad's overloaded != operator
                {
                    if (quadCount > 0)
                        FlushBatch(ref quadCount, ref vertexIndex);

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

        private void FlushBatch(ref int quadCount, ref int vertexIndex)
        {
            geometry.UpdateVB(vertices);
            geometry.Render();
            DrawCalls++;

            Array.Clear(vertices, 0, vertices.Length);
            quadCount = 0;
            vertexIndex = 0;
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
            return Matrix4.CreateOrthographic(
                vp.ScenePos.X,
                vp.SceneSize.X + vp.ScenePos.X,
                vp.SceneSize.Y + vp.ScenePos.Y,
                vp.ScenePos.Y,
                0, 1);
        }

        private void AppendQuadVertices(SpriteQuad spr, ref int vertexIndex)
        {
            vertices[vertexIndex++] = new VertexPositionTexColor(spr.XYUV1, spr.Color);
            vertices[vertexIndex++] = new VertexPositionTexColor(spr.XYUV2, spr.Color);
            vertices[vertexIndex++] = new VertexPositionTexColor(spr.XYUV3, spr.Color);
            vertices[vertexIndex++] = new VertexPositionTexColor(spr.XYUV4, spr.Color);
        }

        #endregion

        #region Primitive Drawing

        public void DrawOutlineRectangle(RotatedRectangle rect, Color color)
        {
            Push(pixelSprite, rect.Vertex1, Vector2.Zero, new Vector2((int)rect.Width, 1f), color, 0, rect.Angle);
            Push(pixelSprite, rect.Vertex2, Vector2.Zero, new Vector2(1f, (int)rect.Height + 1), color, 0, rect.Angle);
            Push(pixelSprite, rect.Vertex3, Vector2.Zero, new Vector2(-(int)rect.Width, 1f), color, 0, rect.Angle);
            Push(pixelSprite, rect.Vertex4, Vector2.Zero, new Vector2(1f, -(int)rect.Height), color, 0, rect.Angle);
        }

        public void DrawRectangle(RotatedRectangle rect, Color color)
        {
            Vector2 origin = new Vector2(rect.Origin.X / rect.Width, rect.Origin.Y / rect.Height);
            Push(pixelSprite, new Vector2(rect.X, rect.Y), origin,
                new Vector2(rect.Width, rect.Height), color, 0, rect.Angle);
        }

        public void DrawHorizontalLine(Vector2 start, float length, Color color)
        {
            Push(pixelSprite, start, Vector2.Zero, new Vector2(length, 1), color, 0, 0);
        }

        public void DrawVerticalLine(Vector2 start, float length, Color color)
        {
            Push(pixelSprite, start, Vector2.Zero, new Vector2(1, length), color, 0, 0);
        }

        #endregion

        #region Text Rendering

        public Vector2 PushString(string text, Font font, Vector2 position, Color color, TextDirection direction)
        {
            if (font.UseSDF)
                SetShader(textShader);

            double x = position.X;
            double y = position.Y;
            Texture tex = font.Texture;

            foreach (char c in text)
            {
                if (c == ' ')
                {
                    Character chr = font.GetCharacterInfo((ushort)c);
                    x += direction == TextDirection.LeftToRight ? chr.Adwidth : -chr.Adwidth;
                    continue;
                }

                if (c == '\n')
                {
                    y += font.Size;
                    x = (int)position.X;
                    continue;
                }

                Character info = font.GetCharacterInfo((ushort)c);
                float hsize = (float)System.Math.Ceiling(info.Bottom);
                float wsize = (float)System.Math.Ceiling(info.Right);
                float left = (float)System.Math.Floor(info.Left);
                float top = (float)System.Math.Ceiling(info.Top);

                var pos = new Vector2((float)x, (float)y);
                if (direction == TextDirection.RightToLeft)
                    pos.X -= (float)info.Adwidth;

                float charW = wsize - left;
                float charH = hsize - top;
                Push(tex, pos, new Vector2(charW, charH), new Vector2(charW, charH),
                    new Vector2(left, top), color);

                x += direction == TextDirection.LeftToRight ? info.Adwidth : -info.Adwidth;
            }

            if (font.UseSDF)
                ResetShader();

            return new Vector2((float)x, (float)y);
        }

        public Vector2 PushString(string text, Font font, Vector2 position, Color color, TextDirection direction, float size)
        {
            if (font.UseSDF)
                SetShader(textShader);

            double x = position.X;
            double y = position.Y;
            Texture tex = font.Texture;
            float scale = size / font.Size;

            foreach (char c in text)
            {
                if (c == ' ')
                {
                    Character chr = font.GetCharacterInfo((ushort)c);
                    x += direction == TextDirection.LeftToRight ? chr.Adwidth * scale : -chr.Adwidth * scale;
                    continue;
                }

                if (c == '\n')
                {
                    y += font.Size * scale;
                    x = (int)position.X;
                    continue;
                }

                Character info = font.GetCharacterInfo((ushort)c);
                float hsize = (float)info.Bottom + 1.0f;
                float wsize = (float)info.Right + 1.0f;
                float left = (float)info.Left;
                float top = (float)info.Top;

                var pos = new Vector2((float)x, (float)y);
                if (direction == TextDirection.RightToLeft)
                    pos.X -= (float)info.Adwidth * scale;

                float charW = wsize - left;
                float charH = hsize - top;
                Push(tex, pos, new Vector2(charW, charH), new Vector2(charW * scale, charH * scale),
                    new Vector2(left, top), color);

                x += direction == TextDirection.LeftToRight ? info.Adwidth * scale : -info.Adwidth * scale;
            }

            if (font.UseSDF)
                ResetShader();

            return new Vector2((float)x, (float)y);
        }

        #endregion
    }
}