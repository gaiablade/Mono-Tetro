using Microsoft.Xna.Framework.Input;
using Monomino.Enums;
using System.Collections.Generic;

namespace Monomino.Constants;

public static class KeyBindingPresets
{
    public static readonly List<Dictionary<BindKeys, Keys>> Presets = new List<Dictionary<BindKeys, Keys>>
    {
        new Dictionary<BindKeys, Keys>
        {
            [BindKeys.LeftMove] = Keys.Left,
            [BindKeys.RightMove] = Keys.Right,
            [BindKeys.DownMove] = Keys.Down,
            [BindKeys.HardDrop] = Keys.Up,
            [BindKeys.Hold] = Keys.C,
            [BindKeys.RotateClockwise] = Keys.Z,
            [BindKeys.RotateCounterClockwise] = Keys.X,
            [BindKeys.Pause] = Keys.Escape
        },
        new Dictionary<BindKeys, Keys>
        {
            [BindKeys.LeftMove] = Keys.A,
            [BindKeys.RightMove] = Keys.D,
            [BindKeys.DownMove] = Keys.S,
            [BindKeys.HardDrop] = Keys.W,
            [BindKeys.Hold] = Keys.L,
            [BindKeys.RotateClockwise] = Keys.J,
            [BindKeys.RotateCounterClockwise] = Keys.K,
            [BindKeys.Pause] = Keys.Escape
        },
    };
}
