using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monomino.Classes;
using Monomino.Constants;
using Monomino.Interfaces;
using Monomino.States;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Monomino;

public class App : Microsoft.Xna.Framework.Game
{
    #region Statics
    public static ContentManager contentManager;
    public static Random RNG = new Random(DateTime.UtcNow.Second);
    public static Configuration GameConfig { get; set; }
    #endregion

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Stack<IState> CurrentState = new Stack<IState>();
    public static IState QueuedState = null;
    public static int PopStack = 0;
    public static ITransition OutTransition = null;
    public static ITransition InTransition = null;
    public static bool UpdateResolution = false;
    public static bool UpdateFullscreen = false;
    public static double Scale = 1.0;

    public App()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        try
        {
            var json = File.ReadAllText("settings.json");
            GameConfig = JsonSerializer.Deserialize<Configuration>(json);
        }
        catch (Exception)
        {
            GameConfig = new Configuration();
        }

        if (GameConfig.ResolutionX != 0)
        {
            _graphics.PreferredBackBufferWidth = GameConfig.ResolutionX;
            _graphics.PreferredBackBufferHeight = GameConfig.ResolutionY;
        }
        else
        {
            _graphics.PreferredBackBufferWidth = Dimensions.DefaultWindowWidth;
            _graphics.PreferredBackBufferHeight = Dimensions.DefaultWindowHeight;
        }

        _graphics.IsFullScreen = GameConfig.IsFullscreen;
        _graphics.ApplyChanges();

        Scale = (double)GameConfig.ResolutionX / (double)Dimensions.DefaultWindowWidth;

        LogManager.InitializeLoggingFile();
        GameConfig.ResolutionX = GraphicsDevice.Viewport.Width;
        GameConfig.ResolutionY = GraphicsDevice.Viewport.Height;
        contentManager = Content;
        Tetromino.InitializeColorTextures(GraphicsDevice);
        Tetromino.InitializeTetrominoTextures(GraphicsDevice);
        CurrentState.Push(new TitleScreen(GraphicsDevice));
        InputManager.InitializeInputManager();

        InTransition = new FadeInTransition(GraphicsDevice, 1000.0, new GameTime().TotalGameTime.TotalMilliseconds);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        InputManager.Update();
        CurrentState.Peek().Update(gameTime);

        if (OutTransition != null)
        {
            OutTransition.Update(gameTime);
        }
        else if (OutTransition == null && InTransition != null)
        {
            InTransition?.Update(gameTime);
        }

        if (InTransition != null && InTransition.IsDone())
        {
            InTransition = null;
        }
        if (OutTransition != null && OutTransition.IsDone())
        {
            OutTransition = null;
            InTransition?.Timer.Restart(gameTime.TotalGameTime.TotalMilliseconds);
        }

        if (OutTransition == null || OutTransition.IsDone())
        {
            if (UpdateResolution)
            {
                _graphics.PreferredBackBufferWidth = GameConfig.ResolutionX;
                _graphics.PreferredBackBufferHeight = GameConfig.ResolutionY;
                _graphics.ApplyChanges();
                Scale = (double)GameConfig.ResolutionX / (double)Dimensions.DefaultWindowWidth;
                UpdateResolution = false;
            }
            if (UpdateFullscreen)
            {
                _graphics.IsFullScreen = GameConfig.IsFullscreen;
                _graphics.ApplyChanges();
                UpdateFullscreen = false;
            }

            if (QueuedState != null)
            {
                CurrentState.Push(QueuedState);
                QueuedState = null;
            }
            if (PopStack > 0)
            {
                for (int i = 0; i < PopStack; i++)
                    CurrentState.Pop();
                if (CurrentState.Count == 0)
                    Exit();
                PopStack = 0;
                CurrentState.Peek().OnResume();
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        CurrentState.Peek().Draw(_spriteBatch);
        OutTransition?.Draw(_spriteBatch);
        if (OutTransition == null || OutTransition.IsDone())
        {
            InTransition?.Draw(_spriteBatch);
        }

        base.Draw(gameTime);
    }
}