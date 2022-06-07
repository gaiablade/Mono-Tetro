using Monomino.Enums;

namespace Monomino.Classes;

public class Field
{
    public readonly int Width;
    public readonly int Height;
    public Tile[,] tiles;

    public Field()
    {
        Width = 10;
        Height = 20;

        tiles = new Tile[Height, Width];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                tiles[y, x] = new Tile { color = TileColor.None, solid = false };
            }
        }
    }
}
