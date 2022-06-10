using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoCustoms;
using BinksFarm.Classes;
using BinksFarm.Constants;
using BinksFarm.Enums;
using System;
using System.Collections.Generic;

namespace BinksFarm.States;

public class TitleScreen : UserInputState
{
    #region Data
    private readonly List<(string Option, string Description, Action Callback)> Menu;
    private readonly string VersionNumber;
    #endregion

    #region Constants
    private const float FontScale = 1.0F;
    private const double PointerMoveTime = 100.0; // ms
    private const double PointerWaveTime = 1000.0;
    #endregion

    #region Rendering
    private readonly Vector2 WindowSize;
    private readonly List<DrawableTexture> OptionsTextures;
    private readonly List<DrawableTexture> DescriptionTextures;
    private readonly Texture2D MenuPointer;
    private readonly Texture2D Background;
    private readonly Texture2D Texture;
    private readonly SpriteFont Font;
    private readonly GraphicsDevice @GraphicsDevice;
    #endregion

    #region Animation
    private int hlp = 0; // highlight position
    private double currentGT;
    private MovementAnimation hlMoveAnim;
    private Timer hlWaveSW;
    private DrawableTexture prevDescTex;
    private DrawableTexture currDescTex;
    private ProgressAnimation progressAnim;
    #endregion

    public TitleScreen(GraphicsDevice graphicsDevice)
    {
        // Data
        Menu = new List<(string Option, string Description, Action Callback)>
        {
            (TitleScreenStrings.Marathon, TitleScreenStrings.MarathonDescription, () => 
            {
                App.QueuedState = new Marathon(GraphicsDevice); 
            }),
            (TitleScreenStrings.Sprint, TitleScreenStrings.SprintDescription, () =>
            {
                App.QueuedState = new Sprint(GraphicsDevice);
            }),
            (TitleScreenStrings.Ultra, TitleScreenStrings.UltraDescription, () =>
            {
                App.QueuedState = new Ultra(GraphicsDevice);
            }),
            (TitleScreenStrings.Options, TitleScreenStrings.OptionsDescription, () =>
            {
                App.QueuedState = new OptionsMenu(GraphicsDevice);
                App.OutTransition = new FadeOutTransition(GraphicsDevice, 300.0, App.TotalGameTime);
                App.InTransition = new FadeInTransition(GraphicsDevice, 300.0, App.TotalGameTime);
            }),
            (TitleScreenStrings.Exit, TitleScreenStrings.ExitDescription, () =>
            {
                App.PopStack = 1;
            })
        };

        VersionNumber = GetVersionNumber();

        // Rendering
        WindowSize = new Vector2(Dimensions.DefaultWindowWidth, Dimensions.DefaultWindowHeight);
        GraphicsDevice = graphicsDevice;
        Font = LogManager.Load<SpriteFont>(Resource.FT01, App.contentManager);
        MenuPointer = LogManager.Load<Texture2D>(Resource.MP01, App.contentManager);
        Background = LogManager.Load<Texture2D>(Resource.BG02, App.contentManager);
        Texture = new Texture2D(GraphicsDevice, 1, 1);
        Texture.SetData(new[] { Color.White });
        OptionsTextures = new List<DrawableTexture>();
        DescriptionTextures = new List<DrawableTexture>();
        foreach (var menuItem in Menu)
        {
            OptionsTextures.Add(DrawOptionToDrawable(GraphicsDevice, menuItem.Option));
            DescriptionTextures.Add(DrawDescriptionToDrawable(GraphicsDevice, menuItem.Description));
        }
        prevDescTex = DescriptionTextures[0];
        currDescTex = DescriptionTextures[0];
        progressAnim = new ProgressAnimation(1.0, 0.0);

        currentGT = new GameTime().TotalGameTime.TotalMilliseconds;
        hlWaveSW = new Timer(currentGT);

        var p = GetMenuPosition(0);
        var x = p.X - MenuPointer.Width - 6;
        hlMoveAnim = new MovementAnimation(x, x, p.Y, p.Y, PointerMoveTime, currentGT);

        AddKeys(new[]
        {
            Keys.Enter, Keys.Up, Keys.Down
        });
    }

    private string GetVersionNumber() => System.Diagnostics.FileVersionInfo.GetVersionInfo(".\\BinksFarm.exe").FileVersion;

    private Vector2 GetMenuPosition(int index)
    {
        var texture = OptionsTextures[index];
        return new Vector2(WindowSize.X - texture.Width - 25, index * texture.Height + 25);
    }

    private DrawableTexture DrawOptionToDrawable(GraphicsDevice gd, string opt)
    {
        var dim = Font.MeasureStringScaled(opt, FontScale);
        return new DrawableTexture(gd, (int)dim.X, (int)dim.Y, (sb) =>
        {
            sb.DrawStringOffset(Font, opt, Color.White, new Vector2(1, 1), scale: FontScale);
        });
    }

    private DrawableTexture DrawDescriptionToDrawable(GraphicsDevice gd, string desc)
    {
        var dim = Font.MeasureStringScaled(desc, 1F);
        return new DrawableTexture(gd, Dimensions.DefaultWindowWidth, (int)dim.Y + 10, (sb) =>
        {
            sb.DrawStringOffset(Font, desc, Color.White, new Vector2(Dimensions.DefaultWindowWidth / 2 - dim.X / 2, 5));
        });
    }

    private void DrawDescription(SpriteBatch sb)
    {
        var f = EaseInOutFunctions.EaseOutQuint;
        var rect = new Rectangle(0, (int)(WindowSize.Y - currDescTex.Height), (int)WindowSize.X, currDescTex.Height + 1);
        sb.Draw(Texture, rect.Scale(App.Scale), Color.Black * 0.5F);
        var percent = progressAnim.GetPercent(currentGT);
        rect = new Rectangle((int)(-prevDescTex.Width * f(percent)), (int)WindowSize.Y - prevDescTex.Height, 
            prevDescTex.Width, prevDescTex.Height);
        sb.Draw(prevDescTex, rect.Scale(App.Scale), Color.White);
        rect = new Rectangle(currDescTex.Width - (int)(currDescTex.Width * f(percent)), (int)WindowSize.Y - currDescTex.Height,
            currDescTex.Width, currDescTex.Height);
        sb.Draw(currDescTex, rect.Scale(App.Scale), Color.White);
    }

    private void DrawVersionInfo(SpriteBatch sb)
    {
        var str1 = TitleScreenStrings.GameTitle;
        var str2 = TitleScreenStrings.URL;
        var str3 = $"Version #{VersionNumber}";

        var dim1 = Font.MeasureStringScaled(str1, 0.5F);
        var dim2 = Font.MeasureStringScaled(str2, 0.5F);

        sb.DrawStringOffset(Font, str1, Color.White, new Vector2(10F, 3F).Scale(App.Scale), scale: 0.5F.Scale(App.Scale));
        sb.DrawStringOffset(Font, str2, Color.White, new Vector2(10F, 6F + dim1.Y).Scale(App.Scale), scale: 0.5F.Scale(App.Scale));
        sb.DrawStringOffset(Font, str3, Color.White, new Vector2(10F, 9F + dim1.Y + dim2.Y).Scale(App.Scale), scale: 0.5F.Scale(App.Scale));
    }

    private void DrawMenuPointer(SpriteBatch spriteBatch)
    {
        var x = hlMoveAnim.GetX(currentGT);
        var y = hlMoveAnim.GetY(currentGT);
        var xo = 5.0 * Math.Sin(hlWaveSW.GetElapsedMilliseconds(currentGT) * Math.PI / PointerWaveTime);
        var rect = new Rectangle((int)(x + xo), (int)y, MenuPointer.Width, MenuPointer.Height);
        spriteBatch.Draw(MenuPointer, rect.Scale(App.Scale), Color.White);
    }

    public override void Draw(SpriteBatch sb)
    {
        var scale = (double)App.GameConfig.ResolutionX / (double)Dimensions.DefaultWindowWidth;

        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

        sb.Draw(Background, destinationRectangle: GraphicsDevice.PresentationParameters.Bounds, 
            sourceRectangle: new Rectangle(0, 700, Background.Width, (int)(Background.Width * WindowSize.Y / WindowSize.X)), Color.White);

        for (int i = 0; i < OptionsTextures.Count; i++)
        {
            var tex = OptionsTextures[i];
            var position = GetMenuPosition(i);
            var rect = new Rectangle((int)position.X, (int)position.Y, tex.Width, tex.Height);
            sb.Draw(tex, rect.Scale(App.Scale), Color.White);
        }

        DrawDescription(sb);
        DrawVersionInfo(sb);
        DrawMenuPointer(sb);

        sb.End();
    }

    public override ChangeState GetChangeState() => ChangeState.NoChange;

    public override GameState GetGameState() => GameState.MainMenu;

    public override void OnResume()
    {
        var scale = (double)App.GameConfig.ResolutionX / (double)Dimensions.DefaultWindowWidth;
        var p = GetMenuPosition(hlp);
        var x = p.X - MenuPointer.Width - 6;
        hlMoveAnim = new MovementAnimation(x, x, p.Y, p.Y, PointerMoveTime, currentGT);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        currentGT += gameTime.ElapsedGameTime.TotalMilliseconds;

        int afterPosition = hlp;
        if (KeyFramesHeld(Keys.Down, f => f == 1 || f % 10 == 0))
        {
            afterPosition = (hlp + 1).ClampLoop(0, Menu.Count - 1);
        }
        else if (KeyFramesHeld(Keys.Up, f => f == 1 || f % 10 == 0))
        {
            afterPosition = (hlp - 1).ClampLoop(0, Menu.Count - 1);
        }
        else if (GetFramesHeld(Keys.Enter) == 1)
        {
            Menu[hlp].Callback();
        }

        if (hlp != afterPosition)
        {
            var p = GetMenuPosition(afterPosition);
            hlMoveAnim.FromX = hlMoveAnim.ToX;
            hlMoveAnim.ToX = p.X - MenuPointer.Width - 6;
            hlMoveAnim.FromY = hlMoveAnim.ToY;
            hlMoveAnim.ToY = p.Y;
            hlMoveAnim.Restart(currentGT);
            hlp = afterPosition;

            prevDescTex = currDescTex;
            currDescTex = DescriptionTextures[hlp];
            progressAnim = new ProgressAnimation(750.0, currentGT);
        }
    }
}