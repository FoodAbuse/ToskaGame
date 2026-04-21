using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    private readonly List<InventoryItem> items = new List<InventoryItem>();

    public IReadOnlyList<InventoryItem> Items => items.AsReadOnly();

    public void AddItem(string itemName, string description, int amount = 1)
    {
        if (amount <= 0)
            return;

        InventoryItem existingItem = items.FirstOrDefault(i => i.itemName == itemName && i.description == description);
        if (existingItem != null)
        {
            existingItem.AddQuantity(amount);
        }
        else
        {
            items.Add(new InventoryItem(itemName, description, amount));
        }
    }

    public bool RemoveItem(string itemName, int amount = 1)
    {
        if (amount <= 0)
            return false;

        InventoryItem existingItem = items.FirstOrDefault(i => i.itemName == itemName);
        if (existingItem == null)
            return false;

        if (existingItem.quantity < amount)
            return false;

        existingItem.AddQuantity(-amount);

        if (existingItem.quantity == 0)
        {
            items.Remove(existingItem);
        }

        return true;
    }

    public int GetItemCount(string itemName)
    {
        InventoryItem existingItem = items.FirstOrDefault(i => i.itemName == itemName);
        return existingItem != null ? existingItem.quantity : 0;
    }

    public List<InventoryItem> ListAllItems()
    {
        return items.Select(item => new InventoryItem(item)).ToList();
    }
}
