using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public abstract class UISpriteGremlin : MonoBehaviour
{
    // this is the component that will be attached to a sprite object to do all the things they will need it to do
    // Ive decided abritrarily that all "gremlins" will be monobehaviours that manage one or more objects that display 
    // ui stuff
    // UI Sprite gremlins are different from regular Gremlins atm because they dont subscribe to the UI system
    // and dont get closed with regular ui closing events because of that. will probably change that in the future
    
    void Start()
    {
        
    }

    // Update is called once per frame
    public abstract void Update();

}

public class UIItemSpriteGremlin : UISpriteGremlin
{
    // it needs to know what slot it should be sitting over so it can refer to it
    public List<Vector2Int> ItemPositions = new List<Vector2Int>(); // the positions the Item can be in
    // this is checked during inventory Update events to make sure that the Item is in the correct position
    public override void Update()
    {
        // maybe we will just have the gremlin call methods to do these instead of every update
        //every update this will need to check that - Its Inventory Gremlin is open
        // if not close itself
        // that position matches the calculation for its current grid position (may lead to updating way more than it needs to
        // but will be useful later incase UI gets the capability to move around
    }

    public void UpdateUI()
    {
        
        // this will check that its held Item matches the ItemGridSpace its been Assigned
        // if not it will delete itself since it no longer is needed
        
    }

    public void AddNewPosition(Vector2Int position)
    {
        if (!ItemPositions.Contains(position))  // check it doesnt already contain the position
            ItemPositions.Add(position);        // if it doesnt, delete it
    }
    
    // will also handle the Sprites when they are sitting still in inventories
}
