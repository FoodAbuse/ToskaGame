using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRetrieverSlot : MonoBehaviour, IReceiver
{
    //[TODO] consider changing this from an ItemGridspaceInteractable to something new that doesnt rely on a Gridspace connection
    // since it can hold an item regardless of grid size
    private InventoryItem heldItem;
    private CharacterSheetGremlin owningCharacterSheet;
    public bool Receive(InventoryItem item, ItemGridSpaceInteractable origin)
    {
        if(item.itemData.GetType() == typeof(WeaponData))
        {
            // here is where we add the item data
            //if(CharacterSheetData.PlayerData.AddMeleeWeapon)
            
        }
        return false;
    }

    public void ConnectReferences(CharacterSheetGremlin owner)
    {
        owningCharacterSheet = owner;
    }
}
