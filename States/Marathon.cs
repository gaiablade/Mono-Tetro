using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monomino.Constants;
using Monomino.Enums;
using MonoCustoms;

namespace Monomino.States;

public class Marathon : SinglePlayerGameMode
{
    public Marathon(GraphicsDevice graphicsDevice)
        : base(new SinglePlayerGameConfig
        {
            GameConfig = App.GameConfig,
            GraphicsDevice = graphicsDevice,
            BackgroundTextureFilename = Resource.BG01,
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

        var dim1 = Font.MeasureStringScaled(scoreString, 0.75F.Scale(App.Scale));
        var dim2 = Font.MeasureStringScaled(levelString, 0.75F.Scale(App.Scale));
        var dim3 = Font.MeasureStringScaled(linesString, 0.75F.Scale(App.Scale));

        spriteBatch.Draw(texture, 
            new Rectangle(BorderXO + Border.Width.Scale(App.Scale) + 110.Scale(App.Scale),
            BorderYO - 10.Scale(App.Scale),
            (int)(dim3.X + 50.Scale(App.Scale)), (int)(dim1.Y + dim2.Y + dim3.Y + 50.Scale(App.Scale))), 
            Color.Black * 0.75F);

        var pos = new Vector2(BorderXO + Border.Width.Scale(App.Scale) + 120.Scale(App.Scale), BorderYO);
        spriteBatch.DrawStringOffset(Font, scoreString, Color.White, pos, scale: 0.75F.Scale(App.Scale));
        pos = new Vector2(BorderXO + Border.Width.Scale(App.Scale) + 120.Scale(App.Scale), 
            BorderYO + dim1.Y + 10.Scale(App.Scale));
        spriteBatch.DrawStringOffset(Font, levelString, Color.White, pos, scale: 0.75F.Scale(App.Scale));
        pos = new Vector2(BorderXO + Border.Width.Scale(App.Scale) + 120.Scale(App.Scale),
            BorderYO + dim1.Y + dim2.Y + 20.Scale(App.Scale));
        spriteBatch.DrawStringOffset(Font, linesString, Color.White, pos, scale: 0.75F.Scale(App.Scale));
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
