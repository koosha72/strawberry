using Android.Opengl;
using Strawberry.Graphics;
using Strawberry.Math;

namespace Strawberry.Android.Graphics;

public class Shader : Strawberry.Graphics.Shader
{
    GraphicsContext graphicsContext;
    public override IGraphicsContext GraphicsContext { get { return graphicsContext; } }

    int program;

    public int Program { get { return program; } }

    InputLayout layout;

    public Shader(IGraphicsContext context, string vsCode, string fsCode, VertexElementContainer elements)
    {
        program = GLES30.GlCreateProgram();

        int[] cs = new int[1] { -1 };
        int status = 0;
        string info = "";
        int vertexShader = GLES30.GlCreateShader(GLES30.GlVertexShader);
        GLES30.GlShaderSource(vertexShader, vsCode);
        GLES30.GlCompileShader(vertexShader);
        info = GLES30.GlGetShaderInfoLog(vertexShader);
        GLES30.GlGetShaderiv(vertexShader, GLES30.GlCompileStatus, cs, 0);
        status = cs[0];

        if (status != GLES30.GlTrue)
        {
            GLES30.GlDeleteShader(vertexShader);
            GLES30.GlDeleteProgram(program);

            program = 0;
            throw new Exception(info);
        }

        GLES30.GlAttachShader(program, vertexShader);
        GLES30.GlDeleteShader(vertexShader);

        int fragmentShader = GLES30.GlCreateShader(GLES30.GlFragmentShader);
        GLES30.GlShaderSource(fragmentShader, fsCode);
        GLES30.GlCompileShader(fragmentShader);
        info = GLES30.GlGetShaderInfoLog(fragmentShader);
        GLES30.GlGetShaderiv(fragmentShader, GLES30.GlCompileStatus, cs, 0);
        status = cs[0];

        if (status != GLES30.GlTrue)
        {
            GLES30.GlDeleteShader(fragmentShader);
            GLES30.GlDeleteProgram(program);

            program = 0;
            throw new Exception(info);
        }

        GLES30.GlAttachShader(program, fragmentShader);
        GLES30.GlDeleteShader(fragmentShader);

        this.graphicsContext = (GraphicsContext)context;
        layout = new InputLayout(elements);
        GLES30.GlLinkProgram(program);
    }

    public override void Activate()
    {
        GLES30.GlUseProgram(program);
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
        GLES30.GlUniformMatrix4fv(GLES30.GlGetUniformLocation(program, name), 1, transpose,
            mat.Array, 0);
    }

    protected override void CleanManaged()
    {
        GLES30.GlUseProgram(0);
        if (program > 0)
            GLES30.GlDeleteProgram(program);

        program = 0;
    }
}
