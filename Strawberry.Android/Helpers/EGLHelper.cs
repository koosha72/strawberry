using Android.Opengl;
using Android.Views;
using Strawberry.Math;

namespace Strawberry.Android.Helpers;

public class EGLHelper
{
    private EGLDisplay? eglDisplay;
    private EGLContext? eglContext;
    private EGLSurface? eglSurface;

    public void Init(Surface surface)
    {
        eglDisplay = EGL14.EglGetDisplay(EGL14.EglDefaultDisplay);
        int[] version = new int[2];
        EGL14.EglInitialize(eglDisplay, version, 0, version, 1);

        int[] configAttribs = {
                EGL14.EglRedSize, 8,
                EGL14.EglGreenSize, 8,
                EGL14.EglBlueSize, 8,
                EGL14.EglAlphaSize, 8,
                EGL14.EglDepthSize, 16,
                EGL14.EglRenderableType, EGL14.EglOpenglEs2Bit,
                EGL14.EglNone
        };
        EGLConfig[] configs = new EGLConfig[1];
        int[] numConfigs = new int[1];
        EGL14.EglChooseConfig(eglDisplay, configAttribs, 0, configs, 0, 1, numConfigs, 0);

        int[] contextAttribs = {
                EGL14.EglContextClientVersion, 3,
                EGL14.EglNone
        };
        eglContext = EGL14.EglCreateContext(eglDisplay, configs[0], EGL14.EglNoContext, contextAttribs, 0);

        int[] surfaceAttribs = {
                EGL14.EglNone
        };
        eglSurface = EGL14.EglCreateWindowSurface(eglDisplay, configs[0], surface, surfaceAttribs, 0);
    }

    public void MakeCurrent()
    {
        EGL14.EglMakeCurrent(eglDisplay, eglSurface, eglSurface, eglContext);
    }

    public void SwapBuffers()
    {
        EGL14.EglSwapBuffers(eglDisplay, eglSurface);
    }

    public void CleanUp()
    {
        EGL14.EglDestroySurface(eglDisplay, eglSurface);
        EGL14.EglDestroyContext(eglDisplay, eglContext);
        EGL14.EglTerminate(eglDisplay);
    }

    public Vector2 GetScreenSize()
    {
        int[] w = new int[1];
        EGL14.EglQuerySurface(eglDisplay, eglSurface, EGL14.EglWidth, w, 0);
        int[] h = new int[1];
        EGL14.EglQuerySurface(eglDisplay, eglSurface, EGL14.EglWidth, h, 0);

        return new Vector2(w[0], h[0]);
    }
}
