using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [Tooltip("All possible items available in the game.")]
    public List<ItemData> allItems = new List<ItemData>();
}
