﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoCustoms;
using Monomino.Classes;
using Monomino.Constants;
using Monomino.Enums;
using System;

namespace Monomino.States;

public struct SinglePlayerGameConfig
{
    public Configuration GameConfig { get; set; }
    public GraphicsDevice @GraphicsDevice { get; set; }
    public string BackgroundTextureFilename { get; set; }
    public string FontFilename { get; set; }
    public string MenuPointerTextureFilename { get; set; }
}

public abstract class SinglePlayerGameMode : UserInputState
{
    #region Settings Fields
    protected readonly bool UsePixelMovement = App.GameConfig.UsePixelMovement;
    protected readonly bool UsePixelRotation = App.GameConfig.UsePixelRotation;
    protected readonly int ARR = Math.Max(App.GameConfig.ARR, 1);
    protected readonly int DAS = Math.Max(App.GameConfig.DAS, 1);
    protected readonly int TileWidth;
    protected readonly int TileHeight;
    #endregion

    #region KeyBinding Fields
    protected readonly Keys LeftMove;
    protected readonly Keys RightMove;
    protected readonly Keys DownMove;
    protected readonly Keys HardDropKey;
    protected readonly Keys ClockwiseRotate;
    protected readonly Keys CounterClockwiseRotate;
    protected readonly Keys Hold;
    protected readonly Keys Pause;
    #endregion

    #region Constants
    // Constants
    protected readonly int MinimumBagCapacity = 3;
    #endregion

    #region Game Logic Fields
    // Game Logic
    protected readonly Field Field;
    protected readonly PieceBag Bag;
    protected double dropTime;
    protected double softDropTime;
    protected int level = 0;
    protected int linesCleared = 0;
    protected Tetromino currentMino;
    protected TetrominoType? holdPiece = null;
    protected bool holdPieceUsed = false;
    protected bool isPaused = false;
    protected int score = 0;
    protected string bannerMsg = string.Empty;
    protected bool isGameFailed = false;
    #endregion

    #region Rendering Fields
    // Rendering
    protected readonly GraphicsDevice @GraphicsDevice;
    protected readonly Rectangle WindowSize;
    protected readonly Texture2D Background;
    protected readonly SpriteFont Font;
    protected readonly Texture2D KirbyTexture;
    protected readonly int GridXO, GridYO;
    protected readonly int BorderXO, BorderYO;
    protected readonly Texture2D texture;
    protected readonly DrawableTexture Border;
    protected readonly DrawableTexture Grid;
    protected DrawableTexture CurrentMinoTexture;
    #endregion

    #region Animation Fields
    // animation related:
    protected double gameTime = 0;
    protected readonly double HMoveTime;
    protected const double RotateTime = 100.0;
    protected const double LockTime = 2000;
    protected const double BannerTime = 2000;
    protected const double KirbyFrameDuration = 100.0;
    protected float fromRT = 0;
    protected float toRT = 0;
    protected MovementAnimation hMoveAnim;
    protected MovementAnimation vMoveAnim;
    protected MovementAnimation rMoveAnim;
    protected Timer lockSW = null;
    protected Timer bannerSW = null;
    protected Timer kirbySW = null;
    protected Timer pauseSW = null;
    #endregion

    #region Pause Screen Fields
    // Pause Screen
    protected readonly string[] PauseMenu;
    protected readonly Texture2D MenuPointer;
    protected readonly double PointerWaveTime = 1000.0;
    protected readonly double PointerMoveTime = 100.0;
    protected int pointerPos = 0;
    protected double pauseTime = 0;
    protected MovementAnimation pointerAnim;
    protected Timer pointerWaveSW;
    #endregion

    protected SinglePlayerGameMode(SinglePlayerGameConfig config)
    {
        var gameConfig = config.GameConfig;
        this.UsePixelMovement = gameConfig.UsePixelMovement;
        this.UsePixelRotation = gameConfig.UsePixelRotation;
        this.LeftMove = gameConfig.KeyBindings[BindKeys.LeftMove];
        this.RightMove = gameConfig.KeyBindings[BindKeys.RightMove];
        this.DownMove = gameConfig.KeyBindings[BindKeys.DownMove];
        this.HardDropKey = gameConfig.KeyBindings[BindKeys.HardDrop];
        this.ClockwiseRotate = gameConfig.KeyBindings[BindKeys.RotateClockwise];
        this.CounterClockwiseRotate = gameConfig.KeyBindings[BindKeys.RotateCounterClockwise];
        this.Hold = gameConfig.KeyBindings[BindKeys.Hold];
        this.Pause = Keys.Escape;

        TileWidth = Dimensions.TileWidth.Scale(App.Scale);
        TileHeight = Dimensions.TileHeight.Scale(App.Scale);

        // Initialize key listener
        AddKeys(new Keys[]
        {
            ClockwiseRotate, CounterClockwiseRotate, Hold, LeftMove, DownMove,
            HardDropKey, RightMove, Pause
        });

        // Game Logic
        Bag = new PieceBag(MinimumBagCapacity);
        currentMino = new Tetromino(Bag.NextPiece);
        Field = new Field();
        dropTime = CalculateGravity();
        softDropTime = 1000.0 / 20.0;

        // Rendering
        this.GraphicsDevice = config.GraphicsDevice;
        WindowSize = GraphicsDevice.PresentationParameters.Bounds;
        Background = LogManager.Load<Texture2D>(config.BackgroundTextureFilename, App.contentManager);
        Font = LogManager.Load<SpriteFont>(config.FontFilename, App.contentManager);
        texture = new Texture2D(this.GraphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        CurrentMinoTexture = Tetromino.TetrominoTextures[currentMino.Type];
        KirbyTexture = LogManager.Load<Texture2D>(Resource.KIRBY, App.contentManager);
        Grid = DrawGridToDrawable(GraphicsDevice);
        Border = DrawBorderToDrawable(GraphicsDevice);
        GridXO = WindowSize.Width / 2 - Grid.Width.Scale(App.Scale) / 2;
        GridYO = WindowSize.Height / 2 - Grid.Height.Scale(App.Scale) / 2;
        BorderXO = GridXO - 5.Scale(App.Scale);
        BorderYO = WindowSize.Height / 2 - Border.Height.Scale(App.Scale) / 2;

        // Animation
        HMoveTime = (1000.0 / 60.0) * (double)ARR; // in milliseconds
        hMoveAnim = new MovementAnimation(currentMino.X * Dimensions.TileWidth,
            currentMino.X * Dimensions.TileWidth,
            0, 0, HMoveTime, gameTime);
        vMoveAnim = new MovementAnimation(0, 0,
            (currentMino.Y - 1) * Dimensions.TileHeight,
            currentMino.Y * Dimensions.TileHeight,
            dropTime, gameTime);
        rMoveAnim = new MovementAnimation(0, 0, 0, 0, RotateTime, gameTime);
        var gt = new GameTime();
        gameTime = gt.TotalGameTime.TotalMilliseconds;
        lockSW = new Timer(gameTime);
        bannerSW = new Timer(gameTime);
        kirbySW = new Timer(gameTime);

        // Pause Screen
        PauseMenu = new string[]
        {
            "Resume",
            "Exit To Title Screen",
            "Exit To Desktop"
        };
        MenuPointer = LogManager.Load<Texture2D>(config.MenuPointerTextureFilename, App.contentManager);
        AddKeys(new Keys[] { Keys.Enter, Keys.Down, Keys.Up });
        var pointerRPos = GetMenuItemPosition(pointerPos);
        pointerAnim = new MovementAnimation(pointerRPos.X - MenuPointer.Width.Scale(App.Scale),
            pointerRPos.X - MenuPointer.Width.Scale(App.Scale),
            pointerRPos.Y, pointerRPos.Y, PointerMoveTime, gameTime);
        pointerWaveSW = new Timer(pauseTime);
    }

    #region Abstract Methods
    protected abstract bool IsGameFinished();

    protected abstract bool IsGameFailed();

    protected abstract void DrawStats(SpriteBatch spriteBatch);
    #endregion

    #region Draw Functions
    protected DrawableTexture DrawBorderToDrawable(GraphicsDevice graphicsDevice)
    {
        return new DrawableTexture(graphicsDevice, Dimensions.TileWidth * Field.Width + 10, Dimensions.TileHeight * Field.Height + 10, (sb) =>
        {
            int borderWidth = Field.Width * Dimensions.TileWidth;
            int borderHeight = Field.Height * Dimensions.TileHeight;

            sb.Draw(texture, new Rectangle(0, 0, borderWidth + 10, 5), Color.White);
            sb.Draw(texture, new Rectangle(0, 0, 5, borderHeight + 10), Color.White);
            sb.Draw(texture, new Rectangle(0, borderHeight + 5, borderWidth + 10, 5), Color.White);
            sb.Draw(texture, new Rectangle(5 + borderWidth, 0, 5, borderHeight + 10), Color.White);
        });
    }

    protected DrawableTexture DrawGridToDrawable(GraphicsDevice graphicsDevice)
    {
        return new DrawableTexture(graphicsDevice, Dimensions.TileWidth * Field.Width, Dimensions.TileHeight * Field.Height, (sb) =>
        {
            for (int x = 1; x < Field.Width; x++)
            {
                sb.Draw(texture, 
                    new Rectangle(x * Dimensions.TileWidth, 0, 2, Dimensions.TileHeight * Field.Height), 
                    Color.White * 0.5F);
            }
            for (int y = 1; y < Field.Height; y++)
            {
                sb.Draw(texture, 
                    new Rectangle(0, y * Dimensions.TileHeight, Dimensions.TileWidth * Field.Width, 2), 
                    Color.White * 0.5F);
            }
        });
    }

    protected virtual void DrawCurrentPiece(SpriteBatch spriteBatch)
    {
        DrawGhostPiece(spriteBatch);

        var color = Tetromino.Layouts[currentMino.Type].color;
        var tileColor = Tetromino.Colors[color];
        double x = 0.0, y = 0.0, rotation = 0.0;

        if (UsePixelMovement)
        {
            x = GridXO + (hMoveAnim.GetX(gameTime) + CurrentMinoTexture.Width / 2).Scale(App.Scale);
            y = GridYO + (vMoveAnim.GetY(gameTime) + CurrentMinoTexture.Height / 2).Scale(App.Scale);
        }
        else
        {
            x = GridXO + (currentMino.X * Dimensions.TileWidth + CurrentMinoTexture.Width / 2).Scale(App.Scale);
            y = GridYO + (currentMino.Y * Dimensions.TileHeight + CurrentMinoTexture.Height / 2).Scale(App.Scale);
        }

        if (UsePixelRotation)
        {
            rotation = rMoveAnim.GetX(gameTime);
        }
        else
        {
            rotation = Math.PI / 2 * currentMino.Rotation;
        }

        spriteBatch.Draw(texture: CurrentMinoTexture,
                destinationRectangle: new Rectangle((int)x, (int)y, 
                    CurrentMinoTexture.Width.Scale(App.Scale), CurrentMinoTexture.Height.Scale(App.Scale)),
                color: Color.White,
                rotation: (float)rotation,
                sourceRectangle: CurrentMinoTexture.Bounds,
                origin: new Vector2(CurrentMinoTexture.Width / 2F, CurrentMinoTexture.Height / 2F),
                //origin: Vector2.Zero,
                effects: SpriteEffects.None,
                layerDepth: 0.0F);
    }

    protected virtual void DrawNextPieces(SpriteBatch spriteBatch)
    {
        int y = GridYO;
        for (int i = 0; i < MinimumBagCapacity; i++)
        {
            var type = Bag.NextPieces.ToArray()[i];
            var texture = Tetromino.TetrominoTextures[type];

            spriteBatch.Draw(texture, 
                new Rectangle(BorderXO + Border.Width.Scale(App.Scale) + 10.Scale(App.Scale), 
                    y, 
                    texture.Width.Scale(App.Scale), 
                    texture.Height.Scale(App.Scale)), 
                Color.White);

            y += texture.Height.Scale(App.Scale) + 5.Scale(App.Scale);
        }
    }

    protected virtual void DrawHoldPiece(SpriteBatch spriteBatch)
    {
        if (holdPiece == null)
            return;

        var texture = Tetromino.TetrominoTextures[(TetrominoType)holdPiece];
        spriteBatch.Draw(texture, 
            new Rectangle(BorderXO - texture.Width.Scale(App.Scale) - 10.Scale(App.Scale), 
                BorderYO, 
                texture.Width.Scale(App.Scale), 
                texture.Height.Scale(App.Scale)), 
            Color.White);
    }

    protected virtual void DrawGhostPiece(SpriteBatch spriteBatch)
    {
        var YO = GetGhostPieceY();
        var pieceTexture = Tetromino.TetrominoTextures[currentMino.Type];
        var X = GridXO + (currentMino.X * Dimensions.TileWidth + CurrentMinoTexture.Width / 2).Scale(App.Scale);
        var Y = GridYO + (YO * Dimensions.TileHeight + pieceTexture.Height / 2F).Scale(App.Scale);
        var Rotation = currentMino.Rotation * Math.PI / 2;

        if (UsePixelMovement)
        {
            X = GridXO + (int)hMoveAnim.GetX(gameTime).Scale(App.Scale) + CurrentMinoTexture.Width.Scale(App.Scale) / 2;
        }

        spriteBatch.Draw(texture: CurrentMinoTexture,
            destinationRectangle: new Rectangle((int)X, (int)Y, 
                CurrentMinoTexture.Width.Scale(App.Scale), CurrentMinoTexture.Height.Scale(App.Scale)),
            color: Color.White * 0.5F,
            rotation: (float)Rotation,
            sourceRectangle: null,
            origin: new Vector2(CurrentMinoTexture.Width / 2F, 
            CurrentMinoTexture.Height / 2F),
            effects: SpriteEffects.None,
            layerDepth: 0.0F);
    }

    protected virtual void DrawKirby(SpriteBatch spriteBatch)
    {
        if (kirbySW.GetElapsedMilliseconds(gameTime) < KirbyFrameDuration)
        {
            spriteBatch.Draw(KirbyTexture,
                new Rectangle(BorderXO + Border.Width + 50, 400, KirbyTexture.Width / 2, KirbyTexture.Height),
                new Rectangle(0, 0, KirbyTexture.Width / 2, KirbyTexture.Height),
                Color.White);
        }
        else if (kirbySW.GetElapsedMilliseconds(gameTime) < KirbyFrameDuration * 2)
        {
            spriteBatch.Draw(KirbyTexture,
                new Rectangle(BorderXO + Border.Width + 50, 400, KirbyTexture.Width / 2, KirbyTexture.Height),
                new Rectangle(KirbyTexture.Width / 2, 0, KirbyTexture.Width / 2, KirbyTexture.Height),
                Color.White);
        }
        else
        {
            kirbySW.Restart(gameTime);
            spriteBatch.Draw(KirbyTexture,
                new Rectangle(BorderXO + Border.Width + 50, 400, KirbyTexture.Width / 2, KirbyTexture.Height),
                new Rectangle(0, 0, KirbyTexture.Width / 2, KirbyTexture.Height),
                Color.White);
        }
    }

    protected virtual void DrawPause(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, WindowSize, Color.Black * 0.75F);
        var dim = Font.MeasureStringScaled("Game Paused", 1.5F.Scale(App.Scale));
        spriteBatch.DrawStringOffset(Font, "Game Paused", Color.White, 
            new Vector2(WindowSize.Width / 2 - dim.X / 2, 100.Scale(App.Scale)), scale: 1.5F.Scale(App.Scale));

        for (int i = 0; i < PauseMenu.Length; i++)
        {
            var pos = GetMenuItemPosition(i);
            spriteBatch.DrawStringOffset(Font, PauseMenu[i], Color.White, pos, scale: 1F.Scale(App.Scale));
        }

        var xo = (5.0 * Math.Sin(pointerWaveSW.GetElapsedMilliseconds(pauseTime) * Math.PI / PointerWaveTime) - 4).Scale(App.Scale);
        var rect = new Rectangle((int)(pointerAnim.GetX(pauseTime) + xo),
            (int)pointerAnim.GetY(pauseTime), 
            MenuPointer.Width.Scale(App.Scale), MenuPointer.Height.Scale(App.Scale));
        spriteBatch.Draw(MenuPointer,
            rect,
            Color.White);

    }

    protected virtual void DrawVictoryScreen(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, WindowSize, Color.Black * 0.75F);
        var str = "You won!";
        var dim = Font.MeasureStringScaled(str, 1F);
        spriteBatch.DrawStringOffset(Font, str, Color.White,
            new Vector2(WindowSize.Width / 2 - dim.X / 2, WindowSize.Height / 2 - dim.Y / 2));
        str = "Press Enter to Return to the Main Menu...";
        var dim2 = Font.MeasureStringScaled(str, 1F);
        spriteBatch.DrawStringOffset(Font, str, Color.White,
            new Vector2(WindowSize.Width / 2 - dim2.X / 2, WindowSize.Height / 2 + dim.Y / 2 + 10));
    }

    protected virtual void DrawBackground(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Background, new Rectangle(0, 0, 800, 600), Color.White);
    }

    protected virtual void DrawGameFailed(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, WindowSize, Color.Black * 0.75F);
        var str = "Game Failed...";
        var dim = Font.MeasureStringScaled(str, 1F);
        spriteBatch.DrawStringOffset(Font, str, Color.White,
            new Vector2(WindowSize.Width / 2 - dim.X / 2, WindowSize.Height / 2 - dim.Y / 2));
        str = "Press Enter to Return to the Main Menu...";
        var dim2 = Font.MeasureStringScaled(str, 1F);
        spriteBatch.DrawStringOffset(Font, str, Color.White,
            new Vector2(WindowSize.Width / 2 - dim2.X / 2, WindowSize.Height / 2 + dim.Y / 2 + 10));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

        DrawBackground(spriteBatch);
        spriteBatch.Draw(Border, 
            new Rectangle(BorderXO, BorderYO, Border.Width.Scale(App.Scale), Border.Height.Scale(App.Scale)), 
            Color.White);
        spriteBatch.Draw(Grid, 
            new Rectangle(GridXO, GridYO, Grid.Width.Scale(App.Scale), Grid.Height.Scale(App.Scale)), 
            Color.White * 0.5F);
        DrawCurrentPiece(spriteBatch);
        DrawNextPieces(spriteBatch);
        DrawHoldPiece(spriteBatch);

        DrawStats(spriteBatch);

        // Draw Field
        for (int y = 0; y < Field.Height; y++)
        {
            for (int x = 0; x < Field.Width; x++)
            {
                if (Field.tiles[y, x].solid)
                {
                    var colorTexture = Tetromino.ColorTextures[Field.tiles[y, x].color];
                    var color = Tetromino.Colors[Field.tiles[y, x].color];
                    spriteBatch.Draw(colorTexture, 
                        new Rectangle(GridXO + (Dimensions.TileWidth * x).Scale(App.Scale), 
                            GridYO + (Dimensions.TileHeight * y).Scale(App.Scale), 
                            TileWidth + 1, 
                            TileHeight + 1), 
                        Color.White);
                }
            }
        }

        if (bannerSW.GetElapsedMilliseconds(gameTime) < BannerTime)
        {
            var strDim = Font.MeasureStringScaled(bannerMsg, 1.0F.Scale(App.Scale));
            spriteBatch.DrawStringOffset(Font, bannerMsg, Color.White,
                new Vector2(BorderXO + Border.Width.Scale(App.Scale) / 2 - strDim.X / 2, BorderYO + 100), 
                scale: 1.0F.Scale(App.Scale));
        }

        if (isGameFailed || IsGameFailed())
        {
            DrawGameFailed(spriteBatch);
        }
        else if (IsGameFinished())
        {
            DrawVictoryScreen(spriteBatch);
        }
        else if (isPaused)
        {
            DrawPause(spriteBatch);
        }

        spriteBatch.End();
    }
    #endregion

    #region Calculation Functions
    protected virtual double CalculateGravity()
    {
        // Time = (0.8-((Level-1)*0.007))(Level-1)
        return Math.Pow(0.8 - (double)level * 0.007, (double)level) * 1000;
    }

    protected virtual RotateToFrom GetRotateToFrom(int to, int from)
    {
        to = to % 4;
        from = from % 4;
        switch (to)
        {
            case 0:
                if (from == 1)
                    return RotateToFrom.ZeroOne;
                return RotateToFrom.ZeroThree;
            case 1:
                if (from == 2)
                    return RotateToFrom.OneTwo;
                return RotateToFrom.OneZero;
            case 2:
                if (from == 3)
                    return RotateToFrom.TwoThree;
                return RotateToFrom.TwoOne;
            case 3:
                if (from == 0)
                    return RotateToFrom.ThreeZero;
                return RotateToFrom.ThreeTwo;
        }
        return RotateToFrom.ZeroOne;
    }

    protected virtual bool IsTSpinDouble()
    {
        for (int py = 0; py < 3; py++)
        {
            for (int px = 0; px < 3; px++)
            {
                if (currentMino.GetValueAtPosition(px, 1) == 1)
                {
                    if (Field.tiles[currentMino.Y + py, px].solid)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }
        return false;
    }

    protected virtual bool IsRowFull(int y)
    {
        for (int x = 0; x < Field.Width; x++)
        {
            if (!Field.tiles[y, x].solid)
                return false;
        }
        return true;
    }

    protected virtual int GetGhostPieceY()
    {
        var layout = Tetromino.Layouts[currentMino.Type].layout;
        var YO = currentMino.Y;

        for (int by = currentMino.Y; by < 20; by++)
        {
            if (by < 0)
                continue;
            for (int y = 0; y < layout.GetLength(0); y++)
            {
                for (int x = 0; x < layout.GetLength(1); x++)
                {
                    if (currentMino.GetValueAtPosition(x, y) == 1)
                    {
                        if (by + y > 19)
                        {
                            return YO;
                        }
                        if (Field.tiles[by + y, currentMino.X + x].solid)
                        {
                            return YO;
                        }
                    }
                }
            }
            YO = by;
        }
        return YO;
    }

    protected virtual Vector2 GetMenuItemPosition(int index)
    {
        var dim = Font.MeasureStringScaled(PauseMenu[index], 1F.Scale(App.Scale));
        var X = WindowSize.Width / 2 - dim.X / 2;
        var Y = 300.Scale(App.Scale) + (dim.Y + 5.Scale(App.Scale)) * index;

        return new Vector2(X, Y);
    }

    protected virtual int CalculateLevel() => linesCleared / 10;
    #endregion

    #region Update Functions
    protected virtual void LockPiece()
    {
        var TLayout = Tetromino.Layouts[currentMino.Type];
        var layout = TLayout.layout;

        for (int y = 0; y < layout.GetLength(0); y++)
        {
            for (int x = 0; x < layout.GetLength(1); x++)
            {
                if (currentMino.GetValueAtPosition(x, y) == 1)
                {
                    if (currentMino.Y + y < 0)
                    {
                        isGameFailed = true;
                    }
                    else
                    {
                        Field.tiles[currentMino.Y + y, currentMino.X + x] = new Tile
                        {
                            color = TLayout.color,
                            solid = true
                        };
                    }
                }
            }
        }
        ClearLines(y: currentMino.Y, h: layout.GetLength(0));
        level = Math.Min(linesCleared / 10, 14);
        dropTime = CalculateGravity();
        NewTetromino();
        holdPieceUsed = false;
    }

    protected virtual void ClearLines(int y, int h)
    {
        var rowsCleared = 0;
        for (int y1 = y; y1 < y + h; y1++)
        {
            if (y1 > 19 || y1 < 0)
                break;
            if (IsRowFull(y1))
            {
                ClearRow(y1);
                rowsCleared++;
            }
        }

        bannerMsg = rowsCleared switch
        {
            1 => BannerMessages.Single,
            2 => BannerMessages.Double,
            3 => BannerMessages.Triple,
            4 => BannerMessages.Tetris,
            _ => string.Empty
        };

        if (bannerMsg == string.Empty)
            return;

        linesCleared += rowsCleared;
        if (currentMino.Type == TetrominoType.T)
        {
            if (rowsCleared == 3)
            {
                bannerMsg = BannerMessages.TSpinTriple;
                score += 1600 * level;
            }
            else if (rowsCleared == 2 && IsTSpinDouble())
            {
                bannerMsg = BannerMessages.TSpinDouble;
                score += 1200 * level;
            }
            else if (rowsCleared == 2)
            {
                score += 300 * level;
            }
            else if (rowsCleared == 1)
            {
                score += 100 * level;
            }
        }
        else
        {
            if (rowsCleared == 4)
            {
                bannerMsg = BannerMessages.Tetris;
                score += 800 * level;
            }
            else if (rowsCleared == 3)
            {
                score += 500 * level;
            }
            else if (rowsCleared == 2)
            {
                score += 300 * level;
            }
            else if (rowsCleared == 1)
            {
                score += 100 * level;
            }
        }

        bannerSW.Restart(gameTime);
    }

    protected virtual void ClearRow(int y)
    {
        for (int y1 = y; y1 > 0; y1--)
        {
            for (int x = 0; x < Field.Width; x++)
            {
                Field.tiles[y1, x] = Field.tiles[y1 - 1, x];
            }
        }
        for (int x = 0; x < Field.Width; x++)
            Field.tiles[0, x] = new Tile
            {
                color = TileColor.None,
                solid = false
            };
    }

    protected virtual void NewTetromino(TetrominoType? type = null)
    {
        currentMino = new Tetromino(type ?? Bag.NextPiece);
        CurrentMinoTexture = Tetromino.TetrominoTextures[currentMino.Type];
        hMoveAnim.FromX = currentMino.X * Dimensions.TileWidth;
        hMoveAnim.ToX = hMoveAnim.FromX;
        vMoveAnim.FromY = (currentMino.Y - 1) * Dimensions.TileHeight;
        vMoveAnim.ToY = currentMino.Y * Dimensions.TileHeight;
        vMoveAnim.Duration = dropTime;
        vMoveAnim.Restart(gameTime);
        rMoveAnim.FromX = 0;
        rMoveAnim.ToX = 0;
    }

    protected virtual void HardDrop()
    {
        var toY = GetGhostPieceY();
        var diff = toY - currentMino.Y;
        score += 2 * diff;
        currentMino.Y = toY;
        LockPiece();
    }

    public virtual void UpdatePaused()
    {
        if (GetFramesHeld(Keys.Down) == 1)
        {
            var afterPos = Math.Clamp(pointerPos + 1, 0, PauseMenu.Length - 1);
            var pointerRPos = GetMenuItemPosition(afterPos);
            pointerAnim = new MovementAnimation(pointerAnim.ToX, pointerRPos.X - MenuPointer.Width.Scale(App.Scale),
                pointerAnim.ToY, pointerRPos.Y, PointerMoveTime, pauseTime);
            pointerPos = afterPos;
        }
        else if (GetFramesHeld(Keys.Up) == 1)
        {
            var afterPos = Math.Clamp(pointerPos - 1, 0, PauseMenu.Length - 1);
            var pointerRPos = GetMenuItemPosition(afterPos);
            pointerAnim = new MovementAnimation(pointerAnim.ToX, pointerRPos.X - MenuPointer.Width.Scale(App.Scale),
                pointerAnim.ToY, pointerRPos.Y, PointerMoveTime, pauseTime);
            pointerPos = afterPos;
        }
        else if (GetFramesHeld(Keys.Enter) == 1)
        {
            switch (pointerPos)
            {
                case 0:
                    isPaused = false;
                    break;
                case 1:
                    App.PopStack = 1;
                    break;
                case 2:
                    App.PopStack = 2;
                    break;
            }
        }
    }

    public override void Update(GameTime gt)
    {
        base.Update(gt);
        App.RNG.Next();

        // Game Over Screen
        if (isGameFailed || IsGameFailed() || IsGameFinished())
        {
            if (GetFramesHeld(Keys.Enter) == 1)
                App.PopStack = 1;
            return;
        }

        // Game Won Screen
        if (IsGameFinished())
        {
            if (GetFramesHeld(Keys.Enter) == 1)
                App.PopStack = 1;
            return;
        }

        // Check for pause keypress
        if (GetFramesHeld(Pause) == 1)
        {
            isPaused = !isPaused;
        }

        // Game Paused Screen
        if (isPaused)
        {
            pauseTime += gt.ElapsedGameTime.TotalMilliseconds;
            UpdatePaused();
            return;
        }

        // Game running
        gameTime += gt.ElapsedGameTime.TotalMilliseconds;

        if (keyListener.IsKeyDown(DownMove) && vMoveAnim.GetElapsedMilliseconds(gameTime) > softDropTime)
        {
            if (!currentMino.WillCollide(XOffset: 0, YOffset: 1, ROffset: 0, field: Field))
            {
                score += 1;
                vMoveAnim.FromY = currentMino.Y * Dimensions.TileHeight;
                currentMino.Y++;
                vMoveAnim.ToY = currentMino.Y * Dimensions.TileHeight;
                vMoveAnim.Restart(gameTime);
                lockSW.Restart(gameTime);
            }
            else if (lockSW.GetElapsedMilliseconds(gameTime) >= LockTime)
            {
                LockPiece();
            }
        }
        else if (vMoveAnim.GetElapsedMilliseconds(gameTime) >= dropTime)
        {
            if (!currentMino.WillCollide(XOffset: 0, YOffset: 1, ROffset: 0, field: Field))
            {
                vMoveAnim.FromY = currentMino.Y * Dimensions.TileHeight;
                currentMino.Y++;
                vMoveAnim.ToY = currentMino.Y * Dimensions.TileHeight;
                vMoveAnim.Restart(gameTime);
                lockSW.Restart(gameTime);
            }
            else if (lockSW.GetElapsedMilliseconds(gameTime) >= LockTime)
            {
                LockPiece();
            }
        }

        if (keyListener.IsKeyDown(LeftMove))
        {
            var framesHeld = keyListener.GetFramesHeld(LeftMove);
            if (framesHeld == 1 || (framesHeld - DAS >= 0 && (framesHeld - DAS) % ARR == 0))
            {
                if (!currentMino.WillCollide(XOffset: -1, YOffset: 0, ROffset: 0, field: Field))
                {
                    hMoveAnim.FromX = currentMino.X * Dimensions.TileWidth;
                    currentMino.X--;
                    hMoveAnim.ToX = currentMino.X * Dimensions.TileWidth;
                    hMoveAnim.Restart(gameTime);
                }
            }
        }
        else if (keyListener.IsKeyDown(RightMove))
        {
            var framesHeld = keyListener.GetFramesHeld(RightMove);
            if (framesHeld == 1 || (framesHeld - DAS >= 0 && (framesHeld - DAS) % ARR == 0))
            {
                if (!currentMino.WillCollide(XOffset: 1, YOffset: 0, ROffset: 0, Field))
                {
                    hMoveAnim.FromX = currentMino.X * Dimensions.TileWidth;
                    currentMino.X++;
                    hMoveAnim.ToX = currentMino.X * Dimensions.TileWidth;
                    hMoveAnim.Restart(gameTime);
                }
            }
        }

        if (GetFramesHeld(ClockwiseRotate) == 1)
        {
            // rotate clockwise
            var rotateToFrom = GetRotateToFrom(currentMino.Rotation, currentMino.Rotation + 1);
            foreach (var offset in Tetromino.WallKickOffsets[Tetromino.Layouts[currentMino.Type].wallKickType][rotateToFrom])
            {
                if (!currentMino.WillCollide(XOffset: (int)offset.X, YOffset: (int)-offset.Y, ROffset: 1, Field))
                {
                    rMoveAnim.FromX = (float)Math.PI / 2 * currentMino.Rotation;
                    currentMino.Rotation++;
                    rMoveAnim.ToX = (float)Math.PI / 2 * currentMino.Rotation;
                    rMoveAnim.Restart(gameTime);

                    if (offset.X != 0 || offset.Y != 0)
                    {
                        currentMino.X += (int)offset.X;
                        currentMino.Y += (int)-offset.Y;
                        hMoveAnim.FromX = currentMino.X * Dimensions.TileWidth;
                        hMoveAnim.ToX = hMoveAnim.FromX;
                        vMoveAnim.FromY = currentMino.Y * Dimensions.TileHeight;
                        vMoveAnim.ToY = vMoveAnim.FromY;
                    }
                    break;
                }
            }
        }
        else if (GetFramesHeld(CounterClockwiseRotate) == 1)
        {
            // rotate counter clockwise
            var rotateToFrom = GetRotateToFrom(currentMino.Rotation, currentMino.Rotation + 3);
            foreach (var offset in Tetromino.WallKickOffsets[Tetromino.Layouts[currentMino.Type].wallKickType][rotateToFrom])
            {
                if (!currentMino.WillCollide(XOffset: (int)offset.X, YOffset: (int)-offset.Y, ROffset: 3, Field))
                {
                    rMoveAnim.FromX = (float)Math.PI / 2 * currentMino.Rotation;
                    rMoveAnim.ToX = (float)Math.PI / 2 * (currentMino.Rotation - 1);

                    currentMino.Rotation = (currentMino.Rotation + 3) % 4;
                    rMoveAnim.Restart(gameTime);

                    if (offset.X != 0 || offset.Y != 0)
                    {
                        currentMino.X += (int)offset.X;
                        currentMino.Y += (int)-offset.Y;
                        hMoveAnim.FromX = currentMino.X * Dimensions.TileWidth;
                        hMoveAnim.ToX = hMoveAnim.FromX;
                        vMoveAnim.FromY = currentMino.Y * Dimensions.TileHeight;
                        vMoveAnim.ToY = vMoveAnim.FromY;
                    }
                    break;
                }
            }
        }

        if (keyListener.GetFramesHeld(HardDropKey) == 1)
        {
            HardDrop();
        }

        if (keyListener.GetFramesHeld(Hold) == 1)
        {
            if (!holdPieceUsed)
            {
                if (holdPiece == null)
                {
                    holdPiece = currentMino.Type;
                    NewTetromino();
                }
                else
                {
                    var type = holdPiece ?? TetrominoType.O;
                    holdPiece = currentMino.Type;
                    NewTetromino(type);
                }
                holdPieceUsed = true;
            }
        }
    }
    #endregion

    #region Callbacks
    protected virtual void OnGameFinished()
    {
    }

    protected virtual void OnGameOver()
    {
    }
    #endregion
}
