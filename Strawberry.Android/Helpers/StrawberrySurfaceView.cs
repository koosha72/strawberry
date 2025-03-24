using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace Strawberry.Android.Helpers;

public class StrawberrySurfaceView : SurfaceView, ISurfaceHolderCallback
{
    public event Action<ISurfaceHolder> OnSurfaceDestroyed = null;
    public event Action<ISurfaceHolder> OnSurfaceCreated = null;
    public event Action<ISurfaceHolder, Format, int, int> OnSurfaceChanged = null;

    public StrawberrySurfaceView(Context context) : base(context)
    {
        Init();
    }

    public StrawberrySurfaceView(Context context, IAttributeSet attrs) : base(context, attrs)
    {
        Init();
    }

    private void Init()
    {
        Holder?.AddCallback(this);
    }


    public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
    {
        OnSurfaceChanged?.Invoke(holder, format, width, height);
    }

    public void SurfaceCreated(ISurfaceHolder holder)
    {
        OnSurfaceCreated?.Invoke(holder);
    }

    public void SurfaceDestroyed(ISurfaceHolder holder)
    {
        OnSurfaceDestroyed?.Invoke(holder);
    }
}
