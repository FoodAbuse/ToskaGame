using System.Collections;
using System.Collections.Generic;
using Grids;
using UnityEngine;

public abstract class GridSpace
{
    protected  Vector2Int gridPosition;     // its x,y position 
    protected  IGrid _gridParent;    // the grid this belongs to

    public abstract IGrid OwningGrid
    {
        get;
    }// grid parent

    public abstract Vector2Int GridPosition
    {
        get;
    }
    

    public GridSpace(Vector2Int gridPos, IGrid owningGrid)
    {
        gridPosition = gridPos;     // here we allow the GridSpaces position to be set when it is constructed
        _gridParent = owningGrid;
        
    }

    public GridSpace()
    {
        
    }
}
public class ItemGridSpace :GridSpace
{
    // will put the ItemGridSpaces Item reference here I guess
    public ItemGrid owningItemGrid;
    private Vector2Int _gridPosition;

    public override Vector2Int GridPosition
    {
        get
        {
            return _gridPosition;
        }
    }

    public override IGrid OwningGrid
    {
        get
        {
            return owningItemGrid as IGrid;
        } 
        
    }
    public InventoryItem heldItem = null;

    public ItemGridSpace(Vector2Int gridPos, ItemGrid owningGrid)
    {
        _gridPosition = gridPos;
        owningItemGrid = owningGrid;
    }
    

    public void ClearSpace()
    {
        InventoryItem heldItem = null;
    }

    public bool HasItem()
    {
        return heldItem != null;
    }

    public bool AddItem(ItemData incomingData)    // this adds an Item to this space and tells the owning inventory to update matching spaces to also be filled as necessary
    {
        return owningItemGrid.AddItemAtSpace(incomingData,GridPosition);
    }

    public bool AttemptAddItemToSpace()
    {
        // this is here to be overwritten for proper item swapping and stacking code 
        return false;
    }
}
