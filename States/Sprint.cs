using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoCustoms;
using Monomino.Constants;
using Monomino.Enums;

namespace Monomino.States;

public class Sprint : SinglePlayerGameMode
{
    private readonly Timer timer;
    private int frame;
    private string timeString = "00:00";
    public Sprint(GraphicsDevice graphicsDevice)
        : base(new SinglePlayerGameConfig
        {
            GameConfig = App.GameConfig,
            BackgroundTextureFilename = Resource.BG03,
            FontFilename = Resource.FT01,
            GraphicsDevice = graphicsDevice,
            MenuPointerTextureFilename = Resource.MP01
        })
    {
        timer = new Timer(gameTime);
    }
    public override ChangeState GetChangeState()
    {
        return ChangeState.NoChange;
    }

    public override GameState GetGameState()
    {
        return GameState.Marathon;
    }

    protected override void DrawStats(SpriteBatch spriteBatch)
    {
        var str1 = $"Lines: {linesCleared}";
        var dim1 = Font.MeasureStringScaled(str1, 0.75F.Scale(App.Scale));
        spriteBatch.DrawStringOffset(Font, str1, Color.White, 
            new Vector2(BorderXO + Border.Width.Scale(App.Scale) + 100.Scale(App.Scale), BorderYO), scale: 0.75F.Scale(App.Scale));
        var str2 = $"Time: {timeString}";
        spriteBatch.DrawStringOffset(Font, str2, Color.White,
            new Vector2(BorderXO + Border.Width.Scale(App.Scale) + 100.Scale(App.Scale), 
                BorderYO + dim1.Y.Scale(App.Scale) + 10.Scale(App.Scale)), scale: 0.75F.Scale(App.Scale));
    }

    protected override bool IsGameFailed()
    {
        return false;
    }

    protected override bool IsGameFinished()
    {
        return linesCleared > 39;
    }

    #region Overrides
    public override void Update(GameTime gt)
    {
        base.Update(gt);
        frame++;
        if (frame % 60 == 0)
        {
            // update time string
            var seconds = (int)(timer.GetElapsedMilliseconds(gameTime) / 1000.0);
            var minutes = seconds / 60;
            seconds = seconds % 60;
            timeString = minutes.ToString().PadLeft(2, '0') + ':' + 
                seconds.ToString().PadLeft(2, '0');
        }
    }

    protected override int CalculateLevel() => 0;

    protected override void DrawVictoryScreen(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, WindowSize, Color.Black * 0.75F);

        var str1 = "You won!";
        var dim1 = Font.MeasureStringScaled(str1, 1F);
        spriteBatch.DrawStringOffset(Font, str1, Color.White,
            new Vector2(WindowSize.Width / 2 - dim1.X / 2, WindowSize.Height / 2 - dim1.Y / 2));

        var str2 = $"Your Time: {timeString}";
        var dim2 = Font.MeasureStringScaled(str2, 1F);
        spriteBatch.DrawStringOffset(Font, str2, Color.White,
            new Vector2(WindowSize.Width / 2 - dim2.X / 2, WindowSize.Height / 2 + dim1.Y / 2 + 10));

        var str3 = "Press Enter to Return to the Main Menu";
        var dim3 = Font.MeasureStringScaled(str3, 1F);
        spriteBatch.DrawStringOffset(Font, str3, Color.White, 
            new Vector2(WindowSize.Width / 2 - dim3.X / 2, WindowSize.Height / 2 + dim1.Y / 2 + dim2.Y + 20));
    }

    protected override void DrawBackground(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Background, WindowSize, Background.Bounds, Color.White);
    }

    protected override void OnGameFinished()
    {
    }

    protected override void OnGameOver()
    {
    }
    #endregion
}
