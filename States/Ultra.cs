using Monomino.Constants;
using Monomino.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoCustoms;

namespace Monomino.States;

public class Ultra : SinglePlayerGameMode
{
    private const int Seconds = 120;
    private readonly Timer GameTimer;
    private readonly int StatsWidth;
    private int frames = 0;
    private string timeString = "01:59";

    public Ultra(GraphicsDevice @GraphicsDevice)
        : base(new SinglePlayerGameConfig 
        {
            BackgroundTextureFilename = Resource.BG01,
            FontFilename = Resource.FT01,
            GameConfig = App.GameConfig,
            GraphicsDevice = GraphicsDevice,
            MenuPointerTextureFilename = Resource.MP01
        })
    {
        GameTimer = new Timer(new GameTime().TotalGameTime.TotalMilliseconds);
        var dim = Font.MeasureStringScaled($"Time Left: {timeString}", 0.75F);
        StatsWidth = (int)(dim.X + 50).Scale(App.Scale);
    }

    public override ChangeState GetChangeState() => ChangeState.NoChange;

    public override GameState GetGameState() => GameState.Marathon;

    protected override void DrawStats(SpriteBatch spriteBatch)
    {
        var scoreString = $"Score: {score}";
        var timeLeftString = $"Time Left: {timeString}";
        var linesString = $"Lines: {linesCleared} / 150";

        var dim1 = Font.MeasureStringScaled(scoreString, 0.75F.Scale(App.Scale));
        var dim2 = Font.MeasureStringScaled(timeLeftString, 0.75F.Scale(App.Scale));
        var dim3 = Font.MeasureStringScaled(linesString, 0.75F.Scale(App.Scale));

        spriteBatch.Draw(texture,
            new Rectangle(BorderXO + Border.Width.Scale(App.Scale) + 110.Scale(App.Scale),
            BorderYO - 10.Scale(App.Scale),
            StatsWidth, (int)(dim1.Y + dim2.Y + dim3.Y + 50.Scale(App.Scale))),
            Color.Black * 0.75F);

        var pos = new Vector2(BorderXO + Border.Width.Scale(App.Scale) + 120.Scale(App.Scale), BorderYO);
        spriteBatch.DrawStringOffset(Font, scoreString, Color.White, pos, scale: 0.75F.Scale(App.Scale));
        pos = new Vector2(BorderXO + Border.Width.Scale(App.Scale) + 120.Scale(App.Scale),
            BorderYO + dim1.Y + 10.Scale(App.Scale));
        spriteBatch.DrawStringOffset(Font, timeLeftString, Color.White, pos, scale: 0.75F.Scale(App.Scale));
        pos = new Vector2(BorderXO + Border.Width.Scale(App.Scale) + 120.Scale(App.Scale),
            BorderYO + dim1.Y + dim2.Y + 20.Scale(App.Scale));
        spriteBatch.DrawStringOffset(Font, linesString, Color.White, pos, scale: 0.75F.Scale(App.Scale));
    }

    protected override bool IsGameFailed()
    {
        return false;
    }

    protected override bool IsGameFinished()
    {
        return GameTimer.GetElapsedMilliseconds(gameTime) >= Seconds * 1000;
    }

    protected override void OnGameFinished()
    {
    }

    protected override void OnGameOver()
    {
    }

    #region Overrides
    protected override void DrawVictoryScreen(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, WindowSize, Color.Black * 0.75F);

        var str1 = "Time's Up!";
        var dim1 = Font.MeasureStringScaled(str1, 1F);
        spriteBatch.DrawStringOffset(Font, str1, Color.White,
            new Vector2(WindowSize.Width / 2 - dim1.X / 2, WindowSize.Height / 2 - dim1.Y / 2));

        var str2 = $"Your Score: {score}";
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
        spriteBatch.Draw(Background,
            WindowSize,
            Background.Bounds,
            Color.White);
    }

    protected override int CalculateLevel() => 0;

    public override void Update(GameTime gt)
    {
        base.Update(gt);

        frames++;
        if (frames % 30 == 0)
        {
            // update time string
            var timeElapsed = GameTimer.GetElapsedMilliseconds(gameTime);
            var timeLeft = (Seconds * 1000) - timeElapsed;
            var minutes = (int)timeLeft / (60 * 1000);
            var seconds = ((int)timeLeft / 1000) % 60;

            timeString = minutes.ToString().PadLeft(2, '0') + ':' + 
                seconds.ToString().PadLeft(2, '0');
        }
    }
    #endregion
}
