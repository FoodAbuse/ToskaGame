using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    
    [FormerlySerializedAs("allItems")] [Tooltip("All possible items available in the game.")]
    public List<ItemData> itemList = new List<ItemData>();


}

