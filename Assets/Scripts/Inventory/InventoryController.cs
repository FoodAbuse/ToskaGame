using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 5;
    public int gridHeight = 5;

    [Header("Controls")]
    public KeyCode toggleKey = KeyCode.I;

    [Header("Debug UI")]
    public bool showBasicGui = true;
    public Vector2 guiPosition = new Vector2(10, 10);
    public Vector2 cellSize = new Vector2(80, 40);
    public Vector2 itemLabelOffset = new Vector2(4, 4);

    public InventoryGrid InventoryGrid { get; private set; }
    public bool IsInventoryOpen { get; private set; }

    private List<InventoryItem> sampleItems = new List<InventoryItem>();
    private GridPosition selectedPosition;
    private InventoryItem draggingItem;
    private Vector2 dragOffset;

    private void Start()
    {
        InventoryGrid = new InventoryGrid(gridWidth, gridHeight);
        Debug.Log("InventoryController created InventoryGrid: " + (InventoryGrid != null));
        InitializeSampleItems();
        PlaceSampleItems();
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

    private void InitializeSampleItems()
    {
        sampleItems.Clear();
        sampleItems.Add(new InventoryItem("Medkit", "Restores health", 2, 1));
        sampleItems.Add(new InventoryItem("Ammo", "7.62 rounds", 1, 1));
        sampleItems.Add(new InventoryItem("Rifle", "Long weapon", 2, 3));
    }

    private void PlaceSampleItems()
    {
        if (InventoryGrid == null)
            return;

        InventoryGrid.PlaceItem(sampleItems[0], new GridPosition(0, 0));
        InventoryGrid.PlaceItem(sampleItems[1], new GridPosition(2, 0));
        InventoryGrid.PlaceItem(sampleItems[2], new GridPosition(0, 2));
    }
}
