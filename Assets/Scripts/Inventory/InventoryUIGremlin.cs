using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Grids;
using Unity.VisualScripting;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class InventoryUIGremlin : UIReporter
{
    //DONT LLOOK AT THIS. IT IS MY SHAME
    public GameObject InventoryPanel;
    private GameObject _slotsParent;                      // these are just here to keep the
    private GameObject _itemsParent;
    private GameObject _UIContainer;
    public GameObject InventorySlotPrefab;
    public float paddingWidth;
    public float paddingHeight;
    private ItemGrid _itemGrid;
    private Vector2 _screenOffset;

    public Vector2 ScreenOffset
    {
        get { return _screenOffset; }
        set
        {
            switch (value.x)
            {
                case > 1:
                    _screenOffset.x = 1;
                    break;
                case  < -1:
                    _screenOffset.x = -1;
                    break;
                default:
                    _screenOffset.x = value.x;
                    break;
            }

            switch (value.y)
            {
                case > 1:
                    _screenOffset.y = 1;
                    break;
                case < -1:
                    _screenOffset.y = -1;
                    break;
                default:
                    _screenOffset.y = value.y;
                    break;
            }
        }
    }
    

    private GameEventListener _InventoryUpdateListener; // this is the GameEvent listener that will tell this bozo to update its sprites
    


    //public List<InventoryItem> heldItems = new List<InventoryItem>();
    
    public Dictionary<InventoryItem, UIItemSpriteGremlin> HeldItemSprites = new Dictionary<InventoryItem, UIItemSpriteGremlin>();
    public Dictionary<GridSpace, ItemGridSpaceInteractable> GridSpaceInteractables = new Dictionary<GridSpace, ItemGridSpaceInteractable>();

    public void StartGremlin(ItemGrid inventoryGrid, Vector2 newPanelOffset)
    {
        ScreenOffset = newPanelOffset;  // have to do this here since we cant have a constructor on a Monobehaviour ;-; will probably move all this to a handler that instantiates this
        _itemGrid = inventoryGrid;
        
        _UIContainer = new GameObject("UIContainer");
        _UIContainer.transform.SetParent(gameObject.transform);
        RectTransform uiRect = _UIContainer.AddComponent<RectTransform>();
        _slotsParent = new GameObject("Slot Parent");
        _slotsParent.transform.SetParent(_UIContainer.transform,false);
        _itemsParent = new GameObject("Items Parent");
        _itemsParent.transform.SetParent(_UIContainer.transform,false);

        InventoryPanel.transform.SetParent(_slotsParent.transform,false);
        RectTransform ipRect = InventoryPanel.GetComponent<RectTransform>();
        RectTransform slotFabRect = InventorySlotPrefab.GetComponent<RectTransform>();
        ipRect.localScale = 
          UISystem.ScaleToSize2(ipRect.localScale,
            UISystem.SizeToFitGrid(InventorySlotPrefab, inventoryGrid.GetGridSize() + new Vector2Int(1,1), paddingWidth, paddingHeight),
              ipRect.sizeDelta);        // we add 1 to the getGridsize to account for 0,0 being 1 slot

        // here we will handle positioning the panels 
        // for now we will use a switch and an int. In the future Id like to change to a float for other positions (maybe a V2 for vertical positioning)
        Rect areaRect = Screen.safeArea;
        // perform calculation to return screen offset scaled into the Safe area -1 far left 0 mid, 1 max
        Vector2 targetPosition;
        targetPosition.x = ((ScreenOffset.x - -1) * (areaRect.xMax - areaRect.xMin)) / 2 + 0;
        targetPosition.y = ((ScreenOffset.y - -1) * (areaRect.yMax - areaRect.yMin)) / 2;

        
        //now we have to make sure that the Size of the final model will not be outside of the safe area
        float RectHalfwidth = (ipRect.rect.width*ipRect.localScale.x/2);
        float RectHalfheight = ipRect.rect.height*ipRect.localScale.y/2;
        if(targetPosition.x - RectHalfwidth < areaRect.xMin)
        {
            
            targetPosition.x += (RectHalfwidth - targetPosition.x);
        }

        if (targetPosition.y - RectHalfheight < areaRect.yMin)
        {
            targetPosition.y += (RectHalfheight - targetPosition.y);
        }

        if (targetPosition.x + RectHalfwidth > areaRect.xMax)
        {
            targetPosition.x -= (RectHalfheight - ( areaRect.xMax- targetPosition.x));
        }

        if (targetPosition.y + RectHalfheight > areaRect.yMax)
        {
            targetPosition.y -= (RectHalfheight- (areaRect.yMax - targetPosition.y));
        }
        
        uiRect.anchorMax = new Vector2(0, 0);
        uiRect.anchorMin = new Vector2(0, 0);
        uiRect.anchoredPosition = targetPosition;
        //uiRect.rect.Set(0,0,uiRect.rect.width,uiRect.rect.height);
        
        // create Item slots in order across the Panel
                
        //create a new InventorySlotPrefab to adjust then we delete it afterwards

        
        //for now we will just go off the corner of the panel, presumably which should be rect.x and rect.y
        Vector2Int gridslotCount = inventoryGrid.GetGridSize();
        
        Vector2 slotOffset = slotFabRect.rect.size;
        Vector3[] v = new Vector3[4];
        ipRect.GetWorldCorners(v);
        Vector2 targetPos = new Vector2(-(slotOffset.x*gridslotCount.x)/2,slotOffset.y * gridslotCount.y /2);//new Vector2(v[1].x + slotOffset.x, v[1].y + slotOffset.y);
        
        

        
        
        //lets get size of the Panel. then place cubes in it
        float originalXOffset = targetPos.x; // so we can reset the offset each loop.
        // now we instantiate at every position
        for (int y = 0; y <= inventoryGrid.GetGridSize().y; y++)
        {
            for (int x = 0; x <= inventoryGrid.GetGridSize().x; x++)
            {
                GameObject newslot = GameObject.Instantiate(InventorySlotPrefab, _slotsParent.transform);
                //newslot.transform.SetParent(InventoryPanel.transform);
                newslot.GetComponent<RectTransform>().anchoredPosition = targetPos;
                // then increase the targets new position
                ItemGridSpaceInteractable interactableComponent = newslot.AddComponent<ItemGridSpaceInteractable>();
                interactableComponent.SetItemGridSpace(inventoryGrid.ItemGridSpaces[new Vector2Int(x, y)]);
                
                //we check if the slot has an item that hasnt been added to the gremlins Item list
                var value = inventoryGrid.ItemGridSpaces[new Vector2Int(x, y)];
                GridSpaceInteractables.Add(value, interactableComponent);
                if (value.heldItem != null)
                {
                    if (!HeldItemSprites.ContainsKey(value.heldItem))
                    {
                        // we check if that Space matches the Offset Origin for the Items data
                        // now we can create the item and add it to the held items list
                        DisplayItem(value.heldItem, targetPos);

                    }
                    else
                    {
                        // here we find the right Gremlin and Update its List of Vector2s
                        if (HeldItemSprites[value.heldItem] != null)
                        {
                            // we got the gremlin for that held item (its should have a sprite made already for it
                            // tell it to make sure it has this position on its list
                            //HeldItemSprites[value.heldItem].AddNewPosition(new Vector2Int(x, y));
                        }
                    }
                }
                targetPos.x += slotOffset.x;
            }

            targetPos.x = originalXOffset;
            targetPos.y -= slotOffset.y;
        }
        //find whatever is running the Inventory panels inventory code and tell it to create its prefabs idk
        ToskaUtilities.DebugItemList(inventoryGrid);
        
        // here is were we will start drawing the Inventory Items in the inventory for now
        /*List<ItemWithDrawPosition> itemGridList = inventoryGrid.GetItemsWithPosition();
        foreach (ItemWithDrawPosition itemPosPair in itemGridList)
        {
            // now we create them at position
            GameObject loadedUI = Resources.Load<GameObject>("BlankUIItem");
            GameObject newUIImage = Instantiate(loadedUI, this.gameObject.transform);
            newUIImage.GetComponent<Image>().sprite = itemPosPair.item.itemData.sprite;        // hey dont worry bout it
            newUIImage.GetComponent<RectTransform>().anchoredPosition = itemPosPair.position;
            // just realised why this was dumb to do. dang sleepy brain cameron
            
        } */
        
        // here is we set up the Gremlins Game Event for Inventory Updates
        Debug.Log("gremling Listeners Running");
        _InventoryUpdateListener =  this.gameObject.AddComponent<GameEventListener>();
        _InventoryUpdateListener.Event = UISystem.UpdateInventoryUI;
        _InventoryUpdateListener.Response.AddListener(UpdateInventoryUI);
        _InventoryUpdateListener.RegisterToEvent();             // gotta do this otherwise it will never actually get called
        
    }

    public void UpdateInventoryUI()
    {
        // we go through and test each gremlin to see if they can say if their slots hold the right item
        // if they are wrong it means that the heldItem key in the Dictionary doesnt have the correct positions saved
        // and that means it needs to be redrawn
        //Debug.Log("UpdateCalled");
        List<InventoryItem> inventoryItemsToRemove = new List<InventoryItem>();
        List<InventoryItem> inventoryItemsToAdd = new List<InventoryItem>();
        foreach (var space in _itemGrid.ItemGridSpaces)
        {
            if(space.Value.heldItem != null)
                if (!HeldItemSprites.ContainsKey(space.Value.heldItem))
                {
                    if(!inventoryItemsToAdd.Contains(space.Value.heldItem))
                        inventoryItemsToAdd.Add(space.Value.heldItem);
                }
                
        }
        foreach (var item in HeldItemSprites)
        {
            bool refreshneeded = false;     // bool used to track if that Items display is outdated
            foreach (Vector2Int position in item.Value.ItemPositions)
            {
                if (_itemGrid.ItemGridSpaces[position].heldItem != item.Key)
                {
                    // if the position in the grid doesnt contain the same Held item as the Gremlin says it should
                    refreshneeded = true;
                }
            }

            if (refreshneeded)
            {
                Destroy(item.Value.gameObject);
                if (!inventoryItemsToRemove.Contains(item.Key))
                {
                    inventoryItemsToRemove.Add(item.Key);
                }
            }
        }

        foreach (var t in inventoryItemsToRemove)
        {
            HeldItemSprites.Remove(t);
        }

        foreach (var t in inventoryItemsToAdd)
        {
            //now we create the item to display.
            //Vector2Int offset = space.Key - space.Value.heldItem.itemData.CalculateOriginPoint();
            Vector2Int localOrigin = ItemData.CalculateOriginPoint(t.HoldingGridPositions.ToArray());
            Vector3 targetPos = GridSpaceInteractables[_itemGrid.GridSpaces[localOrigin]].gameObject.GetComponent<RectTransform>().anchoredPosition;
            DisplayItem (t, targetPos);
        }
        
    }

    public void DisplayItem(InventoryItem itemToDisplay, Vector2 targetPos)
    {
        // this method will be for creating the Items sprite object and making sure the Held Items dictionary matches up
        GameObject loadedUI = Resources.Load<GameObject>("BlankUIItem");
        GameObject newUIImage = Instantiate(loadedUI, _itemsParent.transform);
        newUIImage.GetComponent<Image>().sprite =
            itemToDisplay.itemData.Sprite; // hey dont worry bout it
        newUIImage.GetComponent<RectTransform>().anchoredPosition = targetPos;
        UIItemSpriteGremlin spriteGremlin = newUIImage.AddComponent<UIItemSpriteGremlin>();
        HeldItemSprites.Add(itemToDisplay, spriteGremlin);
        spriteGremlin.ItemPositions = new List<Vector2Int>(itemToDisplay.HoldingGridPositions);
        
    }
    // This class handles the reference of the Inventory panel under the canvas.
    // it will probably also handle running all of the script work for resizing and positioning the panel.
    // as such it will be subscribing to the UI Systems runtime set. Yahoo!
}
