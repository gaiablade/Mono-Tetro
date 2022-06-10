using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoCustoms;
using BinksFarm.Constants;
using BinksFarm.Enums;
using System;

namespace BinksFarm.States;

public class Sprint : SinglePlayerGameMode
{
    private readonly Timer timer;
    private int frame;
    private string timeString = "00:00";
    public Sprint(GraphicsDevice graphicsDevice)
        : base(new SinglePlayerGameConfig
        {
            GameConfig = App.GameConfig,
            BackgroundTextureFilename = Resource.BG06,
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
        var str1 = $"Lines: {linesCleared}/40";
        var str2 = $"Time: {timeString}";

        var dim1 = Font.MeasureStringScaled(str1, 0.75F);
        var dim2 = Font.MeasureStringScaled(str2, 0.75F);

        var tw = Font.MeasureStringScaled("Lines: 40/40", 0.75F).X + 50;
        var th = dim1.Y + dim2.Y + 50;
        var tx = BorderXO + Border.Width + NextPiecesCell.Width + 10;
        var ty = BorderYO;
        spriteBatch.DrawScaled(texture,
            new Rectangle(tx, ty, (int)tw, (int)th),
            Color.Black * 0.75F,
            App.Scale);

        var pos = new Vector2(tx + 10, BorderYO);
        spriteBatch.DrawStringOffset(Font, str1, Color.White, pos.Scale(App.Scale), scale: 0.75F.Scale(App.Scale));
        pos = new Vector2(tx + 10, BorderYO + dim1.Y + 10);
        spriteBatch.DrawStringOffset(Font, str2, Color.White, pos.Scale(App.Scale), scale: 0.75F.Scale(App.Scale));
        /*
        spriteBatch.DrawStringOffset(Font, str1, Color.White, 
            new Vector2(BorderXO + Border.Width.Scale(App.Scale) + 100.Scale(App.Scale), BorderYO), scale: 0.75F.Scale(App.Scale));
        spriteBatch.DrawStringOffset(Font, str2, Color.White,
            new Vector2(BorderXO + Border.Width.Scale(App.Scale) + 100.Scale(App.Scale), 
                BorderYO + dim1.Y.Scale(App.Scale) + 10.Scale(App.Scale)), scale: 0.75F.Scale(App.Scale));
        */
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

        if (!IsGameFinished())
        {
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
    }

    protected override int CalculateLevel() => 0;

    private string GetFullTimeString()
    {
        var time = timer.GetElapsedMilliseconds(gameTime);
        var minutes = (int)time / (60 * 1000);
        var seconds = ((int)time / 1000) % 60;
        var milliseconds = (time / 1000) - Math.Truncate(time / 1000);
        return string.Format("{0}:{1}.{2}",
            minutes.ToString().PadLeft(2, '0'),
            seconds.ToString().PadLeft(2, '0'),
            milliseconds.ToString().Substring(2).Truncate(3));
    }

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
        timeString = GetFullTimeString();
    }

    protected override void OnGameOver()
    {
    }
    #endregion
}
