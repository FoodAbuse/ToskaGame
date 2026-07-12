using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WorldItem : MonoBehaviour, IInteractable
{
    [Header("Item")]
    [SerializeField] private ItemData itemData;
    [Min(1)]
    [SerializeField] private int quantity = 1;

    //[Header("Target Inventory")]
    private UIController _playerInventory;

    public UIController PlayerInventoryController
    {
        get
        {
            if (_playerInventory == null)
            {
                _playerInventory = FindObjectOfType<UIController>();
            }

            return _playerInventory;
        }
    }

    public void Interact()
    {
        if (itemData == null)
        {
            Debug.LogWarning($"WorldItem '{name}' has no ItemData assigned.");
            return;
        }

    


        // here we call Inventory that this is being interacted with and pass it the item and the ItemData its got
        //or attempt to anyway
        //yahoo!
        if (PlayerInventoryController.playerInventory.AddItemToGrid(itemData))
        {
            Destroy(gameObject); // attempt to add item. if it does. then neck this. lol. lmao. ecks Dee
        }
    }

    public static void Drop(Vector3 dropPos, Quaternion rotation, ItemData itemData)
    {
        Instantiate(itemData.WorldItemPrefab,dropPos,rotation);
    }

 

    
}

