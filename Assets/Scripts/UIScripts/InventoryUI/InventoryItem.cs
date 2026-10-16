using System;
using System.Collections.Generic;
using UnityEngine;
using Grids;
using UnityEditor;

[System.Serializable]
public class InventoryItem
{
    // this class will be used to represent the Items when they are in a UI state
    public string itemName;
    public string description;
    public int quantity = 1;
    //public int baseWidth;
    //public int baseHeight;
    public Sprite icon;
    public ItemData itemData;
    public bool IsRotated { get; private set; }

    private ItemGrid _holdingGrid;

    private List<Vector2Int> _localGridPositions
    {
        get
        {
            List<Vector2Int> results = new List<Vector2Int>();
            foreach ((Vector2Int basePos, Vector2Int localPos) tuple in _matchedGridPositions)
            {
                results.Add(tuple.localPos);
            }
            return results;
        }

    }                       // contains reference to the slots its being held by so we can tell them to remove this later
    private List<(Vector2Int itemDataPosition, Vector2Int localGridPosition)> _matchedGridPositions = new List<(Vector2Int , Vector2Int)>();
    public ItemGrid HoldingGrid => _holdingGrid;
    [NonSerialized] public Vector2Int PivotPosition; //this represents the Position the item is being dragged and rotated around

    [NonSerialized]
    public Vector2 ScreenPos; // the offset that will be used to place the Object at the mouses position on screen for dragging
    [NonSerialized]
    public Vector2 DraggingScale; // the Deltasize of the Sprite in inventory is saved to here to be accessed when the item is dragged
                                    // will be assigned in UIDraw
    
    public List<Vector2Int> LocalGridPositions
    {
        get
        {
            if(_localGridPositions == null || _localGridPositions.Count == 0)
                Debug.Log("well 'ello, 'ello, 'ello. whats all this then. the Bloody list is empty?");
            return _localGridPositions;
        }
    }
    
    
    //public int Width => IsRotated ? baseHeight : baseWidth;
    //public int Height => IsRotated ? baseWidth : baseHeight;

    public InventoryItem(ItemData itemData)
    {
        this.itemData = itemData;
    }

    public InventoryItem(string itemName, string description, int quantity = 1)
    {
        this.itemName = itemName;
        this.description = description;
        this.quantity = Mathf.Max(0, quantity);
        //baseWidth = 1;
        //baseHeight = 1;
        icon = null;
        itemData = null;
    }

    public InventoryItem(string itemName, string description, Sprite icon = null, ItemData itemData = null)
    {
        this.itemName = itemName;
        this.description = description;
        quantity = 1;
        //this.baseWidth = Mathf.Max(1, baseWidth);
        //this.baseHeight = Mathf.Max(1, baseHeight);
        this.icon = icon;
        this.itemData = itemData;
    }

    public InventoryItem(InventoryItem other)
    {
        itemName = other.itemName;
        description = other.description;
        quantity = other.quantity;
        //baseWidth = other.baseWidth;
       // baseHeight = other.baseHeight;
        icon = other.icon;
        itemData = other.itemData;
        IsRotated = other.IsRotated;
    }

    public void AddQuantity(int amount)
    {
        quantity = Mathf.Max(0, quantity + amount);
    }

    public void Rotate()
    {
        IsRotated = !IsRotated;
    }

    public override string ToString()
    {
        //return $"{itemName} ({Width}x{Height}) - {description}";
        return $"{itemName} {description} {itemData.GridPositions.Count} spaces large";
    }

    public void AssignInventory(ItemGrid newOwner, Vector2Int[]  newHeldPositions, Vector2Int[] matchedItemDataPositions)
    {
        // this will be used to tell it its owning inventories
        _holdingGrid = newOwner;

        _matchedGridPositions = new List<(Vector2Int itemDataPosition, Vector2Int localGridPosition)>();
        if (newHeldPositions.Length > 0)
        {
            for (int i = 0; i < newHeldPositions.Length; i++)
            {
                _matchedGridPositions.Add((matchedItemDataPositions[i], newHeldPositions[i]));
            }
        }
    }
    public void ClearOwningInventory()
    {
        // this should clear itself from its current inventories
        foreach(var item in _localGridPositions)
            _holdingGrid.ItemGridSpaces[item].heldItem = null;       
        
        //this should go through all the gridspaces in the holding inventory that holds this item and remove it
        
        // then no that all of its held positions have lost their reference to it.
        // then this itself should forget those positions and the grid they belong to
        _matchedGridPositions.Clear();
        _holdingGrid = null;
    }

    public void SetPivot(Vector2Int gridPosition)
    {
        //this method takes a local grid position from something 
        // then finds the matching ItemData Position to be used for placement n such
        bool foundPivot = false;// found Placement
        foreach (var item in _matchedGridPositions)
        {
            if (item.localGridPosition == gridPosition)
            {
                PivotPosition = item.itemDataPosition;
                return;
            }
        }
        //getting to this point means no pivot was found
        Debug.Log("No Pivot Found!");
    }
}
