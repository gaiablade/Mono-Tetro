using Microsoft.Xna.Framework.Input;
using Monomino.Constants;
using Monomino.Enums;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Monomino.Classes;

public class Configuration
{
    public int ResolutionX { get; set; }
    public int ResolutionY { get; set; }
    public enumBindingPreset PresetNumber { get; set; }
    public Dictionary<BindKeys, Keys> KeyBindings { get; set; }
    public bool UsePixelMovement { get; set; } = true;
    public bool UsePixelRotation { get; set; } = true;
    public int DAS { get; set; } = 8;
    public int ARR { get; set; } = 3;
    public bool IsFullscreen { get; set; } = false;

    public Configuration()
    {
        ResolutionX = Dimensions.DefaultWindowWidth;
        ResolutionY = Dimensions.DefaultWindowHeight;
        PresetNumber = enumBindingPreset.Default;
        KeyBindings = KeyBindingPresets.Presets[(int)PresetNumber];
        UsePixelMovement = true;
        UsePixelRotation = true;
    }

    public void ExportConfiguration()
    {
        var json = JsonSerializer.Serialize<Configuration>(this, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText("settings.json", json);
    }
}
