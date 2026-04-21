using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventoryItem item;
    public GridUI gridUI;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPosition;
    public GridPosition originalGridPos;
    private bool isDragging = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        originalPosition = rectTransform.anchoredPosition;
        originalGridPos = inventoryGrid.GetItemPosition(item).Value;
        gridUI.SetDraggedItem(this);
        gridUI.CreateGhost(this);
        inventoryGrid.RemoveItem(item); // Remove from grid during drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // Invert Y axis to fix drag direction
        rectTransform.anchoredPosition += new Vector2(eventData.delta.x, -eventData.delta.y) / canvas.scaleFactor;

        // Update highlights on whichever grid the pointer is currently over.
        if (GridUI.TryGetGridUnderPointer(eventData, out GridUI hoveredGrid, out Vector2 localPoint))
        {
            GridUI.ClearAllHighlights();
            hoveredGrid.UpdateHighlights(item, localPoint);
        }
        else
        {
            GridUI.ClearAllHighlights();
        }

        // Handle rotation
        if (Input.GetKeyDown(KeyCode.R))
        {
            item.Rotate();
            rectTransform.sizeDelta = new Vector2(item.Width * gridUI.cellSize, item.Height * gridUI.cellSize);
            // Update ghost size if needed
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        // Drop handled in GridUI.OnDrop
    }

    private InventoryGrid inventoryGrid => gridUI.inventoryGrid;
}