using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monomino.Classes;
using Monomino.Debug;

namespace Monomino.Menus;

public class TitleMenu : HighlightListMenu
{
    private Grid grid;
    public TitleMenu(GraphicsDevice graphicsDevice) : base(graphicsDevice)
    {
        this.MenuOptions = new MenuOption[] {
            new MenuOption("Marathon"),
            new MenuOption("Options"),
            new MenuOption("Exit Game")
        };

        this.HighlightColor = Color.DarkBlue;
        this.TopOfMenu = App.GameConfig.ResolutionY / 2 - (int)this.GetHeightOfMenu() / 2;
        this.LeftOfMenu = App.GameConfig.ResolutionX / 2 - (int)this.GetWidthOfLongestOption() / 2;
        //this.LeftOfMenu = 5;
        this.Alignment = TextAlignment.Center;

        grid = new Grid(graphicsDevice, App.GameConfig.ResolutionX, App.GameConfig.ResolutionY);
    }

    public new void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        base.Draw(spriteBatch);
        grid.Draw(spriteBatch);

        spriteBatch.End();
    }

    public new void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
}
