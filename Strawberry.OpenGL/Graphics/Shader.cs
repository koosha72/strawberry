using OpenTK.Graphics.OpenGL;
using Strawberry.Graphics;
using Strawberry.Math;

namespace Strawberry.OpenGL.Graphics
{
    public class Shader : Base, IShader
    {
        GraphicsContext graphicsContext;
        public IGraphicsContext GraphicsContext { get { return graphicsContext; } }

        int program;

        public int Program { get { return program; } }

        InputLayout layout;

        public Shader(IGraphicsContext context, string vsCode, string fsCode, VertexElementContainer elements)
        {
            program = GL.CreateProgram();

            int status = 0;
            string info = "";
            int vertexShader = GL.CreateShader(OpenTK.Graphics.OpenGL.ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vsCode);
            GL.CompileShader(vertexShader);
            GL.GetShaderInfoLog(vertexShader, out info);
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out status);

            if (status != 1)
            {
                GL.DeleteShader(vertexShader);
                GL.DeleteProgram(program);

                program = 0;
                throw new Exception(info);
            }

            GL.AttachShader(program, vertexShader);
            GL.DeleteShader(vertexShader);

            int fragmentShader = GL.CreateShader(OpenTK.Graphics.OpenGL.ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fsCode);
            GL.CompileShader(fragmentShader);
            GL.GetShaderInfoLog(fragmentShader, out info);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out status);

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

        public void Activate()
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

        public void SetMatrixParameterByName(string constant, string name, Matrix4 mat, bool transpose)
        {
            GL.UniformMatrix4(GL.GetUniformLocation(program, name), 1, transpose,
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
}
