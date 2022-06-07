using System;

namespace Monomino.Classes;

public class ProgressAnimation
{
    private readonly double Duration;
    private readonly double Start;

    public ProgressAnimation(double duration, double start)
    {
        Duration = duration;
        Start = start;
    }

    public double GetPercent(double gameTime)
    {
        return Math.Min(1.0, (gameTime - Start) / Duration);
    }
}
