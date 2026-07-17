using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grids
{
    [CreateAssetMenu(fileName = "ItemGrid", menuName = "Scriptable Objects/Grids/ItemGrid")]
    public class ItemGrid : ScriptableObject, IGrid
    { 
        public Dictionary<Vector2Int, GridSpace> GridSpaces
        {
            get
            {
                Dictionary<Vector2Int, GridSpace> gridSpaces = ItemGridSpaces.ToDictionary(kvp => kvp.Key, kvp => (GridSpace) kvp.Value);
                return gridSpaces;
            }
        }
        // if something is attempting to interact with ItemGrid as a regular Grid it will upcast its Itemgrid dictionary into a regular grid dictionary
        // this will save us from doin smelly downcasts

        public Dictionary<Vector2Int, ItemGridSpace> ItemGridSpaces { get; set; }
        //currently useless since Item grid is a constructor and we want a 
        public  ItemGrid(int GridHeight = 10, int GridWidth = 10) : base() 
        {
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    // now we construct the gridspace for that coordinate
                    ItemGridSpace newgridSpace =  new ItemGridSpace(new Vector2Int(x, y), this);
                    //and add it to the dictionary
                    GridSpaces.Add(new Vector2Int(x,y), newgridSpace);
                }
            }
        }

        public static ItemGrid Create(int GridHeight = 10, int GridWidth = 10)      // new method for generating the ItemGrids
        {
            ItemGrid newGrid = ScriptableObject.CreateInstance<ItemGrid>();
            newGrid.ItemGridSpaces =  new Dictionary<Vector2Int, ItemGridSpace>();
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    ItemGridSpace newgridSpace = new ItemGridSpace(new Vector2Int(x, y), newGrid );
                    newGrid.ItemGridSpaces.Add(new Vector2Int(x, y), newgridSpace);
                }
            }

            return newGrid;
        }

        public bool AddItemAtSpace(ItemData incomingData, Vector2Int targetCoordinates)
        {
            // this is used to attempt to add an Item to a specific Space in an inventory
            Vector2Int OriginCoords = incomingData.CalculateOriginPoint();
            //check if the Chosen space is empty or not
            if(ItemGridSpaces.ContainsKey(targetCoordinates))        //should always be true but perhaps in the future spaces can be rearranged
                if (ItemGridSpaces[targetCoordinates].heldItem == null)  //check its empty, 
                {
                    //then we check the spaces around the space can fit it
                    Vector2Int Offset = targetCoordinates - OriginCoords;
                    bool spacesValid = true;
                    foreach (Vector2Int checkedSpace in incomingData.GridPositions)
                    {
                        Vector2Int checkedSpaceCoordinates = checkedSpace + Offset;   //apply offset to get actual space in grid
                        if (ItemGridSpaces.ContainsKey(checkedSpaceCoordinates) ) // check that coord exists on grid
                        {
                            // if it does. grab its value as an ItemGridSpace
                            ItemGridSpace space = ItemGridSpaces[checkedSpaceCoordinates] ;
                            //check if it has a held item
                            if (space.heldItem != null)
                            {
                                spacesValid = false;
                            }
                        }
                        else
                        {
                            spacesValid = false;
                        }
                    }

                    if (spacesValid)
                    {
                        InventoryItem newPlacedItem = new InventoryItem(incomingData);
                        List<Vector2Int> updatedGridPositions = new List<Vector2Int>();
                        foreach (Vector2Int spaceCoord in incomingData.GridPositions)
                        {
                            Vector2Int localSpaceCoords = spaceCoord + Offset;
                            ItemGridSpace space = ItemGridSpaces[localSpaceCoords];
                            space.heldItem = newPlacedItem;
                            updatedGridPositions.Add(localSpaceCoords);
                        }
                        newPlacedItem.AssignInventory(this, updatedGridPositions.ToArray(),incomingData.GridPositions.ToArray());

                        UISystem.UpdateInventoryUI.Raise();
                        return true;
                    }
                }
            UISystem.UpdateInventoryUI.Raise();
            return false; // remove this when code is written
        }
        public bool AddItemAtSpace(InventoryItem incomingItem, Vector2Int targetCoordinates)
        {
            // this is used to attempt to add an Item to a specific Space in an inventory
            Vector2Int OriginCoords = incomingItem.PivotPosition;
            //check if the Chosen space is empty or not
            if(ItemGridSpaces.ContainsKey(targetCoordinates))        //should always be true but perhaps in the future spaces can be rearranged
                if (ItemGridSpaces[targetCoordinates].heldItem == null)  //check its empty, 
                {
                    //then we check the spaces around the space can fit it
                    Vector2Int Offset = targetCoordinates - OriginCoords;
                    bool spacesValid = true;
                    foreach (Vector2Int checkedSpace in incomingItem.itemData.GridPositions)
                    {
                        Vector2Int checkedSpaceCoordinates = checkedSpace + Offset;   //apply offset to get actual space in grid
                        if (ItemGridSpaces.ContainsKey(checkedSpaceCoordinates) ) // check that coord exists on grid
                        {
                            // if it does. grab its value as an ItemGridSpace
                            ItemGridSpace space = ItemGridSpaces[checkedSpaceCoordinates] ;
                            //check if it has a held item
                            if (space.heldItem != null)
                            {
                                spacesValid = false;
                            }
                        }
                        else
                        {
                            spacesValid = false;
                        }
                    }

                    if (spacesValid)
                    {
                        InventoryItem newPlacedItem = incomingItem;
                        List<Vector2Int> updatedGridPositions = new List<Vector2Int>();
                        foreach (Vector2Int spaceCoord in incomingItem.itemData.GridPositions)
                        {
                            Vector2Int localSpaceCoords = spaceCoord + Offset;
                            ItemGridSpace space = ItemGridSpaces[localSpaceCoords];
                            space.heldItem = newPlacedItem;
                            updatedGridPositions.Add(localSpaceCoords);
                        }
                        newPlacedItem.AssignInventory(this, updatedGridPositions.ToArray(),incomingItem.itemData.GridPositions.ToArray());

                        UISystem.UpdateInventoryUI.Raise();
                        return true;
                    }
                }
            UISystem.UpdateInventoryUI.Raise();
            return false; // remove this when code is written
        }
        public bool AddItemToGrid(ItemData incomingData)
        {
            Vector2Int OriginCoords = incomingData.CalculateOriginPoint();
            foreach (ItemGridSpace gridSpace in ItemGridSpaces.Values)
            {
                // first we check that the grid is empty or open
                if (gridSpace.heldItem == null)
                {
                    // grab an offset on the items pattern positions to get its Vector2 coordinates matching this grid
                    Vector2Int Offset = gridSpace.GridPosition - OriginCoords;
                
                    // now to check the rest of the slots needed for the item are free
                
                    // first we check if all of the spaces are valid coordinates inside the grid
                    bool spacesValid = true;  // bool for tracking if spaces are valid. for my brain
                    foreach (Vector2Int checkedSpace in incomingData.GridPositions)
                    {
                        // check if the space exists
                        Vector2Int localSpaceCoords = checkedSpace + Offset; // get the local coord
                        if (ItemGridSpaces.ContainsKey(localSpaceCoords) ) // check that coord exists
                        {
                            // if it does. grab its value as an ItemGridSpace
                            ItemGridSpace space = ItemGridSpaces[localSpaceCoords];
                            //check if it has a held item
                            if (space.heldItem != null)
                            {
                                spacesValid = false;
                            }
                        }
                        else
                        {
                            spacesValid = false;
                        }
                    
                    }
                    //spaces valid should only be true at this point if
                    // A: the positions needed exist on the grid
                    // B: the positions do not hold items
                    if (spacesValid)
                    {
                        // here we place the item into the item grid
                        // we go to each of the positions
                        // create an Inventory Item and tell each position that it is their item
                        // we also Match the ItemDatas gridpositions to the inventories. this saves us the 
                        // pain of calculating it later based on grid size and where the mouse clicked
                        
                        InventoryItem newPlacedItem = new InventoryItem(incomingData);
                    
                        List<Vector2Int> updatedGridPositions = new List<Vector2Int>();
                        foreach (Vector2Int spaceCoord in incomingData.GridPositions)
                        {
                            Vector2Int localSpaceCoords = spaceCoord + Offset;
                            ItemGridSpace space = ItemGridSpaces[localSpaceCoords];
                            space.heldItem = newPlacedItem;
                            updatedGridPositions.Add(localSpaceCoords);
                        }
                        newPlacedItem.AssignInventory(this, updatedGridPositions.ToArray(),incomingData.GridPositions.ToArray());
                        UISystem.UpdateInventoryUI.Raise();
                        return true;
                    }

                }
            
            }
            Debug.Log("Return False");
            return false;
        }

        public List<InventoryItem> GetHeldItems()
        {
            List<InventoryItem> heldItems = new List<InventoryItem>();
            foreach (GridSpace gridSpace in ItemGridSpaces.Values)
            {
                ItemGridSpace sillyCast =  gridSpace as ItemGridSpace;
                if(sillyCast != null)
                    if(sillyCast.heldItem != null)
                        if (!heldItems.Contains(sillyCast.heldItem))
                        {
                            heldItems.Add(sillyCast.heldItem);
                        }
            }
            return heldItems;
        }

        public int GetItemCount()
        {
            int count = 0;
            List<InventoryItem> heldItems = new List<InventoryItem>();
            foreach (ItemGridSpace itemgridSpace in ItemGridSpaces.Values)
            {
                    if(itemgridSpace.heldItem != null)
                        if (!heldItems.Contains(itemgridSpace.heldItem))
                        {
                            heldItems.Add(itemgridSpace.heldItem);
                        }
            }
            count = heldItems.Count;
            return count;
        }

        public List<ItemWithDrawPosition> GetItemsWithPosition()
        {
            List<InventoryItem> heldItems = new List<InventoryItem>();
            foreach (ItemGridSpace gridSpace in ItemGridSpaces.Values)
            {
                if(gridSpace.heldItem != null)
                    if (!heldItems.Contains(gridSpace.heldItem))
                    {
                        heldItems.Add(gridSpace.heldItem);
                    }
            }
            // first we get the Items
            // then we get every key with that matches a value
            List<ItemWithDrawPosition> itemsWithDrawPosition = new List<ItemWithDrawPosition>();
            foreach (InventoryItem item in heldItems)
            {
                List<Vector2Int> itemsPositions = new List<Vector2Int>();
                foreach (KeyValuePair<Vector2Int,ItemGridSpace> kvp in ItemGridSpaces)
                {
                        if(kvp.Value.heldItem == item)    // check that it holds the same item we are looking for
                            itemsPositions.Add(kvp.Key);            //if so then add it to the list of that items positions
                }
                // now we get the Centre Vector 2 by adding all of the Vectors and then dividing by the No of vectors
                Vector2 centrePos = ToskaUtilities.GetCentreOfVector2(itemsPositions.ToArray());
                itemsWithDrawPosition.Add(new ItemWithDrawPosition(centrePos,item));
            }
            return itemsWithDrawPosition;
        }
        
        public Vector2Int GetGridSize()
        {
            // go through every key and check if its X is higher if so set maxX to that value
            if (GridSpaces != null)
            {

                int maxX = 0;
                foreach (Vector2Int v2i in GridSpaces.Keys.Where(v => v.x > maxX))
                {
                    maxX = v2i.x;
                }
                int maxY = 0;
                //same for the Y
                foreach (Vector2Int v2i in GridSpaces.Keys.Where(v => v.y > maxY))
                {
                    maxY = v2i.y;
                }
                return new Vector2Int(maxX,maxY);
            }
            else return new Vector2Int(0, 0);
        }

        public int GetGridSpaceCount()
        {
            if (GridSpaces != null)
            {
                return GridSpaces.Count;
            }
            else return 0;
        }
        
    }
}
        public class ItemWithDrawPosition       // class is part of an easy method of drawing items that will be replaced with proper Interaction reactivity
        {
            public Vector2 position;
            public InventoryItem item;

            public ItemWithDrawPosition(Vector2 position, InventoryItem item)
            {
                this.position = position;
                this.item = item;
            }
        }
