using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootGenerator
{
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private LootTable lootTable;
    [SerializeField] private LootGeneratorSettings settings;

    public bool HasRequiredData => itemDatabase != null && lootTable != null && settings != null;

    public List<ItemData> GenerateLoot()
    {
        List<ItemData> generatedLoot = new List<ItemData>();

        if (itemDatabase == null || lootTable == null || settings == null || itemDatabase.allItems == null)
            return generatedLoot;

        int minCount = settings.MinLootCount;
        int maxCount = settings.MaxLootCount;
        int itemsToGenerate = UnityEngine.Random.Range(minCount, maxCount + 1);

        for (int i = 0; i < itemsToGenerate; i++)
        {
            ItemRarity rolledRarity = lootTable.GetRandomRarity();
            ItemData selectedItem = GetRandomItemByRarity(rolledRarity);

            if (selectedItem == null)
                selectedItem = GetAnyRandomItem();

            if (selectedItem == null)
                break;

            generatedLoot.Add(selectedItem);
        }

        return generatedLoot;
    }

    private ItemData GetRandomItemByRarity(ItemRarity rarity)
    {
        int matchingCount = 0;
        foreach (ItemData item in itemDatabase.allItems)
        {
            if (item != null && item.rarity == rarity && settings.AllowsItemType(item.itemType))
                matchingCount++;
        }

        if (matchingCount == 0)
            return null;

        int selectionIndex = UnityEngine.Random.Range(0, matchingCount);
        int currentIndex = 0;

        foreach (ItemData item in itemDatabase.allItems)
        {
            if (item == null || item.rarity != rarity || !settings.AllowsItemType(item.itemType))
                continue;

            if (currentIndex == selectionIndex)
                return item;

            currentIndex++;
        }

        return null;
    }

    private ItemData GetAnyRandomItem()
    {
        int nonNullCount = 0;
        foreach (ItemData item in itemDatabase.allItems)
        {
            if (item != null && settings.AllowsItemType(item.itemType))
                nonNullCount++;
        }

        if (nonNullCount == 0)
            return null;

        int selectionIndex = UnityEngine.Random.Range(0, nonNullCount);
        int currentIndex = 0;

        foreach (ItemData item in itemDatabase.allItems)
        {
            if (item == null || !settings.AllowsItemType(item.itemType))
                continue;

            if (currentIndex == selectionIndex)
                return item;

            currentIndex++;
        }

        return null;
    }
}