using Strawberry.Core;
using Strawberry.Math;
using Strawberry.Misc;

namespace Strawberry.Graphics.Layers
{
    public class SpriteLayer : Layer
    {
        int maxBatchCount = 2048;

        List<SpriteQuad> quadList;

        IGeometry<VertexPositionTexColor> geometry;

        VertexPositionTexColor[] vertices;

        uint[] indices;

        public BasicShader ActiveShader { get; private set; }

        //TextShader TextShader { get; set; }

        BasicShader shader;

        string blendName = "Default";

        public IGraphicsContext GraphicsContext { get { return Scene.GameContext.GraphicsContext; } }

        public float HalfTexel = 0.5f;

        Sprite pixelSprite;

        public int DrawCalls { get; set; }

        public SpriteLayer()
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
            ActiveShader = shader;
            //TextShader = new TextShader(graphicsContext, VertexElementContainer.VertexPositionTexColor);

            pixelSprite = new Sprite(GraphicsContext.PixelTexture, 1, new Vector2(1f, 1f), new Vector2(0f, 0f),
                new Vector2(1f, 1f), new Vector2());
        }

        public void SetShader(BasicShader shader)
        {
            this.ActiveShader = shader;
        }

        public void SetBlendMode(string name)
        {
            blendName = name;
        }

        public void ResetShader()
        {
            this.ActiveShader = shader;
        }

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

            float rx = (GraphicsContext.ActiveViewport.SceneSize.X / GraphicsContext.ActiveViewport.ScreenSize.X) + 0.0f;

            float ry = (GraphicsContext.ActiveViewport.SceneSize.Y / GraphicsContext.ActiveViewport.ScreenSize.Y) + 0.0f;

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

        public void Push(Sprite sprite, Vector2 position, Vector2 origin, Vector2 scale, Color color, int imageIndex, float angle)
        {
            quadList.Add(GetQuad(sprite, position, origin, scale, color, imageIndex, angle));
        }

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

        public void DrawRectangle(RotatedRectangle rect, Color color)
        {
            Vector2 origin = new Vector2((float)(rect.Origin.X / rect.Width), (float)(rect.Origin.Y / rect.Height));
            Push(pixelSprite, new Vector2(rect.X, rect.Y), origin,
                    new Vector2(rect.Width, rect.Height), color, 0, rect.Angle);
        }

        public void DrawHorizontalLine(Vector2 start, float length, Color color)
        {
            Push(pixelSprite, new Vector2(start.X, start.Y), Vector2.Zero,
                    new Vector2(length, 1), color, 0, 0);
        }

        public void DrawVerticalLine(Vector2 start, float length, Color color)
        {
            Push(pixelSprite, new Vector2(start.X, start.Y), Vector2.Zero,
                    new Vector2(1, length), color, 0, 0);
        }
    }
}
