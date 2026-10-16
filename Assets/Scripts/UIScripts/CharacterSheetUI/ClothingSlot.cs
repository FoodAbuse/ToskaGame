using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothingSlot : MonoBehaviour, IReceiver
{
    //[TODO] consider changing this from an ItemGridspaceInteractable to something new that doesnt rely on a Gridspace connection
    // since it can hold an item regardless of grid size
    private InventoryItem heldItem;
    public bool Receive(InventoryItem item, ItemGridSpaceInteractable origin)
    {
        if(item.itemData.GetType() == typeof(ClothingData))
        {
            // here is where we add the item data
            CharacterSheetData.PlayerData.equippedClothing = (ClothingData)item.itemData;
        }
        return false;
    }
}
