using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GridUI : MonoBehaviour, IDropHandler
{
    public InventoryGrid inventoryGrid;
    public GameObject cellPrefab; // UI Image for cell
    public GameObject itemPrefab; // UI Image for item
    public int cellSize = 50;
    public Color validColor = Color.green;
    public Color invalidColor = Color.red;
    public Color defaultColor = Color.white;

    private RectTransform rectTransform;
    private DraggableItemUI draggedItem;
    private GameObject ghostItem;
    private Image[] cellImages;

    private static readonly List<GridUI> activeGrids = new List<GridUI>();
    private static DraggableItemUI globallyDraggedItem;

    private void OnEnable()
    {
        if (!activeGrids.Contains(this))
        {
            activeGrids.Add(this);
        }
    }

    private void OnDisable()
    {
        activeGrids.Remove(this);
        if (globallyDraggedItem != null && globallyDraggedItem.gridUI == this)
        {
            globallyDraggedItem = null;
        }
    }

    private void Start()
    {
        if (inventoryGrid == null)
        {
            Debug.LogError("InventoryGrid not assigned to GridUI");
            return;
        }
        rectTransform = GetComponent<RectTransform>();
        CreateGrid();
        PopulateItems();
    }

    private void CreateGrid()
    {
        cellImages = new Image[inventoryGrid.Width * inventoryGrid.Height];
        rectTransform.sizeDelta = new Vector2(inventoryGrid.Width * cellSize, inventoryGrid.Height * cellSize);

        for (int x = 0; x < inventoryGrid.Width; x++)
        {
            for (int y = 0; y < inventoryGrid.Height; y++)
            {
                GameObject cell = Instantiate(cellPrefab, transform);
                RectTransform cellRect = cell.GetComponent<RectTransform>();
                if (cellRect == null)
                {
                    cellRect = cell.AddComponent<RectTransform>();
                }
                cellRect.anchoredPosition = new Vector2(x * cellSize, -y * cellSize);
                cellRect.sizeDelta = new Vector2(cellSize, cellSize);
                cellImages[GetCellIndex(x, y)] = cell.GetComponent<Image>();
            }
        }
    }

    private void PopulateItems()
    {
        foreach (var item in inventoryGrid.Items)
        {
            GridPosition pos = inventoryGrid.GetItemPosition(item).Value;
            CreateItemUI(item, pos);
        }
    }

    private void CreateItemUI(InventoryItem item, GridPosition pos)
    {
        GameObject itemGO = Instantiate(itemPrefab, transform);
        DraggableItemUI draggable = itemGO.GetComponent<DraggableItemUI>();
        if (draggable == null)
        {
            draggable = itemGO.AddComponent<DraggableItemUI>();
        }
        draggable.item = item;
        draggable.gridUI = this;
        RectTransform itemRect = itemGO.GetComponent<RectTransform>();
        itemRect.anchorMin = new Vector2(0, 1);
        itemRect.anchorMax = new Vector2(0, 1);
        itemRect.pivot = new Vector2(0, 1);
        itemRect.anchoredPosition = new Vector2(pos.X * cellSize, -pos.Y * cellSize);
        itemRect.sizeDelta = new Vector2(item.Width * cellSize, item.Height * cellSize);
        Image img = itemGO.GetComponent<Image>();
        if (img != null && item.icon != null)
        {
            img.sprite = item.icon;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItemUI itemToDrop = draggedItem != null ? draggedItem : globallyDraggedItem;
        if (itemToDrop != null)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint);
            int x = Mathf.FloorToInt(localPoint.x / cellSize);
            int y = Mathf.FloorToInt(-localPoint.y / cellSize);
            GridPosition dropPos = new GridPosition(x, y);

            // If dragging from another grid, allow transfer
            bool isCrossGrid = itemToDrop.gridUI != this;
            InventoryGrid sourceGrid = itemToDrop.gridUI.inventoryGrid;
            InventoryGrid targetGrid = inventoryGrid;
            GridUI sourceGridUI = itemToDrop.gridUI;
            GridPosition finalDropPos = dropPos;
            bool rotateForPlacement = false;

            if (!targetGrid.CanPlaceItem(itemToDrop.item, dropPos) && isCrossGrid)
            {
                targetGrid.TryFindFirstAvailablePlacement(itemToDrop.item, out finalDropPos, out rotateForPlacement);
            }

            if (targetGrid.CanPlaceItem(itemToDrop.item, finalDropPos) || rotateForPlacement)
            {
                targetGrid.PlaceItem(itemToDrop.item, finalDropPos, rotateForPlacement);
                itemToDrop.transform.SetParent(transform);
                RectTransform droppedRect = itemToDrop.GetComponent<RectTransform>();
                droppedRect.anchoredPosition = new Vector2(finalDropPos.X * cellSize, -finalDropPos.Y * cellSize);
                droppedRect.sizeDelta = new Vector2(itemToDrop.item.Width * cellSize, itemToDrop.item.Height * cellSize);
                // Update gridUI reference for cross-grid move
                if (isCrossGrid)
                {
                    itemToDrop.gridUI = this;
                }
            }
            else
            {
                // Return to original grid
                sourceGrid.PlaceItem(itemToDrop.item, itemToDrop.originalGridPos);
                itemToDrop.transform.SetParent(sourceGridUI.transform);
                itemToDrop.GetComponent<RectTransform>().anchoredPosition = new Vector2(itemToDrop.originalGridPos.X * sourceGridUI.cellSize, -itemToDrop.originalGridPos.Y * sourceGridUI.cellSize);
            }

            ClearAllHighlights();
            sourceGridUI.ResetDragVisuals();
            if (sourceGridUI != this)
            {
                ResetDragVisuals();
            }
            globallyDraggedItem = null;
        }
    }

    public void UpdateHighlights(InventoryItem item, Vector2 localPos)
    {
        ClearHighlights();
        int startX = Mathf.FloorToInt(localPos.x / cellSize);
        int startY = Mathf.FloorToInt(-localPos.y / cellSize);
        GridPosition pos = new GridPosition(startX, startY);

        bool valid = inventoryGrid.CanPlaceItem(item, pos);
        Color color = valid ? validColor : invalidColor;

        for (int x = 0; x < item.Width; x++)
        {
            for (int y = 0; y < item.Height; y++)
            {
                int cx = startX + x;
                int cy = startY + y;
                if (cx >= 0 && cx < inventoryGrid.Width && cy >= 0 && cy < inventoryGrid.Height)
                {
                    int index = GetCellIndex(cx, cy);
                    cellImages[index].color = color;
                }
            }
        }
    }

    private void ClearHighlights()
    {
        foreach (var img in cellImages)
        {
            img.color = defaultColor;
        }
    }

    public void SetDraggedItem(DraggableItemUI item)
    {
        draggedItem = item;
        globallyDraggedItem = item;
    }

    public void CreateGhost(DraggableItemUI original)
    {
        ghostItem = Instantiate(original.gameObject, transform);
        Image ghostImg = ghostItem.GetComponent<Image>();
        ghostImg.color = new Color(1, 1, 1, 0.5f);
        ghostItem.GetComponent<DraggableItemUI>().enabled = false;
    }

    public void ResetDragVisuals()
    {
        draggedItem = null;
        if (ghostItem != null)
        {
            Destroy(ghostItem);
            ghostItem = null;
        }
        ClearHighlights();
    }

    public static bool TryGetGridUnderPointer(PointerEventData eventData, out GridUI hitGrid, out Vector2 localPoint)
    {
        for (int i = activeGrids.Count - 1; i >= 0; i--)
        {
            GridUI grid = activeGrids[i];
            if (grid == null || !grid.gameObject.activeInHierarchy)
            {
                continue;
            }

            RectTransform gridRect = grid.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(gridRect, eventData.position, eventData.pressEventCamera))
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRect, eventData.position, eventData.pressEventCamera, out localPoint);
                hitGrid = grid;
                return true;
            }
        }

        hitGrid = null;
        localPoint = Vector2.zero;
        return false;
    }

    public static void ClearAllHighlights()
    {
        foreach (GridUI grid in activeGrids)
        {
            if (grid != null)
            {
                grid.ClearHighlights();
            }
        }
    }

    private int GetCellIndex(int x, int y)
    {
        return (x * inventoryGrid.Height) + y;
    }

}