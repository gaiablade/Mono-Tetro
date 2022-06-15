namespace MonoCustoms;

public static class IntExtensions
{
    public static int ClampLoop(this int t, int min, int max)
    {
        if (t < min)
            return max;
        if (t > max)
            return min;
        return t;
    }
}
