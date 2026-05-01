using System.Collections.Generic;
using UnityEngine;

public class InventoryGrid
{
    public int Width { get; }
    public int Height { get; }
    public InventoryItem[,] Cells { get; }

    private readonly List<InventoryItem> items = new List<InventoryItem>();
    private readonly Dictionary<InventoryItem, GridPosition> itemPositions = new Dictionary<InventoryItem, GridPosition>();

    public InventoryGrid(int width, int height)
    {
        Width = Mathf.Max(1, width);
        Height = Mathf.Max(1, height);
        Cells = new InventoryItem[Width, Height];
    }

    public IReadOnlyList<InventoryItem> Items => items.AsReadOnly();

    public bool CanPlaceItem(InventoryItem item, GridPosition position)
    {
        // this Method checks if the item can be placed into the grid
        if (item == null)
            return false;

        int itemWidth = item.Width;
        int itemHeight = item.Height;

        if (position.X < 0 || position.Y < 0)
            return false;

        if (position.X + itemWidth > Width || position.Y + itemHeight > Height)
            return false;

        for (int x = 0; x < itemWidth; x++)
        {
            for (int y = 0; y < itemHeight; y++)
            {
                if (Cells[position.X + x, position.Y + y] != null)
                    // checks if any grid positions have been filled
                    return false;
            }
        }

        return true;
    }

    public bool PlaceItem(InventoryItem item, GridPosition position, bool rotate = false)
    {
        if (item == null || items.Contains(item))
            return false;

        if (rotate && !item.IsRotated)
            item.Rotate();

        if (!CanPlaceItem(item, position))
        {
            if (rotate && item.IsRotated)
                item.Rotate();
            return false;
        }

        FillCells(item, position);
        items.Add(item);
        itemPositions[item] = position;
        return true;
    }

    public bool RemoveItem(InventoryItem item)
    {
        if (item == null || !itemPositions.ContainsKey(item))
            return false;

        ClearCells(item);
        items.Remove(item);
        itemPositions.Remove(item);
        return true;
    }

    public bool MoveItem(InventoryItem item, GridPosition newPosition, bool rotate = false)
    {
        if (item == null || !itemPositions.ContainsKey(item))
            return false;

        GridPosition oldPosition = itemPositions[item];
        bool originalRotation = item.IsRotated;

        ClearCells(item);

        if (rotate)
            item.Rotate();

        if (!CanPlaceItem(item, newPosition))
        {
            if (rotate)
                item.Rotate(); // Revert rotation

            FillCells(item, oldPosition);
            return false;
        }

        FillCells(item, newPosition);
        itemPositions[item] = newPosition;
        return true;
    }

    public bool IsCellOccupied(GridPosition position)
    {
        if (position.X < 0 || position.Y < 0 || position.X >= Width || position.Y >= Height)
            return false;

        return Cells[position.X, position.Y] != null;
    }

    public GridPosition? GetItemPosition(InventoryItem item)
    {
        if (item == null || !itemPositions.ContainsKey(item))
            return null;

        return itemPositions[item];
    }

    public bool RotateItem(InventoryItem item)
    {
        GridPosition? pos = GetItemPosition(item);
        if (pos == null)
            return false;

        return MoveItem(item, pos.Value, rotate: true);
    }

    public bool TryFindFirstAvailablePosition(InventoryItem item, out GridPosition position)
    {
        return TryFindFirstAvailablePlacement(item, out position, out _);
    }

    public bool TryFindFirstAvailablePlacement(InventoryItem item, out GridPosition position, out bool rotateForPlacement)
    {
        position = default;
        rotateForPlacement = false;

        if (item == null)
            return false;

        int defaultWidth = item.Width;
        int defaultHeight = item.Height;
        if (TryFindPositionForSize(defaultWidth, defaultHeight, out position))
        {
            return true;
        }

        int rotatedWidth = item.Height;
        int rotatedHeight = item.Width;
        bool canTryRotated = defaultWidth != rotatedWidth || defaultHeight != rotatedHeight;
        if (!canTryRotated)
        {
            return false;
        }

        if (TryFindPositionForSize(rotatedWidth, rotatedHeight, out position))
        {
            rotateForPlacement = true;
            return true;
        }

        return false;
    }

    public bool TryAutoPlaceItem(InventoryItem item, out GridPosition position, out bool rotatedDuringPlacement)
    {
        position = default;
        rotatedDuringPlacement = false;

        if (item == null || items.Contains(item))
            return false;

        if (!TryFindFirstAvailablePlacement(item, out position, out bool rotateForPlacement))
            return false;

        if (!PlaceItem(item, position, rotateForPlacement))
            return false;

        rotatedDuringPlacement = rotateForPlacement;
        return true;
    }

    public bool HasSpaceFor(InventoryItem item)
    {
        return TryFindFirstAvailablePlacement(item, out _, out _);
    }

    private bool TryFindPositionForSize(int width, int height, out GridPosition position)
    {
        position = default;

        if (width <= 0 || height <= 0)
            return false;

        for (int y = 0; y <= Height - height; y++)
        {
            for (int x = 0; x <= Width - width; x++)
            {
                if (CanPlaceAtSize(width, height, x, y))
                {
                    position = new GridPosition(x, y);
                    return true;
                }
            }
        }

        return false;
    }

    private bool CanPlaceAtSize(int width, int height, int startX, int startY)
    {
        if (startX < 0 || startY < 0)
            return false;

        if (startX + width > Width || startY + height > Height)
            return false;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (Cells[startX + x, startY + y] != null)
                    return false;
            }
        }

        return true;
    }

    private void FillCells(InventoryItem item, GridPosition position)
    {
        for (int x = 0; x < item.Width; x++)
        {
            for (int y = 0; y < item.Height; y++)
            {
                Cells[position.X + x, position.Y + y] = item;
            }
        }
    }

    private void ClearCells(InventoryItem item)
    {
        if (!itemPositions.TryGetValue(item, out GridPosition position))
            return;

        for (int x = 0; x < item.Width; x++)
        {
            for (int y = 0; y < item.Height; y++)
            {
                if (Cells[position.X + x, position.Y + y] == item)
                {
                    Cells[position.X + x, position.Y + y] = null;
                }
            }
        }
    }
}
