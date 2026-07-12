using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet<T> : ScriptableObject
{
    private List<T> items = new List<T>();
    public void Check()
    {
        for(int i = items.Count-1; i > -1; i--)           // go through each of the items in the list
        {
            if (items[i] == null)                       // check if the reference is badd
            {
                items.RemoveAt(i);                      // if it is we remove it.

            }
        }
    }
    public void Initialize()
    {
        items.Clear();
        Debug.Log("RuntimeSetCleared");
        Initializers();
        {

        }
    }
    public void Add(T t)
    {
        if(!items.Contains(t)) items.Add(t);
        //Debug.Log("Added "+ t + " to " + this.GetType().Name);
        //Debug.Log("RuntimeSetAddition!");
    }
    public void Remove(T t)
    {
        if (items.Contains(t)) items.Remove(t);
    }
    public T GetItemIndex(int index)
    {
        return items[index];
    }
    public List<T> GetItems()
    {
        return items;
    }
    public virtual void Initializers()
    {
    
    }
}