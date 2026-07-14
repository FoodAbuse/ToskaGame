using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    
    [FormerlySerializedAs("allItems")] [Tooltip("All possible items available in the game.")]
    public List<ItemData> itemList = new List<ItemData>();
    
    
    
    // global Database instance here
    
    private static ItemDatabase _globalInstance;
    
    
    public static ItemDatabase GlobalDatabase
    {
        get
        {
            if (_globalInstance == null)
            {
                _globalInstance = ScriptableObject.CreateInstance<ItemDatabase>(); // create Database if its null
                AssetDatabase.CreateAsset(_globalInstance, "Assets/DataBase/TheDataBase/GlobalItemDataBase.asset");
                Debug.Log("GlobalItemDataBase.asset created");
                CollateGlobalDatabase();
            }

            return _globalInstance;
        }
    }  

    
    
    
    
    public static void CollateGlobalDatabase()
    {
        // this is the method that will be used to collect the List of all ItemDatas and assigned to the global DB
        string[] guidsToLoad = AssetDatabase.FindAssets("t:ItemData");
        List<ItemData> itemsToLoad = new List<ItemData>();
        foreach (string guid in guidsToLoad)
        {
            itemsToLoad.Add(AssetDatabase.LoadAssetAtPath<ItemData>(AssetDatabase.GUIDToAssetPath(guid)));
        }

        foreach (ItemData itemData in itemsToLoad)
        {
            GlobalDatabase.itemList.Add(itemData);
        }
    }

    public static void AddToGlobalDatabase(ItemData item)
    {
        //check that it isnt in the list. shouldnt have to worry about it being null since the act of calling this will
        //create the list
        if (!GlobalDatabase.itemList.Contains(item))
        {
            GlobalDatabase.itemList.Add(item);
            Debug.Log("Adding item " + item.name + " to global Item Database");
        }
    }

    public static void RemoveFromGlobalDatabase(ItemData item)
    {
        // this removes the item from the database. used for cleanup so we dont have bad references to deleated items
        if (GlobalDatabase.itemList.Contains(item))
        {
            GlobalDatabase.itemList.Remove(item);
            Debug.Log("Removing item " + item.name + " from global Item Database");
        }
    }


}

