namespace Monomino.Enums;

public enum DisplayMode : byte
{
    Windowed = 0,
    Borderless_Windowed = 1,
    Fullscreen = 2,
    Borderless_Fullscreen = 3
}

public enum GameState : int
{
    MainMenu = 0,
    Map = 1,
    LoadingScreen = 2,
    Marathon = 3
}

public enum ChangeState : int
{
    Exit = -1,
    NoChange = 0,
    MainMenu = 1,
    Overworld = 2,
    SimpleOverworld = 3
}

public enum Direction : byte
{
    Left = 0,
    Right = 1,
    Up = 2,
    Down = 3
}

public enum TileColor : byte
{
    None = 0,
    Blue = 1,
    Orange = 2,
    Yellow = 3,
    Green = 4,
    Cyan = 5,
    Red = 6,
    Purple = 7,
}

public enum TetrominoType : byte
{
    O = 0,
    T = 1,
    S = 2,
    Z = 3,
    L = 4,
    J = 5,
    I = 6
}

public enum WallKickType : byte
{
    JLSTZ = 0,
    I = 1,
    O = 2
}

public enum RotateToFrom : byte
{
    ZeroOne = 0,
    OneZero = 1,
    OneTwo = 2,
    TwoOne = 3,
    TwoThree = 4,
    ThreeTwo = 5,
    ThreeZero = 6,
    ZeroThree = 7
}

public enum BindKeys : byte
{
    LeftMove = 0,
    RightMove = 1,
    DownMove = 2,
    HardDrop = 3,
    Hold = 4,
    RotateClockwise = 5,
    RotateCounterClockwise = 6,
    Pause = 7,
}

public enum enumBindingPreset : byte
{
    Default = 0,
    WASD = 1,
    Custom
}
