using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoCustoms;

namespace Monomino.Interfaces;

public interface ITransition
{
    public double Duration { get; set; }
    public Timer @Timer { get; set; }
    public double @GameTime { get; set; }

    public void Draw(SpriteBatch spriteBatch);
    public void Update(GameTime gameTime);
    public bool IsDone();
}
