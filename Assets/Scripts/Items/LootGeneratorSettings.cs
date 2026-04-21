using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootGeneratorSettings", menuName = "Loot/Loot Generator Settings")]
public class LootGeneratorSettings : ScriptableObject
{
    [Header("Loot Count")]
    [Min(0)] [SerializeField] private int minLootCount;
    [Min(0)] [SerializeField] private int maxLootCount;

    [Header("Optional Type Filter")]
    [SerializeField] private bool filterByItemType;
    [SerializeField] private List<ItemType> allowedItemTypes = new List<ItemType>();

    public int MinLootCount => Mathf.Max(0, minLootCount);

    public int MaxLootCount => Mathf.Max(MinLootCount, maxLootCount);

    public bool FilterByItemType => filterByItemType;

    public IReadOnlyList<ItemType> AllowedItemTypes => allowedItemTypes;

    public bool AllowsItemType(ItemType itemType)
    {
        if (!filterByItemType)
            return true;

        return allowedItemTypes != null && allowedItemTypes.Contains(itemType);
    }
}
