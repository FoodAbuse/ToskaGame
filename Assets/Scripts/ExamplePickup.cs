using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ExamplePickup : MonoBehaviour, IInteractable
{
    [Header("Pickup")]
    public string itemName = "Health Potion";
    public string description = "Restores a small amount of health.";
    public int quantity = 1;
    public InventorySystem inventory;

    private void Reset()
    {
        Collider collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    public void Interact()
    {
        if (inventory != null)
        {
            inventory.AddItem(itemName, description, quantity);
            Debug.Log($"Picked up: {itemName} x{quantity} - {description}");

            // Log current inventory
            var items = inventory.ListAllItems();
            Debug.Log("Current Inventory:");
            foreach (var item in items)
            {
                Debug.Log($"  - {item.ToString()}");
            }
        }
        else
        {
            Debug.LogWarning("No InventorySystem assigned to ExamplePickup.");
        }

        gameObject.SetActive(false);
    }
}
