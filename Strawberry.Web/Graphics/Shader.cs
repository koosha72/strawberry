using Strawberry.Graphics;
using Strawberry.Math;
using Strawberry.Web.Helpers;

namespace Strawberry.Web.Graphics;

public class Shader : Strawberry.Graphics.Shader
{
    GraphicsContext graphicsContext;
    public override IGraphicsContext GraphicsContext { get { return graphicsContext; } }

    int program;

    public int Program { get { return program; } }

    InputLayout layout;

    public Shader(IGraphicsContext context, string vsCode, string fsCode, VertexElementContainer elements)
    {
        program = GL.CreateProgram();

        int status = 0;
        string info = "";
        int vertexShader = GL.CreateShader(GL.VertexShader);
        GL.ShaderSource(vertexShader, 1, new[] { vsCode }, null);
        GL.CompileShader(vertexShader);
        info = GL.GetShaderInfoLog(vertexShader);
        GL.GetShaderiv(vertexShader, GL.CompileStatus, out status);

        if (status != 1)
        {
            GL.DeleteShader(vertexShader);
            GL.DeleteProgram(program);

            program = 0;
            throw new Exception(info);
        }

        GL.AttachShader(program, vertexShader);
        GL.DeleteShader(vertexShader);

        int fragmentShader = GL.CreateShader(GL.FragmentShader);
        GL.ShaderSource(fragmentShader, 1, new[] { fsCode }, null);
        GL.CompileShader(fragmentShader);
        info = GL.GetShaderInfoLog(fragmentShader);
        GL.GetShaderiv(fragmentShader, GL.CompileStatus, out status);

        if (status != 1)
        {
            GL.DeleteShader(fragmentShader);
            GL.DeleteProgram(program);

            program = 0;
            throw new Exception(info);
        }

        GL.AttachShader(program, fragmentShader);
        GL.DeleteShader(fragmentShader);

        this.graphicsContext = (GraphicsContext)context;
        layout = new InputLayout(elements);
        GL.LinkProgram(program);
    }

    public override void Activate()
    {
        GL.UseProgram(program);
        graphicsContext.ActiveShader = this;
    }

    internal void ActivateLayout()
    {
        layout.Activate(this);
    }

    internal void DeActivateLayout()
    {
        layout.DeActivate();
    }

    public override void SetMatrixParameterByName(string constant, string name, Matrix4 mat, bool transpose)
    {
        GL.UniformMatrix4fv(GL.GetUniformLocation(program, name), 1, transpose,
            mat.Array);
    }

    protected override void CleanManaged()
    {
        GL.UseProgram(0);
        if (program > 0)
            GL.DeleteProgram(program);

        program = 0;
    }
}
