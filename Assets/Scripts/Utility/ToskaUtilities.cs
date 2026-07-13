using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grids;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using Unity.VisualScripting;

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

    public static Vector2 GetCentroid(Vector2Int[] vectors)
    {   // this only works if there are no negatives
        float xSum = 0;
        float xMax = 0;
        float xMin = 0;
        float yMax = 0;
        float yMin = 0;
        float ySum = 0;
        for (int i = 0; i < vectors.Length; i++)
        {
            if (vectors[i].x > xMax)
            {
                xMax = vectors[i].x;
            }
            if (vectors[i].y > yMax)
            {
                yMax =  vectors[i].y;
            }

            if (vectors[i].x < xMin)
            {
                xMin = vectors[i].x;
            }

            if (vectors[i].y < yMin)
            {
                yMin = vectors[i].y;
            }
            xSum += vectors[i].x;
            ySum += vectors[i].y;
        }
        float XLength =  MathF.Abs(xMax - xMin);
        float YLength =  MathF.Abs(yMax - yMin);
        return (new Vector2(xSum / XLength, ySum / YLength));
    }
    public static Vector2Int GetRectSizeFromGridSpaces(Vector2Int[] vector2Ints)
    {
        // this method is for recieving a vector 2 of how many gridspaces should fit a rect drawn to encompass a passed
        // array of Vector2s
        
        int xMax = vector2Ints[0].x;
        int xMin = vector2Ints[0].x;
        int yMax = vector2Ints[0].y;
        int yMin = vector2Ints[0].y;
        for (int i = 0; i < vector2Ints.Length; i++)
        {
            if (vector2Ints[i].x+1 > xMax)
            {
                xMax = vector2Ints[i].x+1;
            }

            if (vector2Ints[i].y+1 > yMax)
            {
                yMax = vector2Ints[i].y+1;
            }

            if (vector2Ints[i].x+1 < xMin)
            {
                xMin = vector2Ints[i].x+1;
            }

            if (vector2Ints[i].y+1 < yMin)
            {
                yMin = vector2Ints[i].y+1;
            }
        }
        Vector2Int lengths = new Vector2Int((Mathf.Abs(xMax-xMin)), Mathf.Abs(yMax-yMin));
        Debug.Log(lengths );
        
        return lengths;
    }

    public static Vector2Int GetGridSpaceCorner(Vector2Int[] vector2Ints)
    {
        // this will return a Vector 2 of the Top LeftMost space of a rect sized to contain a List of Vector2s
        // this means we are looking of the Lowest y and the lowest L
        
        int xMin = vector2Ints[0].x;
        int yMin = vector2Ints[0].y;
        for (int i = 1; i < vector2Ints.Length; i++)
        {
            if (vector2Ints[i].x < xMin)
                vector2Ints[i].x = xMin;
            if(vector2Ints[i].y < yMin)
                vector2Ints[i].y = yMin;
        }
        return new Vector2Int(xMin, yMin);
    }
}
