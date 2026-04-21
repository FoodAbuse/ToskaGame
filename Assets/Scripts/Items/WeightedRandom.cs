using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeightedItem<T>
{
    public T item;
    [Min(0)] public int weight;

    public WeightedItem(T item, int weight)
    {
        this.item = item;
        this.weight = weight;
    }
}

public static class WeightedRandom
{
    public static T GetRandomWeighted<T>(List<WeightedItem<T>> items)
    {
        if (items == null || items.Count == 0)
            throw new ArgumentException("Weighted items list cannot be null or empty.", nameof(items));

        int totalWeight = 0;
        foreach (WeightedItem<T> entry in items)
            totalWeight += Math.Max(0, entry.weight);

        if (totalWeight <= 0)
            throw new InvalidOperationException("At least one item must have a positive weight.");

        int roll = UnityEngine.Random.Range(0, totalWeight);
        int runningTotal = 0;

        foreach (WeightedItem<T> entry in items)
        {
            runningTotal += Math.Max(0, entry.weight);
            if (roll < runningTotal)
                return entry.item;
        }

        return items[items.Count - 1].item;
    }

    public static T GetRandomWeighted<TEntry, T>(IReadOnlyList<TEntry> items, Func<TEntry, int> weightSelector, Func<TEntry, T> valueSelector, T fallback)
    {
        if (items == null || items.Count == 0)
            return fallback;

        int totalWeight = 0;
        for (int i = 0; i < items.Count; i++)
            totalWeight += Math.Max(0, weightSelector(items[i]));

        if (totalWeight <= 0)
            return fallback;

        int roll = UnityEngine.Random.Range(0, totalWeight);
        int runningTotal = 0;

        for (int i = 0; i < items.Count; i++)
        {
            TEntry entry = items[i];
            runningTotal += Math.Max(0, weightSelector(entry));
            if (roll < runningTotal)
                return valueSelector(entry);
        }

        return fallback;
    }
}