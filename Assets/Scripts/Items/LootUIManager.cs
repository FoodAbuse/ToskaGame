using UnityEngine;


public class LootUIManager : MonoBehaviour
{
    public InventoryController playerInventory;
    public float gridSpacing = 40f;
    public KeyCode closeLootKey = KeyCode.Escape;

    [Header("Loot Transfer")]
    public bool autoPlaceInPlayerInventory = true;
    public float feedbackDuration = 2.5f;

    [Header("Debug UI")]
    public bool showBasicGui = true;
    public Vector2 guiPosition = new Vector2(10, 220);
    public Vector2 cellSize = new Vector2(80, 40);
    public Vector2 itemLabelOffset = new Vector2(4, 4);

    private bool lootOpen = false;
    private LootContainer currentContainer = null;
    private int lootOpenedFrame = -1;

    private InventoryItem draggingItem;
    private InventoryGrid draggingSourceGrid;
    private GridPosition draggingSourcePosition;
    private Vector2 dragOffset;
    private string feedbackMessage = string.Empty;
    private float feedbackHideTime;

    private void Update()
    {
        // Ignore close input on the exact frame loot opened.
        if (lootOpen && Time.frameCount > lootOpenedFrame && Input.GetKeyDown(closeLootKey))
        {
            CloseLoot();
        }
    }

    public void OpenLoot(LootContainer container)
    {
        // Toggle loot window if already open for this container
        if (lootOpen && currentContainer == container)
        {
            CloseLoot();
            return;
        }

        // If loot is open for another container, close it first
        if (lootOpen)
        {
            CloseLoot();
        }

        if (playerInventory == null || playerInventory.InventoryGrid == null)
        {
            Debug.LogError("Player inventory not assigned or not initialized in LootUIManager");
            return;
        }
        if (container == null || container.containerGrid == null)
        {
            Debug.LogError("Loot container not set up");
            return;
        }

        lootOpen = true;
        currentContainer = container;
        lootOpenedFrame = Time.frameCount;
    }

    private void OnGUI()
    {
        if (!showBasicGui || !lootOpen || currentContainer == null || playerInventory == null)
            return;

        InventoryGrid playerGrid = playerInventory.InventoryGrid;
        InventoryGrid containerGrid = currentContainer.containerGrid;
        if (playerGrid == null || containerGrid == null)
            return;

        Rect playerPanelRect = GetPanelRect(playerGrid, guiPosition);
        Vector2 containerPanelPos = new Vector2(playerPanelRect.xMax + gridSpacing, guiPosition.y);
        Rect containerPanelRect = GetPanelRect(containerGrid, containerPanelPos);

        Event e = Event.current;
        Vector2 mousePos = e.mousePosition;

        if (e.type == EventType.MouseDown && e.button == 0 && draggingItem == null)
        {
            if (TryBeginDragFromGrid(playerGrid, playerPanelRect, mousePos) || TryBeginDragFromGrid(containerGrid, containerPanelRect, mousePos))
            {
                e.Use();
            }
        }

        if (e.type == EventType.MouseDown && e.button == 1)
        {
            if (TryRotateItemInGrid(playerGrid, playerPanelRect, mousePos) || TryRotateItemInGrid(containerGrid, containerPanelRect, mousePos))
            {
                e.Use();
            }
        }

        if (e.type == EventType.MouseUp && e.button == 0 && draggingItem != null)
        {
            bool placed = TryPlaceDraggedItem(playerGrid, playerPanelRect, mousePos) || TryPlaceDraggedItem(containerGrid, containerPanelRect, mousePos);
            if (!placed)
            {
                if (string.IsNullOrEmpty(feedbackMessage) || Time.time >= feedbackHideTime)
                {
                    ShowFeedback("Invalid placement. Item returned to original slot.");
                }
                draggingItem = null;
                draggingSourceGrid = null;
            }
            e.Use();
        }

        DrawGridPanel("Player Inventory", playerGrid, playerPanelRect);
        DrawGridPanel("Container", containerGrid, containerPanelRect);

        if (draggingItem != null)
        {
            GUI.Label(
                new Rect(mousePos.x - dragOffset.x + itemLabelOffset.x, mousePos.y - dragOffset.y + itemLabelOffset.y, cellSize.x * draggingItem.Width, cellSize.y * draggingItem.Height),
                draggingItem.itemName);
        }

        float hintY = Mathf.Max(playerPanelRect.yMax, containerPanelRect.yMax) + 6f;
        GUI.Label(new Rect(guiPosition.x, hintY, 560, 60), "Drag items with left mouse to move or transfer.\nRight click item to rotate.\nPress Esc to close loot.");

        if (!string.IsNullOrEmpty(feedbackMessage) && Time.time < feedbackHideTime)
        {
            GUI.Label(new Rect(guiPosition.x, hintY + 48f, 560, 24), feedbackMessage);
        }
    }

    private Rect GetPanelRect(InventoryGrid grid, Vector2 panelPos)
    {
        float width = grid.Width * cellSize.x + 20f;
        float height = grid.Height * cellSize.y + 40f;
        return new Rect(panelPos.x, panelPos.y, width, height);
    }

    private void DrawGridPanel(string title, InventoryGrid grid, Rect panelRect)
    {
        GUI.Box(panelRect, title);

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                Rect cellRect = GetCellRect(panelRect, x, y);
                GUI.Box(cellRect, "");

                InventoryItem item = grid.Cells[x, y];
                if (item != null && item != draggingItem)
                {
                    GUI.Label(new Rect(cellRect.x + itemLabelOffset.x, cellRect.y + itemLabelOffset.y, cellSize.x - 8, cellSize.y - 8), item.itemName);
                }
            }
        }
    }

    private Rect GetCellRect(Rect panelRect, int x, int y)
    {
        return new Rect(panelRect.x + 10 + x * cellSize.x, panelRect.y + 25 + y * cellSize.y, cellSize.x, cellSize.y);
    }

    private bool TryBeginDragFromGrid(InventoryGrid grid, Rect panelRect, Vector2 mousePos)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                Rect cellRect = GetCellRect(panelRect, x, y);
                if (!cellRect.Contains(mousePos))
                    continue;

                InventoryItem item = grid.Cells[x, y];
                if (item == null)
                    return false;

                draggingItem = item;
                draggingSourceGrid = grid;
                draggingSourcePosition = grid.GetItemPosition(item).Value;
                dragOffset = mousePos - new Vector2(cellRect.x, cellRect.y);
                return true;
            }
        }

        return false;
    }

    private bool TryRotateItemInGrid(InventoryGrid grid, Rect panelRect, Vector2 mousePos)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                Rect cellRect = GetCellRect(panelRect, x, y);
                if (!cellRect.Contains(mousePos))
                    continue;

                InventoryItem item = grid.Cells[x, y];
                if (item == null)
                    return false;

                return grid.RotateItem(item);
            }
        }

        return false;
    }

    private bool TryPlaceDraggedItem(InventoryGrid targetGrid, Rect targetPanelRect, Vector2 mousePos)
    {
        if (draggingItem == null || draggingSourceGrid == null)
            return false;

        for (int x = 0; x < targetGrid.Width; x++)
        {
            for (int y = 0; y < targetGrid.Height; y++)
            {
                Rect cellRect = GetCellRect(targetPanelRect, x, y);
                if (!cellRect.Contains(mousePos))
                    continue;

                GridPosition targetPosition = new GridPosition(x, y);
                bool placed;
                bool toPlayerInventory = targetGrid == playerInventory.InventoryGrid;

                if (targetGrid == draggingSourceGrid)
                {
                    placed = targetGrid.MoveItem(draggingItem, targetPosition);
                    if (!placed)
                    {
                        ShowFeedback($"{draggingItem.itemName} does not fit there.");
                    }
                }
                else
                {
                    GridPosition sourcePosition = draggingSourcePosition;
                    GridPosition finalTargetPosition = targetPosition;
                    bool rotateForPlacement = false;
                    bool usedAutoPlacement = false;

                    if (toPlayerInventory && autoPlaceInPlayerInventory && !targetGrid.CanPlaceItem(draggingItem, targetPosition))
                    {
                        if (targetGrid.TryFindFirstAvailablePlacement(draggingItem, out GridPosition firstAvailable, out rotateForPlacement))
                        {
                            finalTargetPosition = firstAvailable;
                            usedAutoPlacement = true;
                        }
                        else
                        {
                            ShowFeedback($"Inventory full. Cannot pick up {draggingItem.itemName}.");
                            return false;
                        }
                    }

                    if (!targetGrid.CanPlaceItem(draggingItem, finalTargetPosition))
                    {
                        if (toPlayerInventory && !targetGrid.HasSpaceFor(draggingItem))
                        {
                            ShowFeedback($"Inventory full. Cannot pick up {draggingItem.itemName}.");
                        }
                        else
                        {
                            ShowFeedback($"{draggingItem.itemName} does not fit there.");
                        }

                        return false;
                    }

                    if (!draggingSourceGrid.RemoveItem(draggingItem))
                    {
                        placed = false;
                    }
                    else
                    {
                        placed = targetGrid.PlaceItem(draggingItem, finalTargetPosition, rotateForPlacement);
                        if (!placed)
                        {
                            draggingSourceGrid.PlaceItem(draggingItem, sourcePosition);
                            ShowFeedback($"Failed to move {draggingItem.itemName}. Item returned.");
                        }
                        else if (toPlayerInventory && usedAutoPlacement)
                        {
                            string placementMessage = rotateForPlacement
                                ? $"Auto-placed and rotated {draggingItem.itemName} to fit."
                                : $"Placed {draggingItem.itemName} in first available slot.";
                            ShowFeedback(placementMessage, false);
                        }
                        else
                        {
                            ShowFeedback($"Moved {draggingItem.itemName}.", false);
                        }
                    }
                }

                draggingItem = null;
                draggingSourceGrid = null;
                return placed;
            }
        }

        return false;
    }

    private void ShowFeedback(string message, bool warning = true)
    {
        feedbackMessage = message;
        feedbackHideTime = Time.time + Mathf.Max(0.25f, feedbackDuration);

        if (warning)
            Debug.LogWarning($"[Loot] {message}");
        else
            Debug.Log($"[Loot] {message}");
    }

    public void CloseLoot()
    {
        draggingItem = null;
        draggingSourceGrid = null;
        lootOpen = false;
        currentContainer = null;
        lootOpenedFrame = -1;
    }
}