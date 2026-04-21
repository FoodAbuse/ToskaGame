using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public string description;
    public int quantity;
    public int baseWidth;
    public int baseHeight;
    public Sprite icon;
    public Object itemData;
    public bool IsRotated { get; private set; }

    public int Width => IsRotated ? baseHeight : baseWidth;
    public int Height => IsRotated ? baseWidth : baseHeight;

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

    public InventoryItem(string itemName, string description, int baseWidth, int baseHeight, Sprite icon = null, Object itemData = null)
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
}
