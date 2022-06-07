using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoCustoms;
using Monomino.Classes;
using Monomino.Constants;
using Monomino.Enums;
using System;
using System.Collections.Generic;

namespace Monomino.States;

public class OptionsMenu : UserInputState
{
    #region Menu Options
    private readonly string[] GameOptionsMenu;
    private readonly string[] KeyBindingsMenu;
    private readonly List<int> ResolutionsX;
    private readonly List<int> ResolutionsY;
    #endregion

    #region Menu Pointer
    private const double PointerMoveTime = 100.0;
    private const double PointerWaveTime = 1000.0;
    private int pointerPos = 0;
    private MovementAnimation pointerAnim;
    private Timer pointerWaveSW;
    #endregion

    #region Text Scales
    private const float HeaderScale = 1.5F;
    private const float SettingScale = 1;
    #endregion

    #region Rendering
    private readonly SpriteFont Font;
    private readonly Texture2D MenuPointer;
    private readonly Rectangle WindowSize;
    private readonly List<Keys> BindableKeys = new List<Keys>();
    private readonly DrawableTexture GameOptionsTexture;
    private readonly DrawableTexture KeyBindingsTexture;
    private readonly Texture2D Background;
    private readonly GraphicsDevice @GraphicsDevice;
    #endregion

    #region Animation
    private double currentGT;
    #endregion

    #region Game Logic
    private bool ListeningForKeyPress = false;
    private readonly Texture2D Overlay;
    #endregion

    private const double ScrollDuration = 1700.0;
    private ProgressAnimation progressAnim;
    private readonly double TopGap;
    private double beforeYO = 0.0, afterYO = 0.0;

    public OptionsMenu(GraphicsDevice graphicsDevice)
    {
        WindowSize = graphicsDevice.PresentationParameters.Bounds;

        GameOptionsMenu = new string[]
        {
            OptionsMenuStrings.MovementProperty,
            OptionsMenuStrings.RotationProperty,
            OptionsMenuStrings.DASProperty,
            OptionsMenuStrings.ARRProperty,
            OptionsMenuStrings.ResolutionProperty,
            OptionsMenuStrings.FullscreenProperty,
        };
        KeyBindingsMenu = new string[]
        {
            OptionsMenuStrings.PresetProperty,
            OptionsMenuStrings.LeftBinding,
            OptionsMenuStrings.RightBinding,
            OptionsMenuStrings.DownBinding,
            OptionsMenuStrings.HardDropBinding,
            OptionsMenuStrings.HoldBinding,
            OptionsMenuStrings.RotateClockwiseBinding,
            OptionsMenuStrings.RotateCounterClockwiseBinding
        };
        ResolutionsX = new List<int>
        {
            426, 640, 854, 1280, 1920, 2560, 3840
        };
        ResolutionsY = new List<int>
        {
            240, 360, 480, 720, 1080, 1440, 2160
        };

        Font = LogManager.Load<SpriteFont>(Resource.FT01, App.contentManager);
        MenuPointer = LogManager.Load<Texture2D>(Resource.MP01, App.contentManager);
        Background = LogManager.Load<Texture2D>(Resource.BG03, App.contentManager);

        GameOptionsTexture = CreateMenuTexture(GameOptionsMenu, OptionsMenuStrings.GameOptionsHeader, graphicsDevice);
        KeyBindingsTexture = CreateMenuTexture(KeyBindingsMenu, OptionsMenuStrings.KeyBindingsHeader, graphicsDevice);
        GraphicsDevice = graphicsDevice;

        Overlay = new Texture2D(graphicsDevice, 1, 1);
        Overlay.SetData(new Color[] { Color.Black * 0.7F });

        currentGT = new GameTime().TotalGameTime.TotalMilliseconds;
        //pointerMoveSW = new Timer(currentGT);
        pointerWaveSW = new Timer(currentGT);

        for (int i = (int)Keys.A; i <= (int)Keys.Z; i++)
        {
            BindableKeys.Add((Keys)i);
        }
        BindableKeys.AddRange(new Keys[] 
        { 
            Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Space, Keys.LeftShift, Keys.RightShift, Keys.Enter,
            Keys.OemComma, Keys.OemPeriod, Keys.OemSemicolon, Keys.OemQuotes, Keys.OemQuestion, Keys.LeftShift,
            Keys.RightShift, Keys.CapsLock, Keys.Tab, Keys.OemTilde
        });

        AddKeys(BindableKeys.ToArray());

        var pointerRPos = GetCursorPosition(pointerPos);
        pointerAnim = new MovementAnimation(pointerRPos.X,
            pointerRPos.X,
            pointerRPos.Y,
            pointerRPos.Y,
            PointerMoveTime, 
            currentGT);

        // carry over enter key
        SetFramesHeld(Keys.Enter, 1);

        progressAnim = new ProgressAnimation(ScrollDuration, currentGT);
        TopGap = Font.MeasureStringScaled("Game Options", HeaderScale.Scale(App.Scale)).Y + 35.Scale(App.Scale);
    }

    private Vector2 GetCursorPosition(int index)
    {
        float X = 15.0F.Scale(App.Scale);
        float Y = 0F;

        if (index < GameOptionsMenu.Length)
        {
            var by = Font.MeasureStringScaled(OptionsMenuStrings.GameOptionsHeader, HeaderScale).Y + 35;
            Y = (by + Font.MeasureStringScaled(GameOptionsMenu[index], SettingScale).Y * index).Scale(App.Scale);
        }
        else if (index < GameOptionsMenu.Length + KeyBindingsMenu.Length)
        {
            var by = GameOptionsTexture.Height + 25 + Font.MeasureStringScaled(OptionsMenuStrings.KeyBindingsHeader, HeaderScale).Y + 35;
            Y = by + Font.MeasureStringScaled(KeyBindingsMenu[index - GameOptionsMenu.Length], SettingScale).Y * (index - GameOptionsMenu.Length);
            Y = Y.Scale(App.Scale);
        }
        else
        {
            X = WindowSize.Width / 2 - MenuPointer.Width.Scale(App.Scale) - 
                Font.MeasureStringScaled("Save Settings", 1F.Scale(App.Scale)).X / 2 - 8.0F.Scale(App.Scale);
            Y = (GameOptionsTexture.Height + 25 + KeyBindingsTexture.Height + 25).Scale(App.Scale);
        }

        return new Vector2(X, Y);
    }

    private double GetGameOptionsTextureY() => 0.0;

    private double GetKeyBindingsOptionsTextureY() => GameOptionsTexture.Height.Scale(App.Scale) + 25.0.Scale(App.Scale);

    private double GetSaveSettingsY() => (GameOptionsTexture.Height + KeyBindingsTexture.Height + 50.0).Scale(App.Scale);

    private double GetTotalHeight()
    {
        double x = 0;
        // Game Options
        x += GameOptionsTexture.Height;
        // Gap between Game Options and KeyBindings
        x += 25;
        // KeyBindings
        x += KeyBindingsTexture.Height;
        // Gap between KeyBindings and Save Settings
        x += 25;
        // Save Settings
        x += Font.MeasureStringScaled("Save Settings", 1F).Y;
        // End gap
        x += 10;

        return x;
    }

    private DrawableTexture CreateMenuTexture(string[] menu, string header, GraphicsDevice graphicsDevice)
    {
        int h = (int)Font.MeasureStringScaled(header, HeaderScale).Y + 35;
        for (int i = 0; i < menu.Length; i++)
        {
            h += (int)Font.MeasureStringScaled(menu[i], SettingScale).Y;
        }
        return  new DrawableTexture(graphicsDevice, Dimensions.DefaultWindowWidth, h, (sb) =>
        {
            int y = 0;
            var headerDim = Font.MeasureStringScaled(header, HeaderScale);
            sb.DrawStringOffset(Font, header, Color.White, new Vector2(Dimensions.DefaultWindowWidth / 2 - headerDim.X / 2, 0), 
                scale: HeaderScale);
            y += 35 + (int)headerDim.Y;
            for (int i = 0; i < menu.Length; i++)
            {
                var dim = Font.MeasureStringScaled(menu[i], SettingScale);
                sb.DrawStringOffset(Font, menu[i], Color.White, new Vector2(50, y), scale: SettingScale);
                y += (int)dim.Y;
            }
        });
    }

    private double GetYO()
    {
        var percent = progressAnim.GetPercent(currentGT);
        return beforeYO + ((afterYO - beforeYO) * EaseInOutFunctions.EaseOutQuint(percent));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);

        // Adjust YO
        var pos = GetCursorPosition(pointerPos);
        if (pos.Y - afterYO < TopGap)
        {
            beforeYO = GetYO();
            afterYO = Math.Max(0.0, pos.Y - TopGap);
            progressAnim = new ProgressAnimation(ScrollDuration, currentGT);
        }
        else if (pos.Y - afterYO > WindowSize.Height - MenuPointer.Height.Scale(App.Scale) - 25.Scale(App.Scale))
        {
            beforeYO = GetYO();
            afterYO = pos.Y + 25.Scale(App.Scale) + MenuPointer.Height.Scale(App.Scale) - WindowSize.Height;
            progressAnim = new ProgressAnimation(ScrollDuration, currentGT);
        }

        double YO = GetYO();

        spriteBatch.Draw(Background, WindowSize, 
            new Rectangle(0, 0, Background.Width, Background.Width * WindowSize.Height / WindowSize.Width), Color.White);
        spriteBatch.Draw(GameOptionsTexture, 
            new Rectangle(0, 
                (int)(GetGameOptionsTextureY() - YO), 
                GameOptionsTexture.Width.Scale(App.Scale), 
                GameOptionsTexture.Height.Scale(App.Scale)), 
            Color.White);
        spriteBatch.Draw(KeyBindingsTexture, 
            new Rectangle(0, 
            (int)(GetKeyBindingsOptionsTextureY() - YO), 
            KeyBindingsTexture.Width.Scale(App.Scale), 
            KeyBindingsTexture.Height.Scale(App.Scale)), 
            Color.White);

        DrawValues(spriteBatch);

        {
            var dim = Font.MeasureStringScaled("Save Settings", 1F.Scale(App.Scale));
            spriteBatch.DrawStringOffset(Font, "Save Settings", Color.White, 
                new Vector2(WindowSize.Width / 2 - dim.X / 2, (int)(GetSaveSettingsY() - YO)), scale: 1F.Scale(App.Scale));
        }

        {
            var x = pointerAnim.GetX(currentGT);
            var y = pointerAnim.GetY(currentGT);
            var xo = 5.0.Scale(App.Scale) * Math.Sin(pointerWaveSW.GetElapsedMilliseconds(currentGT) * Math.PI / PointerWaveTime);
            spriteBatch.Draw(MenuPointer, 
                new Rectangle((int)x + (int)xo, 
                (int)(y - YO), 
                MenuPointer.Width.Scale(App.Scale), 
                MenuPointer.Height.Scale(App.Scale)), 
                Color.White);
        }

        if (ListeningForKeyPress)
        {
            spriteBatch.Draw(Overlay, WindowSize, Color.White);
            var dim = Font.MeasureStringScaled("Listening for keypress...", 1.0F.Scale(App.Scale));
            spriteBatch.DrawStringOffset(Font, "Listening for keypress...", Color.White, 
                new Vector2(WindowSize.Width / 2, WindowSize.Height / 2), 
                origin: new Vector2(dim.X / 2, dim.Y / 2), scale: 1F.Scale(App.Scale));
        }

        spriteBatch.End();
    }

    private void DrawValue(SpriteBatch spriteBatch, int index, string value)
    {
        double YO = GetYO();
        var position = GetCursorPosition(index);
        var dim = Font.MeasureStringScaled(value, SettingScale.Scale(App.Scale));
        spriteBatch.DrawStringOffset(Font, value, Color.White, 
            new Vector2(WindowSize.Width - dim.X - 10.Scale(App.Scale), position.Y - (int)YO), 
            scale: SettingScale.Scale(App.Scale));
    }

    private void DrawValues(SpriteBatch spriteBatch)
    {
        // Movement Type
        DrawValue(spriteBatch, 0, App.GameConfig.UsePixelMovement ? "Pixel (Animated)" : "Grid (Not Animated)");
        // Rotation Type
        DrawValue(spriteBatch, 1, App.GameConfig.UsePixelRotation ? "Pixel (Animated)" : "Grid (Not Animated)");
        // DAS
        DrawValue(spriteBatch, 2, App.GameConfig.DAS.ToString());
        // ARR
        DrawValue(spriteBatch, 3, App.GameConfig.ARR.ToString());
        // Resolutions
        DrawValue(spriteBatch, 4, $"{App.GameConfig.ResolutionX}x{App.GameConfig.ResolutionY}");
        // Fullscreen
        DrawValue(spriteBatch, 5, App.GameConfig.IsFullscreen.ToString());
        // Preset
        DrawValue(spriteBatch, 6, App.GameConfig.PresetNumber.ToString());
        // Left
        DrawValue(spriteBatch, 7, App.GameConfig.KeyBindings[BindKeys.LeftMove].ToString());
        // Right
        DrawValue(spriteBatch, 8, App.GameConfig.KeyBindings[BindKeys.RightMove].ToString());
        // Down
        DrawValue(spriteBatch, 9, App.GameConfig.KeyBindings[BindKeys.DownMove].ToString());
        // Hard Drop
        DrawValue(spriteBatch, 10, App.GameConfig.KeyBindings[BindKeys.HardDrop].ToString());
        // Hold
        DrawValue(spriteBatch, 11, App.GameConfig.KeyBindings[BindKeys.Hold].ToString());
        // Rotate Clockwise
        DrawValue(spriteBatch, 12, App.GameConfig.KeyBindings[BindKeys.RotateClockwise].ToString());
        // Rotate Counterclockwise
        DrawValue(spriteBatch, 13, App.GameConfig.KeyBindings[BindKeys.RotateCounterClockwise].ToString());
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        currentGT += gameTime.ElapsedGameTime.TotalMilliseconds;

        if (!ListeningForKeyPress)
        {
            int afterPosition = pointerPos;
            var length = GameOptionsMenu.Length + KeyBindingsMenu.Length + 1;

            if (IfKey(Keys.Down, x => x > 0 && (x == 1 || x % 10 == 0)))
            {
                afterPosition = (pointerPos + 1) % length;
            }
            else if (IfKey(Keys.Up, x=> x > 0 && (x == 1 || x % 10 == 0)))
            {
                afterPosition = afterPosition == 0 ? length - 1 : afterPosition - 1;
            }
            else if (GetFramesHeld(Keys.Enter) == 1)
            {
                switch (pointerPos)
                {
                    // Movement
                    case 0:
                        App.GameConfig.UsePixelMovement = !App.GameConfig.UsePixelMovement;
                        break;
                    // Rotation
                    case 1:
                        App.GameConfig.UsePixelRotation = !App.GameConfig.UsePixelRotation;
                        break;
                    case 2:
                        App.GameConfig.DAS = Math.Max((App.GameConfig.DAS + 1) % 26, 1);
                        break;
                    case 3:
                        App.GameConfig.ARR = Math.Max((App.GameConfig.ARR + 1) % 11, 1);
                        break;
                    case 4:
                        var i = ResolutionsX.IndexOf(App.GameConfig.ResolutionX);
                        i = i != -1 ? (i + 1) % ResolutionsX.Count : 0;
                        App.GameConfig.ResolutionX = ResolutionsX[i];
                        App.GameConfig.ResolutionY = ResolutionsY[i];
                        break;
                    case 5:
                        App.GameConfig.IsFullscreen = !App.GameConfig.IsFullscreen;
                        break;
                    // Preset
                    case 6:
                        {
                            var n = Enum.GetValues(typeof(enumBindingPreset)).Length;
                            App.GameConfig.PresetNumber = (enumBindingPreset)(((int)App.GameConfig.PresetNumber + 1) % n);

                            if (App.GameConfig.PresetNumber < enumBindingPreset.Custom)
                            {
                                App.GameConfig.KeyBindings = KeyBindingPresets.Presets[(int)App.GameConfig.PresetNumber];
                            }
                        }
                        break;
                    case 14:
                        {
                            App.GameConfig.ExportConfiguration();
                            App.UpdateFullscreen = true;
                            App.UpdateResolution = true;
                            App.PopStack = 1;
                            App.OutTransition = new FadeOutTransition(GraphicsDevice, 300.0, gameTime.TotalGameTime.TotalMilliseconds);
                            App.InTransition = new FadeInTransition(GraphicsDevice, 300.0, gameTime.TotalGameTime.TotalMilliseconds);
                        }
                        break;
                    default:
                        ListeningForKeyPress = true;
                        break;
                }
            }

            if (afterPosition != pointerPos)
            {
                var pointerRPos = GetCursorPosition(afterPosition);
                pointerAnim.FromX = pointerAnim.ToX;
                pointerAnim.ToX = pointerRPos.X;
                pointerAnim.FromY = pointerAnim.ToY;
                pointerAnim.ToY = pointerRPos.Y;
                pointerAnim.Restart(currentGT);

                pointerPos = afterPosition;
            }
        }
        else
        {
            foreach (var key in BindableKeys)
            {
                if (GetFramesHeld(key) == 1)
                {
                    switch (pointerPos) 
                    {
                        case 7:
                            App.GameConfig.KeyBindings[BindKeys.LeftMove] = key;
                            break;
                        case 8:
                            App.GameConfig.KeyBindings[BindKeys.RightMove] = key;
                            break;
                        case 9:
                            App.GameConfig.KeyBindings[BindKeys.DownMove] = key;
                            break;
                        case 10:
                            App.GameConfig.KeyBindings[BindKeys.HardDrop] = key;
                            break;
                        case 11:
                            App.GameConfig.KeyBindings[BindKeys.Hold] = key;
                            break;
                        case 12:
                            App.GameConfig.KeyBindings[BindKeys.RotateClockwise] = key;
                            break;
                        case 13:
                            App.GameConfig.KeyBindings[BindKeys.RotateCounterClockwise] = key;
                            break;
                    }
                    App.GameConfig.PresetNumber = enumBindingPreset.Custom;
                    ListeningForKeyPress = false;
                }
            }
        }
    }

    public override ChangeState GetChangeState()
    {
        return ChangeState.NoChange;
    }

    public override GameState GetGameState()
    {
        return GameState.LoadingScreen;
    }
}
