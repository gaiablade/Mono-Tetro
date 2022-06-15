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

public struct Option
{
    public string PropertyName { get; set; }
    public Func<string> Value { get; set; }
    public Action Increment { get; set; }
    public Action Decrement { get; set; }
}

public class OptionsMenu : UserInputState
{
    #region Menu Options
    private List<Option> GameOptionsMenu;
    private List<Option> KeyBindingsMenu;
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

        GameOptionsMenu = new List<Option>();

        GameOptionsMenu.Add(new Option
        {
            PropertyName = OptionsMenuStrings.MovementProperty,
            Value = () => App.GameConfig.UsePixelMovement ? "Pixel (Animated)" : "Grid (Not Animated)",
            Increment = App.GameConfig.TogglePixelMovement,
            Decrement = App.GameConfig.TogglePixelMovement
        });

        GameOptionsMenu.Add(new Option
        {
            PropertyName = OptionsMenuStrings.RotationProperty,
            Value = () => App.GameConfig.UsePixelRotation ? "Pixel (Animated)" : "Grid (Not Animated)",
            Increment = App.GameConfig.TogglePixelRotation,
            Decrement = App.GameConfig.TogglePixelRotation
        });

        GameOptionsMenu.Add(new Option
        {
            PropertyName = OptionsMenuStrings.DASProperty,
            Value = () => App.GameConfig.DAS.ToString(),
            Increment = () => App.GameConfig.DAS = (App.GameConfig.DAS + 1).ClampLoop(1, 15),
            Decrement = () => App.GameConfig.DAS = (App.GameConfig.DAS - 1).ClampLoop(1, 15)
        });

        GameOptionsMenu.Add(new Option
        {
            PropertyName = OptionsMenuStrings.ARRProperty,
            Value = () => App.GameConfig.ARR.ToString(),
            Increment = () => App.GameConfig.ARR = (App.GameConfig.ARR + 1).ClampLoop(1, 15),
            Decrement = () => App.GameConfig.ARR = (App.GameConfig.ARR - 1).ClampLoop(1, 15)
        });

        GameOptionsMenu.Add(new Option
        {
            PropertyName = OptionsMenuStrings.ResolutionProperty,
            Value = () => $"{App.GameConfig.ResolutionX}x{App.GameConfig.ResolutionY}",
            Increment = () =>
            {
                var i = ResolutionsX.IndexOf(App.GameConfig.ResolutionX);
                i = i != -1 ? (i + 1) % ResolutionsX.Count : 0;
                App.GameConfig.ResolutionX = ResolutionsX[i];
                App.GameConfig.ResolutionY = ResolutionsY[i];
            },
            Decrement = () =>
            {
                var i = ResolutionsX.IndexOf(App.GameConfig.ResolutionX);
                i = i != -1 ? (i - 1).ClampLoop(0, ResolutionsX.Count - 1) % ResolutionsX.Count : 0;
                App.GameConfig.ResolutionX = ResolutionsX[i];
                App.GameConfig.ResolutionY = ResolutionsY[i];
            }
        });

        GameOptionsMenu.Add(new Option
        {
            PropertyName = OptionsMenuStrings.FullscreenProperty,
            Value = () => App.GameConfig.IsFullscreen.ToString(),
            Increment = App.GameConfig.ToggleFullscreen,
            Decrement = App.GameConfig.ToggleFullscreen
        });

        GameOptionsMenu.Add(new Option
        {
            PropertyName = OptionsMenuStrings.BackgroundDimProperty,
            Value = () => App.GameConfig.BackgroundDim.ToString() + "%",
            Increment = App.GameConfig.IncrementBackgroundDim,
            Decrement = App.GameConfig.DecrementBackgroundDim
        });

        GameOptionsMenu.Add(new Option
        {
            PropertyName = OptionsMenuStrings.ShowBannersProperty,
            Value = () => App.GameConfig.ShowBanners.ToString(),
            Increment = App.GameConfig.ToggleShowBanners,
            Decrement = App.GameConfig.ToggleShowBanners
        });

        KeyBindingsMenu = new List<Option>();

        KeyBindingsMenu.Add(new Option
        {
            PropertyName = OptionsMenuStrings.PresetProperty,
            Value = () => App.GameConfig.PresetNumber.ToString(),
            Increment = () =>
            {
                var n = Enum.GetValues(typeof(enumBindingPreset)).Length;
                App.GameConfig.PresetNumber = (enumBindingPreset)(((int)App.GameConfig.PresetNumber + 1) % n);

                if (App.GameConfig.PresetNumber < enumBindingPreset.Custom)
                {
                    App.GameConfig.KeyBindings = KeyBindingPresets.Presets[(int)App.GameConfig.PresetNumber];
                }
            },
            Decrement = () => 
            {
                var n = Enum.GetValues(typeof(enumBindingPreset)).Length;
                App.GameConfig.PresetNumber = (enumBindingPreset)(((int)App.GameConfig.PresetNumber - 1).ClampLoop(0, n));

                if (App.GameConfig.PresetNumber < enumBindingPreset.Custom)
                {
                    App.GameConfig.KeyBindings = KeyBindingPresets.Presets[(int)App.GameConfig.PresetNumber];
                }
            }
        });

        KeyBindingsMenu.Add(new Option
        {
            PropertyName = OptionsMenuStrings.LeftBinding,
            Value = () => App.GameConfig.KeyBindings[BindKeys.LeftMove].ToString(),
            Increment = () => ListeningForKeyPress = true,
            Decrement = () => ListeningForKeyPress = true
        });

        KeyBindingsMenu.Add(new Option
        {
            PropertyName = OptionsMenuStrings.RightBinding,
            Value = () => App.GameConfig.KeyBindings[BindKeys.RightMove].ToString(),
            Increment = () => ListeningForKeyPress = true,
            Decrement = () => ListeningForKeyPress = true
        });

        KeyBindingsMenu.Add(new Option
        {
            PropertyName = OptionsMenuStrings.DownBinding,
            Value = () => App.GameConfig.KeyBindings[BindKeys.DownMove].ToString(),
            Increment = () => ListeningForKeyPress = true,
            Decrement = () => ListeningForKeyPress = true
        });

        KeyBindingsMenu.Add(new Option
        {
            PropertyName = OptionsMenuStrings.HardDropBinding,
            Value = () => App.GameConfig.KeyBindings[BindKeys.HardDrop].ToString(),
            Increment = () => ListeningForKeyPress = true,
            Decrement = () => ListeningForKeyPress = true
        });

        KeyBindingsMenu.Add(new Option
        {
            PropertyName = OptionsMenuStrings.HoldBinding,
            Value = () => App.GameConfig.KeyBindings[BindKeys.Hold].ToString(),
            Increment = () => ListeningForKeyPress = true,
            Decrement = () => ListeningForKeyPress = true
        });

        KeyBindingsMenu.Add(new Option
        {
            PropertyName = OptionsMenuStrings.RotateClockwiseBinding,
            Value = () => App.GameConfig.KeyBindings[BindKeys.RotateClockwise].ToString(),
            Increment = () => ListeningForKeyPress = true,
            Decrement = () => ListeningForKeyPress = true
        });

        KeyBindingsMenu.Add(new Option
        {
            PropertyName = OptionsMenuStrings.RotateCounterClockwiseBinding,
            Value = () => App.GameConfig.KeyBindings[BindKeys.RotateCounterClockwise].ToString(),
            Increment = () => ListeningForKeyPress = true,
            Decrement = () => ListeningForKeyPress = true
        });

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

        if (index < GameOptionsMenu.Count)
        {
            var by = Font.MeasureStringScaled(OptionsMenuStrings.GameOptionsHeader, HeaderScale).Y + 35;
            Y = (by + Font.MeasureStringScaled(GameOptionsMenu[index].PropertyName, SettingScale).Y * index).Scale(App.Scale);
        }
        else if (index < GameOptionsMenu.Count + KeyBindingsMenu.Count)
        {
            var by = GameOptionsTexture.Height + 25 + Font.MeasureStringScaled(OptionsMenuStrings.KeyBindingsHeader, HeaderScale).Y + 35;
            Y = by + Font.MeasureStringScaled(KeyBindingsMenu[index - GameOptionsMenu.Count].PropertyName, SettingScale).Y * (index - GameOptionsMenu.Count);
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

    private DrawableTexture CreateMenuTexture(List<Option> menu, string header, GraphicsDevice graphicsDevice)
    {
        int h = (int)Font.MeasureStringScaled(header, HeaderScale).Y + 35;
        for (int i = 0; i < menu.Count; i++)
        {
            h += (int)Font.MeasureStringScaled(menu[i].PropertyName, SettingScale).Y;
        }
        return new DrawableTexture(graphicsDevice, Dimensions.DefaultWindowWidth, h, (sb) =>
        {
            int y = 0;
            var headerDim = Font.MeasureStringScaled(header, HeaderScale);
            sb.DrawStringOffset(Font, header, Color.White, new Vector2(Dimensions.DefaultWindowWidth / 2 - headerDim.X / 2, 0),
                scale: HeaderScale);
            y += 35 + (int)headerDim.Y;
            for (int i = 0; i < menu.Count; i++)
            {
                var dim = Font.MeasureStringScaled(menu[i].PropertyName, SettingScale);
                sb.DrawStringOffset(Font, menu[i].PropertyName, Color.White, new Vector2(50, y), scale: SettingScale);
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
        LogManager.Debug("Beginning of Draw");
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
        LogManager.Debug("End of Draw");
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
        LogManager.Debug("Beginning of DrawValues");
        for (int i = 0; i < (GameOptionsMenu.Count + KeyBindingsMenu.Count); i++)
        {
            if (i < GameOptionsMenu.Count)
                DrawValue(spriteBatch, i, GameOptionsMenu[i].Value());
            else
                DrawValue(spriteBatch, i, KeyBindingsMenu[i - GameOptionsMenu.Count].Value());
        }
        LogManager.Debug("End of DrawValues");
    }

    public override void Update(GameTime gameTime)
    {
        LogManager.Debug("Beginning of Update");
        base.Update(gameTime);
        currentGT += gameTime.ElapsedGameTime.TotalMilliseconds;

        if (!ListeningForKeyPress)
        {
            int afterPosition = pointerPos;
            var length = GameOptionsMenu.Count + KeyBindingsMenu.Count + 1;

            if (IfKey(Keys.Down, x => x > 0 && (x == 1 || x % 10 == 0)))
            {
                afterPosition = (pointerPos + 1) % length;
            }
            else if (IfKey(Keys.Up, x => x > 0 && (x == 1 || x % 10 == 0)))
            {
                afterPosition = afterPosition == 0 ? length - 1 : afterPosition - 1;
            }
            else if (GetFramesHeld(Keys.Enter) == 1 || GetFramesHeld(Keys.Right) == 1)
            {
                if (pointerPos < GameOptionsMenu.Count)
                {
                    GameOptionsMenu[pointerPos].Increment();
                }
                else if (pointerPos < GameOptionsMenu.Count + KeyBindingsMenu.Count)
                {
                    KeyBindingsMenu[pointerPos - GameOptionsMenu.Count].Increment();
                }
                else
                {
                    App.GameConfig.ExportConfiguration();
                    App.UpdateFullscreen = true;
                    App.UpdateResolution = true;
                    App.PopStack = 1;
                    App.OutTransition = new FadeOutTransition(GraphicsDevice, 300.0, App.TotalGameTime);
                    App.InTransition = new FadeInTransition(GraphicsDevice, 300.0, App.TotalGameTime);
                }
            }
            else if (GetFramesHeld(Keys.Left) == 1)
            {
                if (pointerPos < GameOptionsMenu.Count)
                {
                    GameOptionsMenu[pointerPos].Decrement();
                }
                else if (pointerPos < GameOptionsMenu.Count + KeyBindingsMenu.Count)
                {
                    KeyBindingsMenu[pointerPos - GameOptionsMenu.Count].Decrement();
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
                        case 9:
                            App.GameConfig.KeyBindings[BindKeys.LeftMove] = key;
                            break;
                        case 10:
                            App.GameConfig.KeyBindings[BindKeys.RightMove] = key;
                            break;
                        case 11:
                            App.GameConfig.KeyBindings[BindKeys.DownMove] = key;
                            break;
                        case 12:
                            App.GameConfig.KeyBindings[BindKeys.HardDrop] = key;
                            break;
                        case 13:
                            App.GameConfig.KeyBindings[BindKeys.Hold] = key;
                            break;
                        case 14:
                            App.GameConfig.KeyBindings[BindKeys.RotateClockwise] = key;
                            break;
                        case 15:
                            App.GameConfig.KeyBindings[BindKeys.RotateCounterClockwise] = key;
                            break;
                    }
                    App.GameConfig.PresetNumber = enumBindingPreset.Custom;
                    ListeningForKeyPress = false;
                }
            }
        }
        LogManager.Debug("End of Update");
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
