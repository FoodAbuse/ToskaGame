using System;
using System.Collections;
using System.Collections.Generic;
using Grids;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode toggleKey = KeyCode.I;
    
    public KeyCode CloseUIKey = KeyCode.Escape;

    public ItemGrid playerInventory;            // the item grid

    public InventoryTemplate playerInventoryTemplate;   // the temp clas for handling Inventories prefabs and their slots

    public int InventoryHeight;
    public int InventoryWidth;
    
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            //InventoryManager.Instance.DisplayInventoryUI(playerInventory,InventoryUIPrefab);
            // we call the UISystem to Display the inventory. in case we are in a situation when we do not want the inventory to be openable
            //
            UISystem.OpenInventoryUI(playerInventoryTemplate,playerInventory);
        }

        if (Input.GetKeyDown(CloseUIKey))
        {
            UISystem.CloseAllUI();
        }
    }

    public void Start()
    {
        // telling the Inventory Grid to be this size if it isnt already
        //check the size of the inventory grid
        // check if the grid size is greater than 0 
        if (playerInventory == null)
        {
            playerInventory = ItemGrid.Create(InventoryHeight, InventoryWidth);
            Debug.Log("New player inventory Created. its size is = " + playerInventory.GetGridSize() +" or " + playerInventory.GetGridSpaceCount() +"spaces!");
        }
    }
}
