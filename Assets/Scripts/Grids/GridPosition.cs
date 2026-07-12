using System;

namespace Grids
{
    [Serializable]
    public class GridPosition
    {
        public int X;
        public int Y;
        
        public GridPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public GridPosition()
        {
            X = 0;
            Y = 0;
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

    public class ItemGridPosition : GridPosition
    {
        public ItemGridPosition(int x, int y, InventoryItem item)
        {
            X = x;
            Y = y;
            Item = item;
        }
        public InventoryItem Item;
    }
    
}