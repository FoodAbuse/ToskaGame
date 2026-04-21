using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WorldItem : MonoBehaviour, IInteractable
{
    [Header("Item")]
    [SerializeField] private ItemData itemData;
    [Min(1)]
    [SerializeField] private int quantity = 1;

    [Header("Target Inventory")]
    [SerializeField] private InventoryController playerInventory;

    private void Reset()
    {
        Collider worldCollider = GetComponent<Collider>();
        if (worldCollider != null)
            worldCollider.isTrigger = true;
    }

    public void Interact()
    {
        if (itemData == null)
        {
            Debug.LogWarning($"WorldItem '{name}' has no ItemData assigned.");
            return;
        }

        InventoryController inventoryController = ResolveInventoryController();
        if (inventoryController == null || inventoryController.InventoryGrid == null)
        {
            Debug.LogWarning($"No valid InventoryController found for WorldItem '{name}'.");
            return;
        }

        InventoryItem inventoryItem = CreateInventoryItem();
        bool added = inventoryController.InventoryGrid.TryAutoPlaceItem(inventoryItem, out _, out _);

        if (added)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"Could not pick up {itemData.itemName}: inventory has no space.");
        }
    }

    private InventoryController ResolveInventoryController()
    {
        if (playerInventory != null)
            return playerInventory;

        playerInventory = FindObjectOfType<InventoryController>();
        return playerInventory;
    }

    private InventoryItem CreateInventoryItem()
    {
        InventoryItem item = new InventoryItem(
            itemData.itemName,
            itemData.rarity.ToString(),
            itemData.width,
            itemData.height,
            itemData.icon,
            itemData
        );

        if (quantity > 1)
            item.AddQuantity(quantity - 1);

        return item;
    }
}
