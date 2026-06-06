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
    /// Background layer. Renders a single sprite as background.
    /// </summary>
    public class BackgroundLayer : Layer
    {
        int maxBatchCount = 2048;

        List<SpriteQuad> quadList;

        Geometry<VertexPositionTexColor> geometry;

        VertexPositionTexColor[] vertices;

        uint[] indices;

        public BasicShader ActiveShader { get; private set; }

        //TextShader TextShader { get; set; }

        BasicShader shader;

        string blendName = "Default";


        public IGraphicsContext GraphicsContext { get { return Scene.GameContext.GraphicsContext; } }

        public float HalfTexel = 0.5f;

        /// <summary>
        /// The sprite to render as background.
        /// </summary>
        public Sprite Sprite { get; set; }

        /// <summary>
        /// Current frame of the sprite.
        /// </summary>
        public int ImageIndex
        {
            get { return this.realImageIndex; }
            set { realImageIndex = value; imageIndex = value; }
        }

        /// <summary>
        /// The speed of the animation in frames (1 = a cycle of animation per second).
        /// </summary>
        public int ImageSpeed { get; set; }

        public int DrawCalls { get; set; }

        float imageIndex = 0f;

        int realImageIndex = 0;
        /// <summary>
        /// The top left position of the background layer.
        /// </summary>
        public Vector2 Position { get; set; }
        /// <summary>
        /// The scale of the sprite rendered by background layer.
        /// </summary>
        public Vector2 Scale { get; set; } = Vector2.One;
        /// <summary>
        /// The number of repeats in the x direction.
        /// </summary>
        public int TileH { get; set; } = 1;
        /// <summary>
        /// The number of repeats in the y direction.
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

        public BackgroundLayer()
        {
        }

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
            this.ActiveShader = shader;
            //TextShader = new TextShader(graphicsContext, VertexElementContainer.VertexPositionTexColor);
        }

        void PushBackground()
        {
            if (Sprite == null)
                return;
            Size = Sprite.Size;
            if (Sprite == null)
                return;
            Vector2 half = new Vector2();
            if (Scale.X == 0 || Scale.Y == 0)
                return;
            float u = 0;
            float v = 0;
            if (Sprite.FrameMap == null)
            {
                u = (Sprite.TopLeft.X + ((Sprite.TexSize.X + Sprite.Skip.X) * ImageIndex)) / Sprite.Texture.Width;
                v = Sprite.TopLeft.Y / Sprite.Texture.Height;
            }
            else
            {
                u = (Sprite.FrameMap[ImageIndex % Sprite.ImageCount].X) / Sprite.Texture.Width;
                v = (Sprite.FrameMap[ImageIndex % Sprite.ImageCount].Y) / Sprite.Texture.Height;
            }
            Vector2 uv1 = new Vector2(u + half.X, v + half.Y);
            Vector2 uv2 = new Vector2((Sprite.TexSize.X / Sprite.Texture.Width + uv1.X) - (half.X * 2), uv1.Y);
            Vector2 uv3 = new Vector2(uv2.X, Sprite.TexSize.Y / Sprite.Texture.Height + uv1.Y - (half.Y * 2));
            Vector2 uv4 = new Vector2(uv1.X, uv3.Y);

            Vector2 halfPixel = new Vector2(0f, 0f);

            if (Scale.X < 0)
                halfPixel.X *= -1;
            if (Scale.Y < 0)
                halfPixel.Y *= -1;

            int tileH = TileH;
            int tileV = TileV;

            Vector2 pos = new Vector2((float)System.Math.Round(Position.X), (float)System.Math.Round(Position.Y));

            if (tileH == -1)
            {
                tileH = ((int)GraphicsContext.ActiveViewport.SceneSize.X / (int)(Size.X * Scale.X)) + 1;
                int dx = (int)pos.X % (int)System.Math.Round(Size.X * Scale.X);
                if (dx > 0)
                {
                    pos.X = dx - (Size.X * Scale.X);
                    tileH++;
                }
                else if (dx < 0)
                {
                    pos.X = dx;
                    tileH++;
                }
                else
                    pos.X = 0;
                dx = (int)System.Math.Round(GraphicsContext.ActiveViewport.ScenePos.X) % (int)System.Math.Round(Size.X * Scale.X);
                pos.X -= dx;
                if (dx != 0)
                    tileH++;
                if (dx < 0)
                    pos.X -= (float)System.Math.Round(Size.X * Scale.X);
                pos.X += (float)System.Math.Round(GraphicsContext.ActiveViewport.ScenePos.X);
            }

            if (tileV == -1)
            {
                tileV = ((int)GraphicsContext.ActiveViewport.SceneSize.Y / (int)(Size.Y * Scale.Y)) + 1;
                int dy = (int)pos.Y % (int)System.Math.Round(Size.Y * Scale.Y);
                if (dy > 0)
                {
                    pos.Y = dy - (float)System.Math.Round(Size.Y * Scale.Y);
                    tileV++;
                }
                else if (dy < 0)
                {
                    pos.Y = dy;
                    tileV++;
                }
                else
                    pos.Y = 0;
                dy = (int)System.Math.Round(GraphicsContext.ActiveViewport.ScenePos.Y) % (int)System.Math.Round(Size.Y * Scale.Y);
                pos.Y -= dy;
                if (dy != 0)
                    tileV++;
                if (dy < 0)
                    pos.Y -= (Size.Y * Scale.Y);
                pos.Y += GraphicsContext.ActiveViewport.ScenePos.Y;
            }


            for (int i = 0; i < tileV; i++)
            {
                for (int j = 0; j < tileH; j++)
                {
                    float x = (float)System.Math.Round((float)System.Math.Round(pos.X) + j * (Size.X * Scale.X));
                    float y = (float)System.Math.Round((float)System.Math.Round(pos.Y) + i * (Size.Y * Scale.Y));
                    float w = Size.X * Scale.X;
                    float h = Size.Y * Scale.Y;

                    x += halfPixel.X;
                    y += halfPixel.Y;
                    w -= halfPixel.X * 2;
                    h -= halfPixel.Y * 2;

                    Rectangle r = new Rectangle(x, y, w, h);
                    Rectangle vr = new Rectangle(GraphicsContext.ActiveViewport.ScenePos, GraphicsContext.ActiveViewport.SceneSize);
                    if (!vr.Overlap(r))
                    {
                        continue;
                    }

                    Vector2 pos1 = new Vector2(x, y);
                    Vector2 pos2 = new Vector2(x + w, y);
                    Vector2 pos3 = new Vector2(x + w, y + h);
                    Vector2 pos4 = new Vector2(x, y + h);




                    SpriteQuad spr = new SpriteQuad(Sprite.Texture, new Vector4(pos1, uv1), new Vector4(pos2, uv2),
                        new Vector4(pos3, uv3), new Vector4(pos4, uv4), Color, ActiveShader, blendName);
                    quadList.Add(spr);
                }
            }
        }

        public override void Update()
        {
            if (Sprite != null)
            {
                if (realImageIndex <= Sprite.ImageCount - 1)
                {
                    imageIndex += (ImageSpeed * Sprite.ImageCount) * FrameInfo.Information.DeltaTime;
                    realImageIndex = (int)imageIndex;
                    if (realImageIndex >= Sprite.ImageCount)
                    {
                        realImageIndex -= Sprite.ImageCount;
                        imageIndex -= Sprite.ImageCount;
                    }
                }
            }
        }

        public override void Render()
        {
            if (!Viewports.Contains(GraphicsContext.ActiveViewport.Name))
                return;
            PushBackground();
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
    }
}
