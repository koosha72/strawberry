using System;
using System.Runtime.InteropServices;

namespace Strawberry.Web.Helpers;

public class GL
{
    [DllImport("libGLESv3")]
    public static extern void glClearColor(float r, float g, float b, float a);

    [DllImport("libGLESv3")]
    public static extern void glClear(int mask);

    [DllImport("libGLESv3")]
    public static extern void glViewport(int x, int y, uint width, uint height);
}
