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
        int maxBatchCount = 2048;

        List<SpriteQuad> quadList;

        Geometry<VertexPositionTexColor> geometry;

        VertexPositionTexColor[] vertices;

        uint[] indices;

        /// <summary>
        /// Gets the currently active shader used for sprite rendering.
        /// </summary>
        public BasicShader ActiveShader { get; private set; }

        //TextShader TextShader { get; set; }

        BasicShader shader;
        TextShader textShader;

        string blendName = "Default";

        /// <summary>
        /// Gets the graphics context for the current scene.
        /// </summary>
        public IGraphicsContext GraphicsContext { get { return Scene.GameContext.GraphicsContext; } }

        Sprite pixelSprite;

        /// <summary>
        /// Gets or sets the number of draw calls performed during the most recent render pass.
        /// </summary>
        public int DrawCalls { get; set; }

        public SpriteLayer()
        {
        }

        /// <summary>
        /// Initializes the sprite layer and allocates rendering resources.
        /// </summary>
        /// <param name="scene">The scene that owns the layer.</param>
        public override void Initialize(Scene scene)
        {
            base.Initialize(scene);

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

            shader = new BasicShader(Scene.GameContext.GraphicsContext, VertexElementContainer.VertexPositionTexColor);
            textShader = new TextShader(GraphicsContext, VertexElementContainer.VertexPositionTexColor);

            ActiveShader = shader;
            //TextShader = new TextShader(graphicsContext, VertexElementContainer.VertexPositionTexColor);

            pixelSprite = new Sprite(GraphicsContext.PixelTexture, 1, new Vector2(1f, 1f), new Vector2(0f, 0f),
                new Vector2(1f, 1f), new Vector2());
        }

        /// <summary>
        /// Sets the active shader used when rendering sprites.
        /// </summary>
        /// <param name="shader">The shader to activate.</param>
        public void SetShader(BasicShader shader)
        {
            this.ActiveShader = shader;
        }

        /// <summary>
        /// Sets the blend mode name used for subsequent sprite draws.
        /// </summary>
        /// <param name="name">The blend mode identifier.</param>
        public void SetBlendMode(string name)
        {
            blendName = name;
        }

        /// <summary>
        /// Resets the active shader to the default sprite shader.
        /// </summary>
        public void ResetShader()
        {
            this.ActiveShader = shader;
        }

        /// <summary>
        /// Creates a sprite quad for the given sprite and transform parameters.
        /// </summary>
        /// <param name="sprite">The sprite to render.</param>
        /// <param name="position">The screen position of the sprite.</param>
        /// <param name="origin">The origin used for rotation and scaling.</param>
        /// <param name="scale">The scaling factor for the sprite.</param>
        /// <param name="color">The color tint to apply.</param>
        /// <param name="imageIndex">The frame index within the sprite texture.</param>
        /// <param name="angle">The rotation angle in degrees.</param>
        /// <returns>A configured sprite quad ready for batching.</returns>
        public SpriteQuad GetQuad(Sprite sprite, Vector2 position, Vector2 origin, Vector2 scale, Color color, int imageIndex, float angle)
        {
            Vector2 halfPixel = new Vector2(0f, 0f);

            if (scale.X < 0)
                halfPixel.X *= -1;
            if (scale.Y < 0)
                halfPixel.Y *= -1;

            float x = (float)System.Math.Round(position.X);
            float y = (float)System.Math.Round(position.Y);
            float w = sprite.Size.X * scale.X;
            float h = sprite.Size.Y * scale.Y;

            x -= origin.X * scale.X;
            y -= origin.Y * scale.Y;

            x += halfPixel.X;
            y += halfPixel.Y;
            w -= halfPixel.X * 2;
            h -= halfPixel.Y * 2;

            Vector2 pos1 = new Vector2(x, y);
            Vector2 pos2 = new Vector2(x + w, y);
            Vector2 pos3 = new Vector2(x + w, y + h);
            Vector2 pos4 = new Vector2(x, y + h);

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

            Vector2 half = new Vector2();

            float u = 0;
            float v = 0;
            if (sprite.FrameMap == null)
            {
                u = (sprite.TopLeft.X + ((sprite.TexSize.X + sprite.Skip.X) * imageIndex)) / sprite.Texture.Width;
                v = sprite.TopLeft.Y / sprite.Texture.Height;
            }
            else
            {
                u = (sprite.FrameMap[imageIndex % sprite.ImageCount].X) / sprite.Texture.Width;
                v = (sprite.FrameMap[imageIndex % sprite.ImageCount].Y) / sprite.Texture.Height;
            }
            Vector2 uv1 = new Vector2(u + half.X, v + half.Y);
            Vector2 uv2 = new Vector2((sprite.TexSize.X / sprite.Texture.Width + uv1.X) - (half.X * 2), uv1.Y);
            Vector2 uv3 = new Vector2(uv2.X, sprite.TexSize.Y / sprite.Texture.Height + uv1.Y - (half.Y * 2));
            Vector2 uv4 = new Vector2(uv1.X, uv3.Y);

            SpriteQuad spr = new SpriteQuad(sprite.Texture, new Vector4(pos1, uv1), new Vector4(pos2, uv2),
                new Vector4(pos3, uv3), new Vector4(pos4, uv4), color, ActiveShader, blendName);
            return spr;
        }

        /// <summary>
        /// Adds a transformed sprite quad to the current batch.
        /// </summary>
        /// <param name="sprite">The sprite to draw.</param>
        /// <param name="position">The position in scene coordinates.</param>
        /// <param name="origin">The origin offset for rotation and scaling.</param>
        /// <param name="scale">The scale for the sprite.</param>
        /// <param name="color">The tint color.</param>
        /// <param name="imageIndex">The frame index in the sprite sheet.</param>
        /// <param name="angle">The rotation angle in degrees.</param>
        public void Push(Sprite sprite, Vector2 position, Vector2 origin, Vector2 scale, Color color, int imageIndex, float angle)
        {
            quadList.Add(GetQuad(sprite, position, origin, scale, color, imageIndex, angle));
        }


        /// <summary>
        /// Adds a textured quad to the current batch.
        /// </summary>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="position">The position in scene coordinates.</param>
        /// <param name="texSize">The size of the source region in texture space.</param>
        /// <param name="size">The destination size in scene coordinates.</param>
        /// <param name="topLeft">The texture coordinates of the top-left corner.</param>
        /// <param name="color">The tint color.</param>
        public void Push(Texture texture, Vector2 position, Vector2 texSize, Vector2 size, Vector2 topLeft, Color color)
        {
            float x = (float)System.Math.Round(position.X);
            float y = (float)System.Math.Round(position.Y);
            float w = (float)System.Math.Round(size.X);
            float h = (float)System.Math.Round(size.Y);

            Vector2 pos1 = new Vector2(x, y);
            Vector2 pos2 = new Vector2(x + w, y);
            Vector2 pos3 = new Vector2(pos2.X, y + h);
            Vector2 pos4 = new Vector2(x, pos3.Y);

            Vector2 half = new Vector2(0, 0);

            float u = topLeft.X / texture.Width;
            Vector2 uv1 = new Vector2(u + half.X, topLeft.Y / texture.Height + half.Y);
            Vector2 uv2 = new Vector2((texSize.X / texture.Width + uv1.X) - (half.X * 2), uv1.Y);
            Vector2 uv3 = new Vector2(uv2.X, texSize.Y / texture.Height + uv1.Y - (half.Y * 2));
            Vector2 uv4 = new Vector2(uv1.X, uv3.Y);

            SpriteQuad spr = new SpriteQuad(texture, new Vector4(pos1, uv1), new Vector4(pos2, uv2),
                new Vector4(pos3, uv3), new Vector4(pos4, uv4), color, ActiveShader, blendName);

            quadList.Add(spr);
        }

        /// <summary>
        /// Renders all batched sprite quads for the current frame.
        /// </summary>
        public override void Render()
        {
            if (!Enabled)
            {
                quadList.Clear();
                return;
            }
            if (!Viewports.Contains(GraphicsContext.ActiveViewport.Name))
            {
                quadList.Clear();
                return;
            }
            if (Sorter != null)
                Sorter.Sort(quadList);

            int count = 0;
            int v = 0;

            SpriteQuad temp = null;

            /*BasicShader shader = ActiveShader;
            shader.Activate();
            shader.Projection = Matrix4.CreateOrthoProjection(GraphicsContext.ActiveViewport.WorldPos.X,
                GraphicsContext.ActiveViewport.WorldSize.X + GraphicsContext.ActiveViewport.WorldPos.X, GraphicsContext.ActiveViewport.WorldSize.Y + GraphicsContext.ActiveViewport.WorldPos.Y,
                GraphicsContext.ActiveViewport.WorldPos.Y, 0, 1);*/
            if (quadList.Count == 0)
                return;

            foreach (SpriteQuad spr in quadList)
            {
                if (temp == null)
                {
                    temp = spr;
                    temp.Shader.Activate();
                    temp.Shader.Projection = Matrix4.CreateOrthographic((int)GraphicsContext.ActiveViewport.ScenePos.X,
                        (int)GraphicsContext.ActiveViewport.SceneSize.X + (int)GraphicsContext.ActiveViewport.ScenePos.X, (int)GraphicsContext.ActiveViewport.SceneSize.Y + (int)GraphicsContext.ActiveViewport.ScenePos.Y,
                        (int)GraphicsContext.ActiveViewport.ScenePos.Y, 0, 1);
                    temp.Shader.SetTexture(spr.Texture);
                    GraphicsContext.ActivateBlendMode(spr.BlendName);
                }
                else if (temp != spr)
                {
                    geometry.UpdateVB(vertices);
                    geometry.Render();
                    DrawCalls++;
                    count = 0;
                    v = 0;
                    Array.Clear(vertices, 0, vertices.Length);

                    if (temp.Texture != spr.Texture)
                        temp = spr;
                    /*if (temp.Shader != spr.Shader)
                    {
                        temp = spr;
                    }*/
                    if (temp.BlendName != spr.BlendName)
                        temp = spr;
                    spr.Shader.Activate();
                    spr.Shader.Projection = Matrix4.CreateOrthographic(GraphicsContext.ActiveViewport.ScenePos.X,
                        GraphicsContext.ActiveViewport.SceneSize.X + GraphicsContext.ActiveViewport.ScenePos.X, GraphicsContext.ActiveViewport.SceneSize.Y + GraphicsContext.ActiveViewport.ScenePos.Y,
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

        /// <summary>
        /// Draws the outline of a rotated rectangle using pixel sprites.
        /// </summary>
        /// <param name="rect">The rotated rectangle to outline.</param>
        /// <param name="color">The color of the outline.</param>
        public void DrawOutlineRectangle(RotatedRectangle rect, Color color)
        {
            Push(pixelSprite, rect.Vertex1, new Vector2(),
                    new Vector2((int)rect.Width, 1f), color, 0, rect.Angle);

            Push(pixelSprite, rect.Vertex2, new Vector2(),
                    new Vector2(1f, (int)rect.Height + 1), color, 0, rect.Angle);

            Push(pixelSprite, rect.Vertex3, new Vector2(),
                new Vector2(-(int)rect.Width, 1f), color, 0, rect.Angle);

            Push(pixelSprite, rect.Vertex4, new Vector2(),
                new Vector2(1f, -(int)rect.Height), color, 0, rect.Angle);
        }

        /// <summary>
        /// Draws a filled rotated rectangle using a pixel sprite.
        /// </summary>
        /// <param name="rect">The rectangle to draw.</param>
        /// <param name="color">The fill color.</param>
        public void DrawRectangle(RotatedRectangle rect, Color color)
        {
            Vector2 origin = new Vector2((float)(rect.Origin.X / rect.Width), (float)(rect.Origin.Y / rect.Height));
            Push(pixelSprite, new Vector2(rect.X, rect.Y), origin,
                    new Vector2(rect.Width, rect.Height), color, 0, rect.Angle);
        }

        /// <summary>
        /// Draws a horizontal line using a pixel sprite.
        /// </summary>
        /// <param name="start">The starting position.</param>
        /// <param name="length">The length of the line.</param>
        /// <param name="color">The line color.</param>
        public void DrawHorizontalLine(Vector2 start, float length, Color color)
        {
            Push(pixelSprite, new Vector2(start.X, start.Y), Vector2.Zero,
                    new Vector2(length, 1), color, 0, 0);
        }

        /// <summary>
        /// Draws a vertical line using a pixel sprite.
        /// </summary>
        /// <param name="start">The starting position.</param>
        /// <param name="length">The length of the line.</param>
        /// <param name="color">The line color.</param>
        public void DrawVerticalLine(Vector2 start, float length, Color color)
        {
            Push(pixelSprite, new Vector2(start.X, start.Y), Vector2.Zero,
                    new Vector2(1, length), color, 0, 0);
        }

        /// <summary>
        /// Pushes a line of text into the sprite batch using the font's native size.
        /// </summary>
        /// <param name="text">The text to render.</param>
        /// <param name="font">The font to use.</param>
        /// <param name="position">The starting position in scene coordinates.</param>
        /// <param name="color">The text color.</param>
        /// <param name="direction">The text direction.</param>
        /// <returns>The final draw position after rendering the text.</returns>
        public Vector2 PushString(string text, Font font, Vector2 position, Color color, TextDirection direction)
        {
            if (font.UseSDF)
                SetShader(textShader);
            double x = position.X;
            double y = position.Y;
            Texture tex = font.Texture;
            Character chr;
            float m = 1f;
            foreach (char c in text)
            {
                ushort code = c;
                if (c == ' ')
                {
                    chr = font.GetCharacterInfo(code);
                    if (direction == TextDirection.LeftToRight)
                        x += chr.Adwidth * m;
                    else
                        x -= chr.Adwidth * m;
                    continue;
                }
                if (c == '\n')
                {
                    y += font.Size;
                    x = (int)position.X;
                    continue;
                }
                chr = font.GetCharacterInfo(code);
                Vector2 origin = new Vector2(0.0f, 0.0f);
                Vector2 pos = new Vector2((float)x, (float)y);

                float hsize = (float)System.Math.Ceiling(chr.Bottom);
                float wsize = (float)System.Math.Ceiling(chr.Right);
                float l = (float)System.Math.Floor(chr.Left);
                float t = (float)System.Math.Ceiling(chr.Top);
                if (direction == TextDirection.RightToLeft)
                    pos.X -= (float)chr.Adwidth;

                Push(tex, pos, new Vector2(wsize - l, (float)hsize - t), new Vector2((wsize - l) * m, (float)(hsize - t) * m), new Vector2(l, (float)t), color);

                if (direction == TextDirection.LeftToRight)
                    x += chr.Adwidth * m;
                else
                    x -= chr.Adwidth * m;
            }
            if (font.UseSDF)
                ResetShader();
            return new Vector2((float)x, (float)y);
        }

        /// <summary>
        /// Pushes a line of text into the sprite batch at a custom size.
        /// </summary>
        /// <param name="text">The text to render.</param>
        /// <param name="font">The font to use.</param>
        /// <param name="position">The starting position in scene coordinates.</param>
        /// <param name="color">The text color.</param>
        /// <param name="direction">The text direction.</param>
        /// <param name="size">The target size of the text.</param>
        /// <returns>The final draw position after rendering the text.</returns>
        public Vector2 PushString(string text, Font font, Vector2 position, Color color, TextDirection direction, float size)
        {
            if (font.UseSDF)
                SetShader(textShader);
            double x = position.X;
            double y = position.Y;
            Texture tex = font.Texture;
            Character chr;
            float m = size / (float)font.Size;
            foreach (char c in text)
            {
                ushort code = (ushort)c;
                if (c == ' ')
                {
                    chr = font.GetCharacterInfo(code);
                    if (direction == TextDirection.LeftToRight)
                        x += chr.Adwidth * m;
                    else
                        x -= chr.Adwidth * m;
                    continue;
                }
                if (c == '\n')
                {
                    y += font.Size * m;
                    x = (int)position.X;
                    continue;
                }
                chr = font.GetCharacterInfo(code);
                Vector2 origin = new Vector2(0.0f, 0.0f);
                Vector2 pos = new Vector2((float)x, (float)y);

                float hsize = (float)chr.Bottom + 1.0f;
                float wsize = (float)chr.Right + 1.0f;
                float l = (float)chr.Left;
                float t = (float)chr.Top;
                if (direction == TextDirection.RightToLeft)
                    pos.X -= (float)chr.Adwidth * m;
                Push(tex, pos, new Vector2(wsize - l, (float)hsize - t), new Vector2((wsize - l) * m, (float)(hsize - t) * m), new Vector2(l, (float)t), color);

                if (direction == TextDirection.LeftToRight)
                    x += chr.Adwidth * m;
                else
                    x -= chr.Adwidth * m;
            }
            if (font.UseSDF)
                ResetShader();
            return new Vector2((float)x, (float)y);
        }
    }
}
