using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monomino.Enums;

namespace Monomino.Interfaces;

public interface IState
{
    public GameState GetGameState();

    public ChangeState GetChangeState();

    public void Update(GameTime gameTime);

    public void Draw(SpriteBatch spriteBatch);

    public void OnResume();
}
