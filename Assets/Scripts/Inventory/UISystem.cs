using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grids;
using UnityEngine;
using UnityEngine.UI;


public static class UISystem
{
    // this will do all the handling of UI stuff.

    //runtime Set for all UI elements here

    //runtime Set for subtypes of UI elements here.

    private static GameEvent _updateInventoryUI;

    public static GameEvent UpdateInventoryUI
    {
        get
        {
            if (_updateInventoryUI == null)
            {
                _updateInventoryUI = ScriptableObject.CreateInstance<GameEvent>();
            }
            return _updateInventoryUI;
        }
    }

    private static UIRuntimeSet _uiReporterRuntimeSet;

    public static UIRuntimeSet UIReporterRuntimeSet
    {
        get
        {
            if (!_uiReporterRuntimeSet)
            {
                _uiReporterRuntimeSet = ScriptableObject.CreateInstance<UIRuntimeSet>();

            }
            return _uiReporterRuntimeSet;
        }
    }

    public static void CloseAllUI()
    {
        List<UIReporter> uiReporterstoClose = new List<UIReporter>();
        foreach (var reporter in UIReporterRuntimeSet.GetItems())
        {
            uiReporterstoClose.Add(reporter);
        }
        uiReporterstoClose.ForEach(reporter => reporter.CloseUI());
    }

    public static void CloseInventoryUI()
    {
        List<UIReporter>  uiReportersToClose = new List<UIReporter>();
        foreach (var reporter in UIReporterRuntimeSet.GetItems()
                     .Where(rep => rep.uiType == UIReporter.UIType.INVENTORY))
        {
            uiReportersToClose.Add(reporter);
        }
        uiReportersToClose.ForEach(reporter => reporter.CloseUI());
    }

    public static bool OpenContainerUI(InventoryTemplate inventTemplate, ItemGrid itemGrid)
    {
        bool lootingUIopen = false;
        if (UIReporterRuntimeSet.GetItems().Any(rep => rep.uiType == UIReporter.UIType.CONTAINER))
        {
            CloseContainerUI();
            return false;
        }
        InventoryUIGremlin uiGremlin = GameObject.Instantiate(inventTemplate.InventoryParent).GetComponent<InventoryUIGremlin>();
        uiGremlin.StartGremlin(itemGrid, new Vector2(1,0));
        return false;
    }

    public static void CloseContainerUI()
    {
        List<UIReporter> uiReportersToClose = new List<UIReporter>();
        foreach (UIReporter reporter in UIReporterRuntimeSet.GetItems()
                     .Where(rep => rep.uiType == UIReporter.UIType.CONTAINER))
        {
            uiReportersToClose.Add(reporter);
        }
        uiReportersToClose.ForEach(reporter => reporter.CloseUI());
    }

    public static bool OpenInventoryUI(InventoryTemplate inventTemplate, ItemGrid inventoryGrid)
    {
        bool inventOpen = false;
        List<UIReporter> uiReporters = UIReporterRuntimeSet.GetItems();
        // hacky boolean check since we dont actually a child type of UIReporter just an enum with the different types on it
        if (uiReporters.Any(rep => rep.uiType == UIReporter.UIType.INVENTORY))      //checks if any of the elements meets a condition
        {
            CloseInventoryUI();
            return false;
        }
        // here we call the gremlin. supposedly
        InventoryUIGremlin uiGremlin = GameObject.Instantiate(inventTemplate.InventoryParent).GetComponent<InventoryUIGremlin>();
        uiGremlin.StartGremlin(inventoryGrid, new Vector2(-1,0));
        
        

        return true;
    }

    private static void ResizeToFitObject(GameObject uiTargetObject, GameObject objectToFit, float paddingWidth,
        float paddingHeight)
    {
        Vector3 dimensions = Vector3.zero;
        (bool componentTrue, MeshFilter meshFilter) meshFiltTry =  TestReturnComponent<MeshFilter>(objectToFit);
        if (meshFiltTry.componentTrue)
        { 
            dimensions = meshFiltTry.meshFilter.mesh.bounds.size;
        }
        else
        {
            (bool componentTrue, RectTransform rectTransform) rectTransformTry = TestReturnComponent<RectTransform>(objectToFit);
            if (rectTransformTry.componentTrue)
            {
                dimensions = rectTransformTry.rectTransform.rect.size;
            }
        }
        
        //now we scale two of these dimensions by the grid Size X,z
        dimensions.x +=  paddingWidth;
        dimensions.y +=  paddingHeight;
    }

    public static Vector3 ScaleToSize(Vector3 currentScale, Vector3 desiredSize, Vector3 currentSize)
    {
        
        
        // we need to 
        /*
        let X be The desired Scale of the object
        z be the curentSize of the Object     
        s be the current scale
        let y be the Desired size
        t is true size of object at 1,1,1
        
        we need the True size first         (1/s)s gives us the number to multiply z by
        which is
        s = 1  z = 1st
        (1/s)z =t gives us the number to multiply z by
        ^^ reciprocal
        find s
        st= y
        s= y/t
        */
            // changing the first value from current scale to current size,IDk if this is right tho
            
        Vector3 trueSize = Vector3.Scale(currentSize, currentScale.GetReciprocal()) ;

        return desiredSize.DivideByVector(trueSize);
        
        
    }

    public static Vector2 ScaleToSize2(Vector2 currentScale, Vector2 desiredSize, Vector2 currentSize)
    {
        Vector2 trueSize = Vector2.Scale(currentSize, currentScale.GetReciprocal()) ;
       
        return desiredSize.DivideByVector(trueSize);
        
    }

    public static Vector2 ScaleToSize2(Vector2 currentScale, Vector3 desiredSize, Vector2 currentSize)
    {
        Vector2 trueSize = Vector2.Scale(currentSize, currentScale.GetReciprocal()) ;
        return desiredSize.DivideByVector(trueSize);
    }

    public static Vector2 SizeToFitGrid( GameObject objectToFit, Vector2Int gridSize,
        float paddingWidth, float paddingHeight)
    {
        // here we take a Vector 2 and resize an object to fit another object assuming they are layed out on a grid within it
        // probs a Unity UI thing for this but I cbf

        
        // grab the dimensions of the Object to fit
        Vector2 dimensions = Vector2.zero;
        (bool componentTrue, MeshFilter meshFilter) meshFiltTry =  TestReturnComponent<MeshFilter>(objectToFit);
        if (meshFiltTry.componentTrue)
        { 
            dimensions = meshFiltTry.meshFilter.mesh.bounds.size;
        }
        else
        {
            (bool componentTrue, RectTransform rectTransform) rectTransformTry = TestReturnComponent<RectTransform>(objectToFit);
            if (rectTransformTry.componentTrue)
            {
                dimensions = rectTransformTry.rectTransform.rect.size;
            }
        }
        
        //now we scale two of these dimensions by the grid Size X,z
        dimensions.x = (dimensions.x * gridSize.x) +  paddingWidth;
        dimensions.y = (dimensions.y * gridSize.y) +  paddingHeight;

        return dimensions;
    }
    
    
    //method for both checking if a component is true and returning the bool along with the Value without needing another gross GetComponent
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

