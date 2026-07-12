using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGridSpaceInteractable : InteractableComponent, IReceiver
{
    // this will be the Interactable button or game Object counterpart to a Grid space.
    // when a grid space is allocated to a GameObject of a slot it will add this component and keep a reference to it
    // this will too keep a Reference to its corresponding grid space
    //
    
    private ItemGridSpace _itemgridSpace; // the owning gridspace
    public ItemGridSpace ItemGridSpace => _itemgridSpace;

    private float InteractRange => UIInteractor.Instance.InteractRange;

    private ItemData heldItem => ItemGridSpace.heldItem.itemData;

    public override void Interact()
    {
        if (ItemGridSpace.HasItem()) // check the slot is holding an item
        {
            var mouserItem = new GameObject();
            var mouser = mouserItem.AddComponent<MouseFollower>();
            //mouser.interactRange = InteractRange; changed Mousers to refer to UIInteractors interact range
            mouser.movingItemData = heldItem;
            mouser.CreateSpriteChild(heldItem);
            mouser.CreateItemChild(heldItem);
            mouser.itemOrigin = this;
            //then we gotta clear the slot
            ClearItem();
        }
        Debug.Log(ItemGridSpace.GridPosition);
    }

    public bool Receive(ItemData itemData, ItemGridSpaceInteractable origin)
    {
        // this will be called when the Object is recieve
        //origin is the component the Itemdata came from I guess. 
        Debug.Log( "Recieving" + itemData.itemName);
        return ItemGridSpace.AddItem(itemData);
    }

    public void SetItemGridSpace(ItemGridSpace itemGridSpace)
    {
        _itemgridSpace = itemGridSpace;
    }
    public void ClearItem()
    {
        
        //inventoryParent.ClearItem(index);
        //InventoryParent.inventoryUpdate.Raise();
        _itemgridSpace.heldItem.ClearOwningInventory();
        UISystem.UpdateInventoryUI.Raise();
        Debug.Log("Item Cleared");
    }
}
