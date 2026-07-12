using UnityEngine;

namespace Grids
{
    public class InventoryGrid 
    {
        /*public InventoryGrid(int GridHeight, int GridWidth) :  base(GridHeight, GridWidth)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    // now we construct the gridspace for that coordinate
                    GridSpace newgridSpace =  new GridSpace(new Vector2Int(x, y));
                    //and add it to the dictionary
                    //gridSpaces.Add(new Vector2Int(x,y), newgridSpace);
                }
            }
        } */
        //new cameron code here
        public bool AddItemToGrid(ItemData incomingData)
        {
            // we check each spot of the grid till we find a valid position and then slot the item in
            //here we attempt to add the item to the grid
    
            return false;
        }

        public bool AddItemToGrid(ItemData incomingData, Vector2Int targetPosition)
        {
            return false;
        }
    
    
    
        /*
    
    public InventoryGrid(int width, int height)
    {
        Width = Mathf.Max(1, width);
        Height = Mathf.Max(1, height);
        Cells = new InventoryItem[Width, Height];
    }

    public IReadOnlyList<InventoryItem> Items => items.AsReadOnly();

    public bool CanPlaceItem(InventoryItem item, GridPosition position)
    {
       /* // this Method checks if the item can be placed into the grid
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
       
       // rewriting this to use the Iventory Items ItemData
       if(item == null)
           return false;
       // instead of grabbing the items width and height
       // we will instead refer to the items Itemdata to determine its grid positions
       // its grid positions will always be local so some manipulation is needed
       Vector2Int[] gridPositions = new Vector2Int[item.itemData.GridPositions.Count];
       item.itemData.GridPositions.CopyTo(gridPositions, 0); // create a copy of the itemdatas patterns
       // now we go through the array
       foreach (Vector2Int gridPosition in gridPositions)
       {
           // we check if corresponding cells are Empty or if they contain something
           // for now we will assume the holding point (where the mouse is dragging from is the bottom left hand corner
           
           //grab the position offset by the ItemsGridPosition maybe rename them to local grid position
           if (Cells[gridPosition.x, gridPosition.y] != null)
           {
               // check if the position is filled if so then return false
               return false;
               // AT THE MOMENT THIS CHECKS IT FROM THE BASE OF THE GRID DOESNT ACCOUNT FOR MOUSE POSITION OR NUTHIN
               // THO I think thats fine for this method
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

    public GridPosition GetItemPosition(InventoryItem item)
    {
        if (item == null || !itemPositions.ContainsKey(item))
            return null;

        return itemPositions[item];
    }

    public bool RotateItem(InventoryItem item)
    {
        GridPosition pos = GetItemPosition(item);
        if (pos == null)
            return false;

        return MoveItem(item, pos, rotate: true);
    }

    public bool TryFindFirstAvailablePosition(InventoryItem item, out GridPosition position)
    {
        return TryFindFirstAvailablePlacement(item, out position, out _);
    }

    public bool TryFindFirstAvailablePlacement(InventoryItem item, out GridPosition position, out bool rotateForPlacement)
    {
        position = default; // position is 0,0 to begin with I believe
        rotateForPlacement = false; // dont rotate for placement

        if (item == null)
            return false;       // exit out if no item has been passed

        int defaultWidth = item.Width;      // get the items width
        int defaultHeight = item.Height;
        if (TryFindPositionForSize(defaultWidth, defaultHeight, out position))  // we will replace this
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
    */
    }
}

