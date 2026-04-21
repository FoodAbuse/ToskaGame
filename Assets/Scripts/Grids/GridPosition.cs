using System;

[Serializable]
public struct GridPosition
{
    public int X;
    public int Y;

    public GridPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static GridPosition Zero => new GridPosition(0, 0);

    public override bool Equals(object obj)
    {
        return obj is GridPosition position && X == position.X && Y == position.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}
