using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoCustoms;
using BinksFarm.Enums;
using BinksFarm.Constants;

namespace BinksFarm.Classes;

public class Tetromino
{
    #region Statics
    public static Dictionary<TetrominoType, TetrominoLayout> Layouts = new Dictionary<TetrominoType, TetrominoLayout>
    {
        [TetrominoType.O] = new TetrominoLayout(
            new short[,]
            {
                { 1, 1 },
                { 1, 1 }
            },
            TileColor.Yellow,
            WallKickType.O
        ),
        [TetrominoType.T] = new TetrominoLayout(
            new short[,]
            {
                { 0, 1, 0 },
                { 1, 1, 1 },
                { 0, 0, 0 }
            },
            TileColor.Purple,
            WallKickType.JLSTZ
        ),
        [TetrominoType.Z] = new TetrominoLayout(
            new short[,]
            {
                { 1, 1, 0 },
                { 0, 1, 1 },
                { 0, 0, 0 }
            },
            TileColor.Red,
            WallKickType.JLSTZ
        ),
        [TetrominoType.S] = new TetrominoLayout(
            new short[,]
            {
                { 0, 1, 1 },
                { 1, 1, 0 },
                { 0, 0, 0 }
            },
            TileColor.Green,
            WallKickType.JLSTZ
        ),
        [TetrominoType.L] = new TetrominoLayout(
            new short[,]
            {
                { 0, 0, 1 },
                { 1, 1, 1 },
                { 0, 0, 0 }
            },
            TileColor.Orange,
            WallKickType.JLSTZ
        ),
        [TetrominoType.J] = new TetrominoLayout(
            new short[,]
            {
                { 1, 0, 0 },
                { 1, 1, 1 },
                { 0, 0, 0 }
            },
            TileColor.Blue,
            WallKickType.JLSTZ
        ),
        [TetrominoType.I] = new TetrominoLayout(
            new short[,]
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 1, 1, 1, 1 },
                { 0, 0, 0, 0 }
            },
            TileColor.Cyan,
            WallKickType.I
        )
    };
    public static Dictionary<TileColor, Color> Colors = new Dictionary<TileColor, Color>
    {
        [TileColor.Red] = new Color(214, 0, 0),
        [TileColor.Green] = new Color(0, 240, 0),
        [TileColor.Blue] = new Color(0, 0, 214),
        [TileColor.Cyan] = new Color(0, 241, 241),
        [TileColor.Orange] = new Color(222, 153, 9),
        [TileColor.Purple] = new Color(152, 0, 233),
        [TileColor.Yellow] = new Color(234, 228, 5),

        //[TileColor.None] = new Color(0, 0, 0, alpha: 0)
        [TileColor.None] = new Color(255, 0, 0, alpha: 255)
    };
    public static Dictionary<TileColor, Texture2D> ColorTextures;
    public static Dictionary<TetrominoType, DrawableTexture> TetrominoTextures;
    public static Dictionary<WallKickType, Dictionary<RotateToFrom, List<Vector2>>> WallKickOffsets
        = new Dictionary<WallKickType, Dictionary<RotateToFrom, List<Vector2>>>
        {
            [WallKickType.JLSTZ] = new Dictionary<RotateToFrom, List<Vector2>>
            {
                [RotateToFrom.ZeroOne] = new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(-1, 0),
                    new Vector2(-1, 1),
                    new Vector2(0, -2),
                    new Vector2(-1, -2)
                },
                [RotateToFrom.OneZero] = new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, -1),
                    new Vector2(0, 2),
                    new Vector2(1, 2)
                },
                [RotateToFrom.OneTwo] = new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, -1),
                    new Vector2(0, 2),
                    new Vector2(1, 2)
                },
                [RotateToFrom.TwoOne] = new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(-1, 0),
                    new Vector2(-1, 1),
                    new Vector2(0, -2),
                    new Vector2(-1, -2)
                },
                [RotateToFrom.TwoThree] = new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, -2),
                    new Vector2(1, -2)
                },
                [RotateToFrom.ThreeTwo] = new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(-1, 0),
                    new Vector2(-1, -1),
                    new Vector2(0, 2),
                    new Vector2(-1, 2)
                },
                [RotateToFrom.ThreeZero] = new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(-1, 0),
                    new Vector2(-1, -1),
                    new Vector2(0, 2),
                    new Vector2(-1, 2)
                },
                [RotateToFrom.ZeroThree] = new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, -2),
                    new Vector2(1, -2)
                }
            },
            [WallKickType.I] = new Dictionary<RotateToFrom, List<Vector2>>
            {
                [RotateToFrom.ZeroOne] = new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(-2, 0),
                    new Vector2(1, 0),
                    new Vector2(-2, -1),
                    new Vector2(1, 2)
                },
                [RotateToFrom.OneZero] = new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(2, 0),
                    new Vector2(-1, 0),
                    new Vector2(2, 1),
                    new Vector2(-1, -2)
                },
                [RotateToFrom.OneTwo] = new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(-1, 0),
                    new Vector2(2, 0),
                    new Vector2(-1, 2),
                    new Vector2(2, -1)
                },
                [RotateToFrom.TwoOne] = new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(-2, 0),
                    new Vector2(1, -2),
                    new Vector2(-2, 1)
                },
                [RotateToFrom.TwoThree] = new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(2, 0),
                    new Vector2(-1, 0),
                    new Vector2(2, 1),
                    new Vector2(-1, -2)
                },
                [RotateToFrom.ThreeTwo] = new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(-2, 0),
                    new Vector2(1, 0),
                    new Vector2(-2, -1),
                    new Vector2(1, 2)
                },
                [RotateToFrom.ThreeZero] = new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(-2, 0),
                    new Vector2(1, -2),
                    new Vector2(-2, 1)
                },
                [RotateToFrom.ZeroThree] = new List<Vector2>
                {
                    new Vector2(0, 0),
                    new Vector2(-1, 0),
                    new Vector2(2, 0),
                    new Vector2(-1, 2),
                    new Vector2(2, -1)
                }
            },
            [WallKickType.O] = new Dictionary<RotateToFrom, List<Vector2>>
            {
                [RotateToFrom.ZeroOne] = new List<Vector2>
                {
                    new Vector2(0, 0)
                },
                [RotateToFrom.OneZero] = new List<Vector2>
                {
                    new Vector2(0, 0)
                },
                [RotateToFrom.OneTwo] = new List<Vector2>
                {
                    new Vector2(0, 0)
                },
                [RotateToFrom.TwoOne] = new List<Vector2>
                {
                    new Vector2(0, 0)
                },
                [RotateToFrom.TwoThree] = new List<Vector2>
                {
                    new Vector2(0, 0)
                },
                [RotateToFrom.ThreeTwo] = new List<Vector2>
                {
                    new Vector2(0, 0)
                },
                [RotateToFrom.ThreeZero] = new List<Vector2>
                {
                    new Vector2(0, 0)
                },
                [RotateToFrom.ZeroThree] = new List<Vector2>
                {
                    new Vector2(0, 0)
                }
            }
        };
    public static Texture2D TileTexture;
    #endregion

    public bool IsTSpinned { get; set; } = false;

    public static void InitializeColorTextures(GraphicsDevice graphics)
    {
        Func<Color, Texture2D> CreateTexture = (c) =>
        {
            var texture = new Texture2D(graphics, 1, 1);
            texture.SetData(new[] { c });
            return texture;
        };

        ColorTextures = Colors.Aggregate(new Dictionary<TileColor, Texture2D>(), (acc, next) =>
        {
            acc.Add(next.Key, CreateTexture(next.Value));
            return acc;
        });

        LogManager.Debug("Initialized TileColors: " + string.Join(", ", ColorTextures.Select(x => new { Color = x.Key })));
    }

    public static void InitializeTetrominoTextures(GraphicsDevice graphics)
    {
        TileTexture = LogManager.Load<Texture2D>(Resource.TL01, App.contentManager);
        var TileWidth = Dimensions.TileWidth;
        var TileHeight = Dimensions.TileHeight;
        var texture = LogManager.Load<Texture2D>(Resource.TL01, App.contentManager);
        TetrominoTextures = new Dictionary<TetrominoType, DrawableTexture>();
        for (int i = 0; i < Enum.GetValues(typeof(TetrominoType)).Length; i++)
        {
            var type = (TetrominoType)i;
            var tLayout = Layouts[type];
            var layout = tLayout.layout;
            var colorTexture = ColorTextures[tLayout.color];
            var color = Colors[Layouts[type].color];
            var width = TileWidth * layout.GetLength(1);
            var height = TileHeight * layout.GetLength(0);
            var tileWidth = TileWidth;
            var tileHeight = TileHeight;

            var dTexture = new DrawableTexture(graphics, width, height, (sb) =>
            {
                for (int y = 0; y < layout.GetLength(0); y++)
                {
                    for (int x = 0; x < layout.GetLength(1); x++)
                    {
                        if (layout[y, x] == 1)
                        {
                            var rect = new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
                            //sb.Draw(colorTexture, rect, Color.White);                                
                            sb.Draw(TileTexture, rect, color);
                        }
                    }
                }
            });

            TetrominoTextures[type] = dTexture;
        }
    }

    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public int Rotation { get; set; } = 0;
    public TetrominoType Type { get; private set; }

    public Tetromino(TetrominoType type)
    {
        Type = type;
        X = 3;
        Y = -2;
        switch (type)
        {
            case TetrominoType.L:
                break;
            case TetrominoType.J:
                break;
            case TetrominoType.I:
                Y = -3;
                break;
            case TetrominoType.O:
                X = 4;
                break;
            case TetrominoType.T:
                break;
            case TetrominoType.S:
                break;
            case TetrominoType.Z:
                break;
            default:
                break;
        }
    }

    public void SetPositionAndRotation(int x, int y, int rotation)
    {

    }

    public bool WillCollide(int XOffset, int YOffset, int ROffset, Field field)
    {
        var layout = Layouts[Type].layout;
        for (int gy = 0; gy < layout.GetLength(0); gy++)
        {

            for (int gx = 0; gx < layout.GetLength(1); gx++)
            {
                if (GetValueAtPosition(gx, gy, ROffset) == 1)
                {
                    if (this.Y + gy + YOffset > 19)
                    {
                        return true;
                    }
                    if (this.X + gx + XOffset > 9 || this.X + gx + XOffset < 0)
                    {
                        return true;
                    }
                    if (Y + gy + YOffset > -1 && field.tiles[Y + gy + YOffset, X + gx + XOffset].solid)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public int GetValueAtPosition(int x, int y, int ROffset = 0)
    {
        var layout = Layouts[Type].layout;
        var postRotation = (Rotation + ROffset) % 4;

        if (postRotation == 0)
            return layout[y, x];
        if (Type == TetrominoType.O)
            return layout[y, x];

        int x1 = 0;
        int y1 = 0;
        int n = layout.GetLength(0); // 2, 3, or 4

        switch (postRotation)
        {
            case 1:
                x1 = y;
                y1 = n - 1 - x;
                break;
            case 2:
                x1 = n - 1 - x;
                y1 = n - 1 - y;
                break;
            case 3:
                x1 = n - 1 - y;
                y1 = x;
                break;
        }

        return layout[y1, x1];
    }

    public void PrintLayout(Action<string> printFunction)
    {
        var str = "\n";
        var layout = Layouts[Type].layout;
        for (int y = 0; y < layout.GetLength(0); y++)
        {
            str += $"{y}: ";
            for (int x = 0; x < layout.GetLength(1); x++)
            {
                str += layout[y, x].ToString();
            }
            str += '\n';
        }
        printFunction(str);
    }
}
