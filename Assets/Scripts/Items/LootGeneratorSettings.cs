using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootGeneratorSettings", menuName = "Loot/Loot Generator Settings")]
public class LootGeneratorSettings : ScriptableObject
{
    [Header("Loot Count")]
    [Min(0)] [SerializeField] private int minLootCount;
    [Min(0)] [SerializeField] private int maxLootCount;
    [SerializeField, Tooltip("Relative chance for each count. X is normalized from the minimum count (0) to the maximum count (1); Y is the count's weight. Lower the curve toward 1 to make full containers rarer.")]
    private AnimationCurve lootCountWeight = AnimationCurve.Linear(0f, 1f, 1f, 0.1f);

    [Header("Optional Type Filter")]
    [SerializeField] private bool filterByItemType;
    [SerializeField] private List<ItemType> allowedItemTypes = new List<ItemType>();

    public int MinLootCount => Mathf.Max(0, minLootCount);

    public int MaxLootCount => Mathf.Max(MinLootCount, maxLootCount);

    public bool FilterByItemType => filterByItemType;

    public IReadOnlyList<ItemType> AllowedItemTypes => allowedItemTypes;

    public int GetRandomLootCount()
    {
        int minCount = MinLootCount;
        int maxCount = MaxLootCount;
        if (minCount == maxCount)
            return minCount;

        double totalWeight = 0d;
        for (int count = minCount; ; count++)
        {
            totalWeight += GetLootCountWeight(count, minCount, maxCount);
            if (count == maxCount)
                break;
        }

        if (totalWeight <= 0d)
        {
            Debug.LogError("Loot count weight curve must assign a positive weight to at least one count.", this);
            return minCount;
        }

        double roll = UnityEngine.Random.value * totalWeight;
        double accumulatedWeight = 0d;
        int lastWeightedCount = minCount;

        for (int count = minCount; ; count++)
        {
            float weight = GetLootCountWeight(count, minCount, maxCount);
            if (weight > 0f)
            {
                lastWeightedCount = count;
                accumulatedWeight += weight;
                if (roll < accumulatedWeight)
                    return count;
            }

            if (count == maxCount)
                break;
        }

        return lastWeightedCount;
    }

    private float GetLootCountWeight(int count, int minCount, int maxCount)
    {
        float normalizedCount = (float)(count - minCount) / (maxCount - minCount);
        float weight = lootCountWeight != null ? lootCountWeight.Evaluate(normalizedCount) : 0f;
        return float.IsNaN(weight) || float.IsInfinity(weight) ? 0f : Mathf.Max(0f, weight);
    }

    public bool AllowsItemType(ItemType itemType)
    {
        if (!filterByItemType)
            return true;

        return allowedItemTypes != null && allowedItemTypes.Contains(itemType);
    }
}
