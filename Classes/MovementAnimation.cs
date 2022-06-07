using System;
using MonoCustoms;

namespace Monomino.Classes;

public class MovementAnimation
{
    public double FromX { get; set; }
    public double ToX { get; set; }
    public double FromY { get; set; }
    public double ToY { get; set; }
    public double Duration { get; set; }

    private Timer timer;

    public MovementAnimation(double fromX, double toX, double fromY, double toY, double duration, double start)
    {
        FromX = fromX;
        ToX = toX;
        FromY = fromY;
        ToY = toY;
        Duration = duration;

        timer = new Timer(start);
    }

    public double GetPercent(double gameTime)
    {
        return Math.Min(1.0, timer.GetElapsedMilliseconds(gameTime) / Duration);
    }
    public double GetX(double gameTime)
    {
        return FromX + (ToX - FromX) * GetPercent(gameTime);
    }
    public double GetY(double gameTime)
    {
        return FromY + (ToY - FromY) * GetPercent(gameTime);
    }
    public void Restart(double gameTime)
    {
        timer.Restart(gameTime);
    }
    public double GetElapsedMilliseconds(double gameTime)
    {
        return timer.GetElapsedMilliseconds(gameTime);
    }
}
