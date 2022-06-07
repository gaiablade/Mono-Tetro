using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoCustoms;
using BinksFarm.Interfaces;
using System;

namespace BinksFarm.Classes;

public class FadeInTransition : ITransition
{
    public double Duration { get; set; }
    public Timer Timer { get; set; }
    public double GameTime { get; set; }

    private readonly Texture2D Texture;

    public FadeInTransition(GraphicsDevice graphicsDevice, double duration, double startTime)
    {
        Duration = duration;
        Timer = new Timer(startTime);
        GameTime = startTime;

        Texture = new Texture2D(graphicsDevice, 1, 1);
        Texture.SetData(new Color[] { Color.White });
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        var percent = Math.Min(1.0, Timer.GetElapsedMilliseconds(GameTime) / Duration);
        var opacity = 1F - EaseInOutFunctions.EaseOutCubic(percent);
        spriteBatch.Draw(Texture, spriteBatch.GraphicsDevice.PresentationParameters.Bounds, Color.Black * (float)opacity);
        spriteBatch.End();
    }

    public void Update(GameTime gameTime)
    {
        GameTime += gameTime.ElapsedGameTime.TotalMilliseconds;
    }

    public bool IsDone()
    {
        return Timer.GetElapsedMilliseconds(GameTime) >= Duration;
    }
}
