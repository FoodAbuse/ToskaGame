using System;
using System.Collections;
using System.Collections.Generic;
using Grids;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode toggleKey = KeyCode.I;

    public ItemGrid playerInventory;

    public int GridHeight;
    public int GridWidth;
    // Start is called before the first frame update

    // Update is called once per frame
    
    // replacing this script with UI controller.
    // UI controller will be able to work with or without references to a player inventory

    public void Start()
    {
        playerInventory = ItemGrid.Create(GridHeight, GridWidth); // creates the ItemGrid
        // this is temporary for testing , will be re written to account for loading an inventory later.
        
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            //UISystem.OpenInventory()
                // we have this under the UI system in case there are times when we register the Toggle key but
                // we do not want to open the inventory
        }
        // on I tell the InventoryManager to Open PlayerInventory
    }
}
