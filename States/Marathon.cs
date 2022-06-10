using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BinksFarm.Constants;
using BinksFarm.Enums;
using MonoCustoms;

namespace BinksFarm.States;

public class Marathon : SinglePlayerGameMode
{
    public Marathon(GraphicsDevice graphicsDevice)
        : base(new SinglePlayerGameConfig
        {
            GameConfig = App.GameConfig,
            GraphicsDevice = graphicsDevice,
            BackgroundTextureFilename = Resource.BG04,
            FontFilename = Resource.FT01,
            MenuPointerTextureFilename = Resource.MP01
        })
    {
    }

    protected override bool IsGameFailed()
    {
        return false;
    }

    protected override bool IsGameFinished()
    {
        return linesCleared > 149;
    }

    protected override void DrawStats(SpriteBatch spriteBatch)
    {
        var scoreString = $"Score: {score}";
        var levelString = $"Level: {level}";
        var linesString = $"Lines: {linesCleared} / 150";

        var dim1 = Font.MeasureStringScaled(scoreString, 0.75F);
        var dim2 = Font.MeasureStringScaled(levelString, 0.75F);
        var dim3 = Font.MeasureStringScaled(linesString, 0.75F);

        var tw = dim3.X + 50;
        var th = dim1.Y + dim2.Y + dim3.Y + 50;
        var tx = BorderXO + Border.Width + NextPiecesCell.Width + 10;
        var ty = BorderYO;
        spriteBatch.DrawScaled(texture,
            new Rectangle(tx, ty, (int)tw, (int)th),
            Color.Black * 0.75F,
            App.Scale);

        var pos = new Vector2(tx + 10, BorderYO);
        spriteBatch.DrawStringOffset(Font, scoreString, Color.White, pos.Scale(App.Scale), scale: 0.75F.Scale(App.Scale));
        pos = new Vector2(tx + 10, BorderYO + dim1.Y + 10);
        spriteBatch.DrawStringOffset(Font, levelString, Color.White, pos.Scale(App.Scale), scale: 0.75F.Scale(App.Scale));
        pos = new Vector2(tx + 10, BorderYO + dim1.Y + dim2.Y + 20);
        spriteBatch.DrawStringOffset(Font, linesString, Color.White, pos.Scale(App.Scale), scale: 0.75F.Scale(App.Scale));
    }

    protected override void DrawBackground(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Background, 
            WindowSize, 
            Background.Bounds, 
            Color.White);
    }

    public override ChangeState GetChangeState()
    {
        return ChangeState.NoChange;
    }
    public override GameState GetGameState()
    {
        return GameState.Marathon;
    }

    protected override void OnGameFinished()
    {
    }

    protected override void OnGameOver()
    {
    }
}
