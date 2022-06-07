using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monomino.Enums;
using Monomino.Interfaces;
using System;
using MonoCustoms;
using Microsoft.Xna.Framework.Input;

namespace Monomino.Classes;

public abstract class UserInputState : IState
{
    protected KeyListener keyListener = new KeyListener();

    public void AddKey(Keys key)
    {
        keyListener.Add(key, 0);
    }

    public void AddKeys(Keys[] keys)
    {
        foreach (var key in keys)
        {
            keyListener.Add(key, 0);
        }
    }

    public int GetFramesHeld(Keys key)
    {
        return keyListener.GetFramesHeld(key) ?? 0;
    }

    public void SetFramesHeld(Keys key, int frames)
    {
        keyListener.keyStates[key] = frames;
    }

    public bool IfKey(Keys key, Predicate<int> predicate)
    {
        return predicate(GetFramesHeld(key));
    }

    public bool KeyFramesHeld(Keys key, Predicate<int> predicate)
    {
        return GetFramesHeld(key) > 0 && predicate(GetFramesHeld(key));
    }

    public abstract void Draw(SpriteBatch spriteBatch);

    public abstract ChangeState GetChangeState();

    public abstract GameState GetGameState();

    public virtual void Update(GameTime gameTime)
    {
        keyListener.UpdateKeyStates(Keyboard.GetState());
    }

    public virtual void OnResume()
    {
        SetFramesHeld(Keys.Enter, 1);
    }
}
