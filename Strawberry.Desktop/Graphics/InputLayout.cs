using OpenTK.Graphics.OpenGL;
using Strawberry.Graphics;

namespace Strawberry.Desktop.Graphics
{
    internal class InputLayout
    {
        VertexElementContainer elements;
        public InputLayout(VertexElementContainer elements)
        {
            this.elements = elements;
        }

        public void Activate(Shader shader)
        {
            int offset = 0;
            int ind = 0;
            foreach (KeyValuePair<string, ElementFormats> format in elements.Elements)
            {
                ind = GL.GetAttribLocation(shader.Program, format.Key);
                GL.EnableVertexAttribArray(ind);
                switch (format.Value)
                {
                    case ElementFormats.Position2:
                        //GL.BindAttribLocation(shader.Program, ind, format.Key);
                        GL.VertexAttribPointer(ind, 2, VertexAttribPointerType.Float, false, elements.Size, offset);
                        offset += 8;
                        break;
                    case ElementFormats.Color:
                        //GL.BindAttribLocation(shader.Program, ind, format.Key);
                        GL.VertexAttribPointer(ind, 4, VertexAttribPointerType.Float, false, elements.Size, offset);
                        offset += 16;
                        break;
                }
            }
        }

        public void DeActivate()
        {
            for (int i = 0; i < elements.Elements.Count; i++)
            {
                GL.DisableVertexAttribArray(i);
            }
        }
    }
}
