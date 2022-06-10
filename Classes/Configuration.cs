using Microsoft.Xna.Framework.Input;
using BinksFarm.Constants;
using BinksFarm.Enums;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using MonoCustoms;

namespace BinksFarm.Classes;

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
    public int BackgroundDim { get; set; } = 0;

    public void TogglePixelMovement() => UsePixelMovement = !UsePixelMovement;
    public void TogglePixelRotation() => UsePixelRotation = !UsePixelRotation;
    public void ToggleFullscreen() => IsFullscreen = !IsFullscreen;
    public void IncrementBackgroundDim() => BackgroundDim = (BackgroundDim + 10).ClampLoop(0, 100);
    public void DecrementBackgroundDim() => BackgroundDim = (BackgroundDim - 10).ClampLoop(0, 100);

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
