namespace Plugin.MauiTouchEffect.Extensions;

static class JavaObjectExtensions
{
    public static bool IsDisposed(this Java.Lang.Object obj)
        => obj.Handle == IntPtr.Zero;

    public static bool IsAlive(this Java.Lang.Object obj)
        => obj != null && !obj.IsDisposed();

    public static bool IsDisposed(this Android.Runtime.IJavaObject obj)
        => obj.Handle == IntPtr.Zero;

    public static bool IsAlive(this Android.Runtime.IJavaObject obj)
        => obj != null && !obj.IsDisposed();
}