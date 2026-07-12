using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "InventoryTemplate", menuName = "Scriptable Objects/Inventory/InventoryTemplate")]
public class InventoryTemplate : ScriptableObject
{
    // this will be a temporary class for handling the prefab elements of Inventories until a neater solution for the artists is finished
    

    public GameObject InventoryParent;
    public GameObject InventorySlotPrefab;
    
}

