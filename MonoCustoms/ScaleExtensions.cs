using Microsoft.Xna.Framework;

namespace MonoCustoms;

public static class ScaleExtensions
{
    public static int Scale(this int t, double scale)
    {
        return (int)((double)t * scale);
    }

    public static float Scale(this float t, double scale)
    {
        return (float)((double)t * scale);
    }

    public static double Scale(this double t, double scale)
    {
        return t * scale;
    }

    public static Vector2 Scale(this Vector2 t, double scale)
    {
        return new Vector2(t.X.Scale(scale), t.Y.Scale(scale));
    }

    public static Rectangle Scale(this Rectangle t, double scale)
    {
        return new Rectangle(t.X.Scale(scale), t.Y.Scale(scale), t.Width.Scale(scale), t.Height.Scale(scale));
    }
}
