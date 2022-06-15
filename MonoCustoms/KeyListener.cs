using Microsoft.Xna.Framework.Input;

namespace MonoCustoms
{
    public class KeyListener
    {
        public Dictionary<Keys, int> keyStates;

        public KeyListener()
        {
            keyStates = new Dictionary<Keys, int>();
        }

        public void Add(Keys key, int framesHeld = 0)
        {
            if (!keyStates.ContainsKey(key))
                keyStates.Add(key: key, value: framesHeld);
        }

        public void UpdateKeyStates(KeyboardState keyboardState)
        {
            foreach (var key in keyStates.Keys)
            {
                if (keyStates.ContainsKey(key: key))
                {
                    if (keyboardState.IsKeyDown(key: key))
                    {
                        keyStates[key]++;
                    }
                    else if (keyboardState.IsKeyUp(key: key))
                    {
                        keyStates[key] = 0;
                    }
                }
            }
        }

        public bool IsKeyDown(Keys key)
        {
            if (!keyStates.ContainsKey(key: key))
                return false;
            else
                return keyStates[key] > 0;
        }

        public int? GetFramesHeld(Keys key)
        {
            if (!keyStates.ContainsKey(key))
                return null;
            else
                return keyStates[key];
        }

        public static KeyListener operator +(KeyListener listener, Keys key)
        {
            listener.Add(key: key, framesHeld: 0);
            return listener;
        }
    }
}