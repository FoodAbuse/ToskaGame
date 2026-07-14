using System.Collections.Generic;
using UnityEngine;
using Grids;

public class LootContainer : MonoBehaviour, IInteractable
{
    public ItemGrid containerGrid;
    public int width = 5;
    public int height = 5;
    public bool hasGeneratedLoot;
    
    
    public InventoryTemplate inventoryTemplate;

    
    [Header("Loot Generation")]
    [SerializeField] private LootGenerator lootGenerator = new LootGenerator();

    private void Awake()
    {
        containerGrid = ItemGrid.Create(height, width);
    }

    private void EnsureLootGenerated()
    {
        if (hasGeneratedLoot)
            return;

        if (lootGenerator != null && lootGenerator.HasRequiredData)
            PopulateLootFromTable();
        else
            PopulateSampleItems();

        hasGeneratedLoot = true;
    }

    private void PopulateLootFromTable()
    {
        List<ItemData> generatedLoot = lootGenerator.GenerateLoot();
        foreach (ItemData selectedItem in generatedLoot)
        {
            if (selectedItem == null)
                continue;

            InventoryItem inventoryItem = new InventoryItem(
                selectedItem.itemName,
                selectedItem.rarity.ToString(),
                //selectedItem.width,
                //selectedItem.height,
                selectedItem.Sprite,
                selectedItem
            );

            if (!containerGrid.AddItemToGrid(selectedItem))
                return;
        }
    }

    private void PopulateSampleItems()
    {
        // Example items, replace with your own logic
        //InventoryItem ammo = new InventoryItem("Ammo", "7.62 rounds", 1, 1);
        //InventoryItem medkit = new InventoryItem("Medkit", "Restores health", 2, 1);
        //containerGrid.PlaceItem(ammo, new GridPosition(0, 0));
        //containerGrid.PlaceItem(medkit, new GridPosition(2, 0));
    }

    public void Interact()
    {
        EnsureLootGenerated();
        UISystem.OpenContainerUI(inventoryTemplate, containerGrid);
        
        /*
        if (lootUIManager == null)
            lootUIManager = FindObjectOfType<LootUIManager>();

        if (lootUIManager != null)
            lootUIManager.OpenLoot(this);
        else
        */
           // Debug.LogWarning("LootUIManager not found in scene.");
    }
}