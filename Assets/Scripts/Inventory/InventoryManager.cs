using System;
using System.Collections.Generic;
using Grids;
using UnityEngine;


public class InventoryManager : ScriptableObject
{

    [Header("Debug UI")]
    public bool showBasicGui = true;
    public Vector2 guiPosition = new Vector2(10, 10);
    public Vector2 cellSize = new Vector2(80, 40);
    public Vector2 itemLabelOffset = new Vector2(4, 4);


    
    private GridPosition selectedPosition;
    private InventoryItem draggingItem;
    private Vector2 dragOffset;

    private static InventoryManager _instance;
    
    //Runtime set of Inventory Screens Here.

    public static InventoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new InventoryManager();
            }
            return _instance;
        }
    }
    

    enum InventType // this will just be used to determine which segment of the screen the UI will open to by default may be a child class later
    {
        Player,Looting,Other
    }
    
    // this class will handle the Display and manipulation of All inventory Grids. :O


    public void DisplayInventoryUI(ItemGrid grid, GameObject InventoryPrefab)
    {
        // this will go through all of its grid spaces and create a list of its held Items
        // then it will Open a UI prefab for the inventory and draw the sprites of the Prefab at those positions
        // the ui prefab will assign itself to a runtime set so other things can Know that the players Inventory is open
        List<InventoryItem> itemList = grid.GetHeldItems();
        // then we will open the UI prefab here, IE create the prefab under a canvas
    }
    
    


    /*private void Start()
    {
        InventoryGrid = new InventoryGrid(gridWidth, gridHeight);
        Debug.Log("InventoryController created InventoryGrid: " + (InventoryGrid != null));
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            IsInventoryOpen = !IsInventoryOpen;
            Debug.Log($"Inventory toggled: {IsInventoryOpen}");
        }
    }

    private void OnGUI()
    {
        if (!showBasicGui || !IsInventoryOpen)
            return;

        Rect panelRect = new Rect(guiPosition.x, guiPosition.y, gridWidth * cellSize.x + 20, gridHeight * cellSize.y + 80);
        GUI.Box(panelRect, "Inventory");

        Event e = Event.current;
        Vector2 mousePos = e.mousePosition;

        // Handle drag start
        if (e.type == EventType.MouseDown && e.button == 0 && draggingItem == null)
        {
            // check if the current event is a  Mouse click
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Rect cellRect = new Rect(guiPosition.x + 10 + x * cellSize.x, guiPosition.y + 25 + y * cellSize.y, cellSize.x, cellSize.y);
                    if (cellRect.Contains(mousePos))
                    {
                        InventoryItem item = InventoryGrid.Cells[x, y];
                        if (item != null)
                        {
                            draggingItem = item;
                            dragOffset = mousePos - new Vector2(cellRect.x, cellRect.y);
                            selectedPosition = new GridPosition(x, y);
                            Debug.Log($"Started dragging {item.itemName}");
                        }
                        break;
                    }
                }
            }
        }

        // Handle right-click rotation
        if (e.type == EventType.MouseDown && e.button == 1)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Rect cellRect = new Rect(guiPosition.x + 10 + x * cellSize.x, guiPosition.y + 25 + y * cellSize.y, cellSize.x, cellSize.y);
                    if (cellRect.Contains(mousePos))
                    {
                        InventoryItem item = InventoryGrid.Cells[x, y];
                        if (item != null)
                        {
                            InventoryGrid.RotateItem(item);
                            Debug.Log($"Rotated {item.itemName}");
                        }
                        break;
                    }
                }
            }
        }

        // Handle drag end
        if (e.type == EventType.MouseUp && e.button == 0 && draggingItem != null)
        {
            bool placed = false;
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Rect cellRect = new Rect(guiPosition.x + 10 + x * cellSize.x, guiPosition.y + 25 + y * cellSize.y, cellSize.x, cellSize.y);
                    if (cellRect.Contains(mousePos))
                    {
                        GridPosition newPos = new GridPosition(x, y);
                        if (newPos.Equals(selectedPosition))
                        {
                            // Dropped back to original position
                            placed = true;
                        }
                        else
                        {
                            placed = InventoryGrid.MoveItem(draggingItem, newPos);
                        }
                        break;
                    }
                }
            }
            if (placed)
            {
                Debug.Log($"Dropped {draggingItem.itemName} successfully");
            }
            else
            {
                Debug.Log($"Failed to drop {draggingItem.itemName}");
            }
            draggingItem = null;
        }

        // Draw grid
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Rect cellRect = new Rect(guiPosition.x + 10 + x * cellSize.x, guiPosition.y + 25 + y * cellSize.y, cellSize.x, cellSize.y);
                GUI.Box(cellRect, "");

                InventoryItem item = InventoryGrid.Cells[x, y];
                if (item != null && item != draggingItem)
                {
                    GUI.Label(new Rect(cellRect.x + itemLabelOffset.x, cellRect.y + itemLabelOffset.y, cellSize.x - 8, cellSize.y - 8), item.itemName);
                }
            }
        }

        // Draw dragged item
        if (draggingItem != null)
        {
            GUI.Label(new Rect(mousePos.x - dragOffset.x + itemLabelOffset.x, mousePos.y - dragOffset.y + itemLabelOffset.y, cellSize.x - 8, cellSize.y - 8), draggingItem.itemName);
        }

        Rect hintRect = new Rect(guiPosition.x + 10, guiPosition.y + gridHeight * cellSize.y + 35, 400, 60);
        GUI.Label(hintRect, "Drag items with left mouse to move.\nRight click item to rotate.\nPress I to close inventory.");
    }
    */
}
