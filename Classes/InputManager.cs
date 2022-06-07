using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace BinksFarm.Classes;

public class InputManager
{
    public static Dictionary<string, Keys> ConfiguredKeys = new Dictionary<string, Keys>();
    private static Dictionary<Keys, int> KeyStates = new Dictionary<Keys, int>();
    private static Dictionary<Keys, bool> PreviousKeyStates = new Dictionary<Keys, bool>();

    public static void InitializeInputManager()
    {
        ConfiguredKeys.Add("Up", Keys.Up);
        ConfiguredKeys.Add("Down", Keys.Down);
        ConfiguredKeys.Add("Left", Keys.Left);
        ConfiguredKeys.Add("Right", Keys.Right);

        ConfiguredKeys.Add("Enter", Keys.Enter);
        ConfiguredKeys.Add("Back", Keys.Back);
        ConfiguredKeys.Add("Escape", Keys.Escape);

        foreach (var (_, key) in ConfiguredKeys)
        {
            KeyStates.Add(key, 0);
            PreviousKeyStates.Add(key, false);
        }
    }

    public static void AddAction(string actionName, Keys key)
    {
        ConfiguredKeys.Add(actionName, key);
        KeyStates.Add(key, 0);
        PreviousKeyStates.Add(key, false);
    }

    public static void Update()
    {
        var keys = Keyboard.GetState();
        foreach (var (action, key) in ConfiguredKeys)
        {
            PreviousKeyStates[key] = KeyStates[key] > 0;

            if (keys.IsKeyDown(key))
            {
                ++KeyStates[key];
            }
            else
            {
                KeyStates[key] = 0;
            }
        }
    }

    public static int GetKeyState(string action)
    {
        var key = ConfiguredKeys[action];
        return KeyStates[key];
    }

    public static bool GetPreviousKeyState(string action)
    {
        var key = ConfiguredKeys[action];
        return PreviousKeyStates[key];
    }
}
