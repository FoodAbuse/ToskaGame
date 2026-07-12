using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grids;

public static class ToskaUtilities 
{
    public static void DebugItemList(ItemGrid itemGrid)
    {
        Debug.Log(itemGrid.GetItemCount());
        List<InventoryItem> itemGridList = itemGrid.GetHeldItems();
        foreach (InventoryItem item in itemGridList)
            Debug.Log(item.itemData.itemName);
    }

    public static Vector2 GetCentreOfVector2(Vector2[] vectors)
    {
        Vector2 centre = Vector2.zero;
        foreach (Vector2 vector in vectors)
        {
            centre += vector;
        }
        return centre/vectors.Length;
    }

    public static Vector2 GetCentreOfVector2(Vector2Int[] vectors)
    {
        Vector2 centre = Vector2.zero;
        foreach (Vector2Int vector in vectors)
        {
            centre += vector;
        }
        return centre/vectors.Length;
    }
    public static (bool, T) TestReturnComponent<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component != null)
        {
            return (true, component);
        }
        else
        {
            return (false, null);
        }
    }
}
