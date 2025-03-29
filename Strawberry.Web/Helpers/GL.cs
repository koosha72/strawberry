using System;
using System.Runtime.InteropServices;

namespace Strawberry.Web.Helpers;

public static partial class GL
{
    private const string LibGLES = "libGLESv3";

    [DllImport(LibGLES, EntryPoint = "glClearColor")]
    public static extern void ClearColor(float r, float g, float b, float a);

    [DllImport(LibGLES, EntryPoint = "glClear")]
    public static extern void Clear(int mask);

    [DllImport(LibGLES, EntryPoint = "glViewport")]
    public static extern void Viewport(int x, int y, int width, int height);

    [DllImport(LibGLES, EntryPoint = "glEnable")]
    public static extern void Enable(int cap);

    [DllImport(LibGLES, EntryPoint = "glBlendColor")]
    public static extern void BlendColor(float r, float g, float b, float a);

    [DllImport(LibGLES, EntryPoint = "glBlendFuncSeparate")]
    public static extern void BlendFuncSeparate(
    int sfactorRGB,   // Blend factor for RGB (e.g., GL_SRC_ALPHA)
    int dfactorRGB,   // Destination blend factor for RGB (e.g., GL_ONE_MINUS_SRC_ALPHA)
    int sfactorAlpha, // Blend factor for Alpha
    int dfactorAlpha  // Destination blend factor for Alpha
);

    // glBlendEquationSeparate
    [DllImport(LibGLES, EntryPoint = "glBlendEquationSeparate")]
    public static extern void BlendEquationSeparate(
        int modeRGB,    // Blend equation for RGB (e.g., GL_FUNC_ADD)
        int modeAlpha   // Blend equation for Alpha
    );

    // glBindFramebuffer
    [DllImport(LibGLES, EntryPoint = "glBindFramebuffer")]
    public static extern void BindFramebuffer(int target, int framebuffer);

    [DllImport(LibGLES, EntryPoint = "glGenRenderbuffers")]
    public static extern void GenRenderbuffers(int n, int[] buffers);

    [DllImport(LibGLES, EntryPoint = "glDeleteRenderbuffers")]
    public static extern void DeleteRenderbuffers(int n, int[] buffers);

    [DllImport(LibGLES, EntryPoint = "glGenFramebuffers")]
    public static extern void GenFramebuffers(int n, int[] buffers);

    [DllImport(LibGLES, EntryPoint = "glDeleteFramebuffers")]
    public static extern void DeleteFramebuffers(int n, int[] buffers);

    [DllImport(LibGLES, EntryPoint = "glGenTextures")]
    public static extern void GenTextures(int n, int[] buffers);


    [DllImport(LibGLES, EntryPoint = "glBindRenderbuffer")]
    public static extern void BindRenderbuffer(int target, int buffer);

    [DllImport(LibGLES, EntryPoint = "glBindTexture")]
    public static extern void BindTexture(int target, int texture);

    [DllImport(LibGLES, EntryPoint = "glRenderbufferStorage")]
    public static extern void RenderbufferStorage(
    int target, int internalFormat, int width, int height);

    [DllImport(LibGLES, EntryPoint = "glFramebufferTexture2D")]
    public static extern void FramebufferTexture2D(int target, int attachment, int textarget, int texture, int level);

    [DllImport(LibGLES, EntryPoint = "glFramebufferRenderbuffer")]
    public static extern void FramebufferRenderbuffer(int target, int attachment, int Renderbuffertarget, int renderbuffer);

    [DllImport(LibGLES, EntryPoint = "glCheckFramebufferStatus")]
    public static extern int CheckFramebufferStatus(int target);

    // Texture Functions
    [DllImport(LibGLES, EntryPoint = "glActiveTexture")]
    public static extern void ActiveTexture(int texture);

    [DllImport(LibGLES, EntryPoint = "glDeleteTextures")]
    public static extern void DeleteTextures(int n, [In] int[] textures);

    [DllImport(LibGLES, EntryPoint = "glTexImage2D")]
    public static extern void TexImage2D(int target, int level, int internalformat, int width, int height, int border, int format, int type, IntPtr pixels);

    [DllImport(LibGLES, EntryPoint = "glTexSubImage2D")]
    public static extern void TexSubImage2D(int target, int level, int xoffset, int yoffset, int width, int height, int format, int type, IntPtr pixels);

    [DllImport(LibGLES, EntryPoint = "glTexParameteri")]
    public static extern void TexParameteri(int target, int pname, int param);

    [DllImport(LibGLES, EntryPoint = "glTexParameterf")]
    public static extern void TexParameterf(int target, int pname, float param);

    [DllImport(LibGLES, EntryPoint = "glGenerateMipmap")]
    public static extern void GenerateMipmap(int target);

    // Uniform Functions
    [DllImport(LibGLES, EntryPoint = "glUniform1i")]
    public static extern void Uniform1(int location, int v0);

    [DllImport(LibGLES, EntryPoint = "glUniform2i")]
    public static extern void Uniform2(int location, int v0, int v1);

    [DllImport(LibGLES, EntryPoint = "glUniform3i")]
    public static extern void Uniform3(int location, int v0, int v1, int v2);

    [DllImport(LibGLES, EntryPoint = "glUniform4i")]
    public static extern void Uniform4(int location, int v0, int v1, int v2, int v3);

    [DllImport(LibGLES, EntryPoint = "glUniform1f")]
    public static extern void Uniform1(int location, float v0);

    [DllImport(LibGLES, EntryPoint = "glUniform2f")]
    public static extern void Uniform2(int location, float v0, float v1);

    [DllImport(LibGLES, EntryPoint = "glUniform3f")]
    public static extern void Uniform3(int location, float v0, float v1, float v2);

    [DllImport(LibGLES, EntryPoint = "glUniform4f")]
    public static extern void Uniform4(int location, float v0, float v1, float v2, float v3);

    [DllImport(LibGLES, EntryPoint = "glUniformMatrix2fv")]
    public static extern void UniformMatrix2(int location, int count, bool transpose, float[] value);

    [DllImport(LibGLES, EntryPoint = "glUniformMatrix3fv")]
    public static extern void UniformMatrix3(int location, int count, bool transpose, float[] value);

    [DllImport(LibGLES, EntryPoint = "glUniformMatrix4fv")]
    public static extern void UniformMatrix4(int location, int count, bool transpose, float[] value);

    [DllImport(LibGLES, EntryPoint = "glGetUniformLocation")]
    public static extern int GetUniformLocation(int program, string name);

    // Sampler Functions
    [DllImport(LibGLES, EntryPoint = "glBindSampler")]
    public static extern void BindSampler(int unit, int sampler);

    [DllImport(LibGLES, EntryPoint = "glGenSamplers")]
    public static extern void GenSamplers(int count, [Out] int[] samplers);

    [DllImport(LibGLES, EntryPoint = "glDeleteSamplers")]
    public static extern void DeleteSamplers(int count, [In] int[] samplers);

    [DllImport(LibGLES, EntryPoint = "glSamplerParameteri")]
    public static extern void SamplerParameteri(int sampler, int pname, int param);

    [DllImport(LibGLES, EntryPoint = "glSamplerParameterf")]
    public static extern void SamplerParameterf(int sampler, int pname, float param);

    [DllImport(LibGLES, EntryPoint = "glCreateShader")]
    public static extern int CreateShader(int type);

    [DllImport(LibGLES, EntryPoint = "glShaderSource")]
    public static extern void ShaderSource(int shader, int count, string[] source, int[] length);

    [DllImport(LibGLES, EntryPoint = "glCompileShader")]
    public static extern void CompileShader(int shader);

    [DllImport(LibGLES, EntryPoint = "glDeleteShader")]
    public static extern void DeleteShader(int shader);

    // --- Shader Info Log ---
    [DllImport(LibGLES, EntryPoint = "glGetShaderiv")]
    public static extern void GetShaderiv(int shader, int pname, out int @params);

    [DllImport(LibGLES, EntryPoint = "glGetShaderInfoLog")]
    public static extern void GetShaderInfoLog(int shader, int maxLength, out int length, System.Text.StringBuilder infoLog);

    // --- Program Creation & Linking ---
    [DllImport(LibGLES, EntryPoint = "glCreateProgram")]
    public static extern int CreateProgram();

    [DllImport(LibGLES, EntryPoint = "glAttachShader")]
    public static extern void AttachShader(int program, int shader);

    [DllImport(LibGLES, EntryPoint = "glLinkProgram")]
    public static extern void LinkProgram(int program);

    [DllImport(LibGLES, EntryPoint = "glUseProgram")]
    public static extern void UseProgram(int program);

    [DllImport(LibGLES, EntryPoint = "glDeleteProgram")]
    public static extern void DeleteProgram(int program);

    // --- Program Info Log ---
    [DllImport(LibGLES, EntryPoint = "glGetProgramiv")]
    public static extern void GetProgramiv(int program, int pname, out int @params);

    [DllImport(LibGLES, EntryPoint = "glGetProgramInfoLog")]
    public static extern void GetProgramInfoLog(int program, int maxLength, out int length, System.Text.StringBuilder infoLog);

    [DllImport(LibGLES, EntryPoint = "glGetAttribLocation")]
    public static extern int GetAttribLocation(int program, string name);

    [DllImport(LibGLES, EntryPoint = "glUniform1f")]
    public static extern void Uniform1f(int location, float v0);

    [DllImport(LibGLES, EntryPoint = "glUniform2f")]
    public static extern void Uniform2f(int location, float v0, float v1);

    [DllImport(LibGLES, EntryPoint = "glUniform3f")]
    public static extern void Uniform3f(int location, float v0, float v1, float v2);

    [DllImport(LibGLES, EntryPoint = "glUniform4f")]
    public static extern void Uniform4f(int location, float v0, float v1, float v2, float v3);

    [DllImport(LibGLES, EntryPoint = "glUniform1i")]
    public static extern void Uniform1i(int location, int v0);

    [DllImport(LibGLES, EntryPoint = "glUniformMatrix4fv")]
    public static extern void UniformMatrix4fv(int location, int count, bool transpose, float[] value);

    // --- Vertex Attributes ---
    [DllImport(LibGLES, EntryPoint = "glEnableVertexAttribArray")]
    public static extern void EnableVertexAttribArray(int index);

    [DllImport(LibGLES, EntryPoint = "glDisableVertexAttribArray")]
    public static extern void DisableVertexAttribArray(int index);

    [DllImport(LibGLES, EntryPoint = "glVertexAttribPointer")]
    public static extern void VertexAttribPointer(int index, int size, int type, bool normalized, int stride, IntPtr pointer);

    // --- Vertex Array Objects (VAO) ---
    [DllImport(LibGLES, EntryPoint = "glGenVertexArrays")]
    public static extern void GenVertexArrays(int n, int[] arrays);

    [DllImport(LibGLES, EntryPoint = "glBindVertexArray")]
    public static extern void BindVertexArray(int array);

    [DllImport(LibGLES, EntryPoint = "glDeleteVertexArrays")]
    public static extern void DeleteVertexArrays(int n, int[] arrays);

    // --- Buffer Objects (VBO/IBO) ---
    [DllImport(LibGLES, EntryPoint = "glGenBuffers")]
    public static extern void GenBuffers(int n, int[] buffers);

    [DllImport(LibGLES, EntryPoint = "glBindBuffer")]
    public static extern void BindBuffer(int target, int buffer);

    [DllImport(LibGLES, EntryPoint = "glBufferData")]
    public static extern void BufferData(int target, IntPtr size, IntPtr data, int usage);

    [DllImport(LibGLES, EntryPoint = "glBufferSubData")]
    public static extern void BufferSubData(int target, IntPtr offset, IntPtr size, IntPtr data);

    [DllImport(LibGLES, EntryPoint = "glDeleteBuffers")]
    public static extern void DeleteBuffers(int n, int[] buffers);

    [DllImport(LibGLES, EntryPoint = "glDrawArrays")]
    public static extern void DrawArrays(int mode, int first, int count);

    [DllImport(LibGLES, EntryPoint = "glDrawElements")]
    public static extern void DrawElements(int mode, int count, int type, IntPtr indices);

    [DllImport(LibGLES, EntryPoint = "glDrawRangeElements")]
    public static extern void DrawRangeElements(int mode, int start, int end, int count, int type, IntPtr indices);

    [DllImport(LibGLES, EntryPoint = "glDisable")]
    public static extern void Disable(int cap);

    [DllImport(LibGLES, EntryPoint = "glCullFace")]
    public static extern void CullFace(int mode);

    [DllImport(LibGLES, EntryPoint = "glGetError")]
    public static extern int GetError();

    public static string GetShaderInfoLog(int shader)
    {
        // Get the info log length
        GetShaderiv(shader, InfoLogLength, out int maxLength);

        if (maxLength <= 0)
        {
            return string.Empty; // No log available
        }

        // Allocate a StringBuilder to hold the log
        var infoLog = new System.Text.StringBuilder(maxLength);
        GetShaderInfoLog(shader, maxLength, out _, infoLog);

        return infoLog.ToString();
    }
}
