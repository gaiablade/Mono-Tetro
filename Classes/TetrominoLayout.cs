using Monomino.Enums;

namespace Monomino.Classes;

public class TetrominoLayout
{
    public short[,] layout;
    public int wh;
    public TileColor color;
    public WallKickType wallKickType;

    public TetrominoLayout(short[,] layout, TileColor color, WallKickType wallKickType)
    {
        this.layout = layout;
        this.color = color;
        this.wallKickType = wallKickType;
        wh = layout.GetLength(0);
    }
}
