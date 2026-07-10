using UnityEngine;

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Cursed,
}

public enum ItemType
{
    Misc,
    Consumable,
    Weapon,
    Armor,
    Material,
    QuestItem,
    Cursed,
    Upgrades,
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    public Sprite icon;

    [Header("Grid Size")]
    [Min(1)] public int width = 1;
    [Min(1)] public int height = 1;

    [Header("Rarity")]
    public ItemRarity rarity = ItemRarity.Common;

    [Header("Type")]
    public ItemType itemType = ItemType.Misc;
}
