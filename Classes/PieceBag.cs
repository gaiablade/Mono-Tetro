using System;
using System.Collections.Generic;
using Monomino.Enums;

namespace Monomino.Classes;

public class PieceBag
{
    public Queue<TetrominoType> NextPieces { get; set; }
    private int minCapacity;

    public PieceBag(int minCapacity = 1)
    {
        this.minCapacity = minCapacity;

        NextPieces = new Queue<TetrominoType>();

        while (NextPieces.Count < minCapacity)
        {
            AddMorePieces();
        }
    }

    private void AddMorePieces()
    {
        var n = Enum.GetValues(typeof(TetrominoType)).Length;

        List<int> temp = new List<int>();
        for (int i = 0; i < n; i++)
            temp.Add(i);

        while (temp.Count > 0)
        {
            var index = App.RNG.Next() % temp.Count;
            NextPieces.Enqueue((TetrominoType)temp[index]);
            temp.RemoveAt(index);
        }
    }

    public TetrominoType NextPiece
    {
        get
        {
            var retVal = NextPieces.Dequeue();
            while (NextPieces.Count < minCapacity)
                AddMorePieces();
            return retVal;
        }
    }
}
