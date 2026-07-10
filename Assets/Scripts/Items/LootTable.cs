using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLootTable", menuName = "Loot/Loot Table")]
public class LootTable : ScriptableObject
{
    [Serializable]
    public class RarityWeight
    {
        public ItemRarity rarity;
        [Min(0)] public int weight;
    }

    [Header("Rarity Weights")]
    [SerializeField] private List<RarityWeight> rarityWeights = new List<RarityWeight>
    {
        new RarityWeight { rarity = ItemRarity.Common, weight = 40 },
        new RarityWeight { rarity = ItemRarity.Uncommon, weight = 35 },
        new RarityWeight { rarity = ItemRarity.Rare, weight = 15 },
        new RarityWeight { rarity = ItemRarity.Epic, weight = 5 },
        new RarityWeight { rarity = ItemRarity.Legendary, weight = 2 },
        new RarityWeight { rarity = ItemRarity.Cursed, weight = 3 }
    };

    public IReadOnlyList<RarityWeight> RarityWeights => rarityWeights;

    public ItemRarity GetRandomRarity()
    {
        if (rarityWeights == null || rarityWeights.Count == 0)
            return default;

        ItemRarity fallbackRarity = rarityWeights[0].rarity;

        ItemRarity rarity = WeightedRandom.GetRandomWeighted(
            rarityWeights,
            entry => entry.weight,
            entry => entry.rarity,
            fallbackRarity
        );

        if (GetWeight(rarity) <= 0)
            Debug.LogWarning("LootTable has no positive rarity weights. Falling back to the first configured rarity.");

        return rarity;
    }

    public int GetWeight(ItemRarity rarity)
    {
        foreach (RarityWeight entry in rarityWeights)
        {
            if (entry.rarity == rarity)
                return Math.Max(0, entry.weight);
        }

        return 0;
    }
}