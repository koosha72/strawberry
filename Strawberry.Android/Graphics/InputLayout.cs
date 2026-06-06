using Android.Opengl;
using Strawberry.Graphics;

namespace Strawberry.Android.Graphics;

public class InputLayout
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
            ind = GLES30.GlGetAttribLocation(shader.Program, format.Key);
            GLES30.GlEnableVertexAttribArray(ind);
            switch (format.Value)
            {
                case ElementFormats.Position2:
                    //GL.BindAttribLocation(shader.Program, ind, format.Key);
                    GLES30.GlVertexAttribPointer(ind, 2, GLES30.GlFloat, false, elements.Size, offset);
                    offset += 8;
                    break;
                case ElementFormats.Color:
                    //GL.BindAttribLocation(shader.Program, ind, format.Key);
                    GLES30.GlVertexAttribPointer(ind, 4, GLES30.GlFloat, false, elements.Size, offset);
                    offset += 16;
                    break;
            }
        }
    }

    public void DeActivate()
    {
        for (int i = 0; i < elements.Elements.Count; i++)
        {
            GLES30.GlDisableVertexAttribArray(i);
        }
    }
}
