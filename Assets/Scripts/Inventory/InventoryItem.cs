using System.Collections.Generic;
using UnityEngine;
using Grids;

[System.Serializable]
public class InventoryItem
{
    // this class will be used to represent the Items when they are in a UI state
    public string itemName;
    public string description;
    public int quantity = 1;
    public int baseWidth;
    public int baseHeight;
    public Sprite icon;
    public ItemData itemData;
    public bool IsRotated { get; private set; }

    private ItemGrid _holdingGrid;                          // contains reference to the slots its being held by so we can tell them to remove this later
    private List<Vector2Int> _holdingGridPositions = new List<Vector2Int>();

    public ItemGrid HoldingGrid => _holdingGrid;
    public List<Vector2Int> HoldingGridPositions
    {
        get
        {
            if(_holdingGridPositions == null || _holdingGridPositions.Count == 0)
                Debug.Log("well 'ello, 'ello, 'ello. whats all this then. the Bloody list is empty?");
            return _holdingGridPositions;
        }
    }
    
    
    public int Width => IsRotated ? baseHeight : baseWidth;
    public int Height => IsRotated ? baseWidth : baseHeight;

    public InventoryItem(ItemData itemData)
    {
        this.itemData = itemData;
    }

    public InventoryItem(string itemName, string description, int quantity = 1)
    {
        this.itemName = itemName;
        this.description = description;
        this.quantity = Mathf.Max(0, quantity);
        baseWidth = 1;
        baseHeight = 1;
        icon = null;
        itemData = null;
    }

    public InventoryItem(string itemName, string description, int baseWidth, int baseHeight, Sprite icon = null, ItemData itemData = null)
    {
        this.itemName = itemName;
        this.description = description;
        quantity = 1;
        this.baseWidth = Mathf.Max(1, baseWidth);
        this.baseHeight = Mathf.Max(1, baseHeight);
        this.icon = icon;
        this.itemData = itemData;
    }

    public InventoryItem(InventoryItem other)
    {
        itemName = other.itemName;
        description = other.description;
        quantity = other.quantity;
        baseWidth = other.baseWidth;
        baseHeight = other.baseHeight;
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
        return $"{itemName} ({Width}x{Height}) - {description}";
    }

    public void AssignInventory(ItemGrid newOwner, Vector2Int[]  newHeldPositions)
    {
        // this will be used to tell it its owning inventories
        _holdingGrid = newOwner;
        if (newHeldPositions.Length > 0)
        {


            _holdingGridPositions = new List<Vector2Int>();
            for (int i = 0; i < newHeldPositions.Length; i++)
            {
                _holdingGridPositions.Add(newHeldPositions[i]);
            }
        }
    }
    public void ClearOwningInventory()
    {
        // this should clear itself from its current inventories
        foreach(var item in _holdingGridPositions)
            _holdingGrid.ItemGridSpaces[item].heldItem = null;       
        
        //this should go through all the gridspaces in the holding inventory that holds this item and remove it
        
        // then no that all of its held positions have lost their reference to it.
        // then this itself should forget those positions and the grid they belong to
        _holdingGridPositions.Clear();
        _holdingGrid = null;
    }
}
