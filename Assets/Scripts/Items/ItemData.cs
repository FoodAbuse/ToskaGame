using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine.Serialization;

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Cursed,
}

public enum ItemType
{
    Misc,
    Consumable,
    Weapon,
    Armor,
    Material,
    QuestItem,
    Cursed,
    Upgrades,
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    [SerializeField]
    private Sprite _sprite;

    public Sprite Sprite
    {
        get
        {
            if (_sprite == null)
            {
                _sprite = Resources.Load<Sprite>("MissingDefaults/MissingSprite");
            }
            return _sprite;
        }
    }

   // [Header("Grid Size")]
    //[Min(1)] public int width = 1;
    //[Min(1)] public int height = 1;
    [Header("Rarity")]
    public ItemRarity rarity = ItemRarity.Common;

    [Header("Type")]
    public ItemType itemType = ItemType.Misc;

    [SerializeField]
    private GameObject worldItemDefault;

    public GameObject WorldItemPrefab
    {
        get
        {
            if (worldItemDefault == null)
            {
                worldItemDefault = Resources.Load<GameObject>("MissingDefaults/MissingPrefab");
            }
            return worldItemDefault;
        }
    }// the world representation of this Item
    // here we will have the grid of bools that will represent the Items space in the inventory
    
    [SerializeField]
    private List<Vector2Int> _gridPositions;

    public List<Vector2Int> GridPositions  // rename these plx cam
    {
        get
        {
            if (_gridPositions == null || _gridPositions.Count == 0)
            {
                _gridPositions = new List<Vector2Int>();
                _gridPositions.Add(new Vector2Int(0, 0)); // if _gridPositions returns null generate it with a list of 1 element
                return _gridPositions;
            }
            else
            {
                return _gridPositions;
            }
        }
    }

    private void Awake()
    {
        ItemDatabase.AddToGlobalDatabase(this);
    }

    public Vector2Int CalculateOriginPoint()
    {
        // this method will calculate an Origin for grid shifting items
        // we the list for the "shortest Vector2"
        /*
        Vector2Int shortestVector = GridPositions[0];
        bool multipleShortest = false;
        foreach (Vector2Int gridPosition in GridPositions)
        {
            if (gridPosition.magnitude < shortestVector.magnitude)
                shortestVector = gridPosition;
            multipleShortest = false;
            else if (gridPosition.magnitude == shortestVector.magnitude)
            {
                multipleShortest = true;
            }
        } */
        // for now we will assume we just want the Shortest X leftmost X coordinate
        Vector2Int originPoint = GridPositions[0];
        foreach(Vector2Int gridPosition in GridPositions)
        { // check if its the tallest
            if (gridPosition.x <= originPoint.x)
            {
                if(gridPosition.y <= originPoint.y)
                    originPoint = gridPosition;
            }
        }
        return originPoint;
    }

    public static Vector2Int CalculateOriginPoint(Vector2Int[]  gridPositions)
    {
        Debug.Log(gridPositions.Length);
        Vector2Int originPoint = gridPositions[0];
        foreach(Vector2Int gridPosition in gridPositions)
        { // check if its the tallest
            if (gridPosition.x <= originPoint.x)
            {
                if(gridPosition.y <= originPoint.y)
                    originPoint = gridPosition;
            }
        }
        return originPoint;
    }

    public Vector2Int GetLengths()
    {
        //this method will return the lenghts of its x,y
        int xLength = 0;
        int yLength = 0;
        foreach (Vector2Int gridPosition in GridPositions)
        {    if (gridPosition.x > xLength)
            {
                xLength = gridPosition.x;
            }
            if(gridPosition.y > yLength)
            {
                yLength = gridPosition.y;
            }
        }
        return new Vector2Int(xLength+1, yLength+1);
    }



    public static Vector2Int GetLengths(Vector2Int[] vector2Ints)
    {
        int xMax = 0;
        int xMin = 0;
        int yMax = 0;
        int yMin = 0;
        for (int i = 0; i < vector2Ints.Length; i++)
        {
            if (vector2Ints[i].x > xMax)
            {
                xMax = vector2Ints[i].x;
            }

            if (vector2Ints[i].y > yMax)
            {
                yMax = vector2Ints[i].y;
            }

            if (vector2Ints[i].x < xMin)
            {
                xMin = vector2Ints[i].x;
            }

            if (vector2Ints[i].y < yMin)
            {
                yMin = vector2Ints[i].y;
            }
        }
        Vector2Int lengths = new Vector2Int((Mathf.Abs(xMax-xMin)), Mathf.Abs(yMax-yMin));
        Debug.Log(lengths.x);
        
        return lengths;
    }

    private void OnDestroy()
    {
        // cleanup code for if the SO is deleted
        // might need to be changed to OnDisable if we have issues with the ItemData falling out of scope
        ItemDatabase.RemoveFromGlobalDatabase(this);
        
    }
}

