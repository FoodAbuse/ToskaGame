using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public abstract class InventorySlot : InteractableComponent
{
    // this will be the abstract class for inventory slots and whatever they do...
    // a basic inventory slot will need to be interactable.
    // it will need to display something I.e update
    // it may not need to recieve something
    //
    [SerializeField]
    //protected Inventory inventoryParent; 
    //public Inventory InventoryParent{get{return inventoryParent;}}
    public FloatVariable interactRange;   //
    
    protected int _index = 0;
    public int index => _index;
    public int Index{get{return _index;}}
    
    public virtual void OnInteract(GameObject user)
    { 
        
    }

    public virtual void UpdateSlot()
    {
    
    }

    //public virtual Inventory GetInvParent()
    //{
    //    return inventoryParent;
   // }
}

