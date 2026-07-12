using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableComponent : MonoBehaviour, IInteractableExtended
{
    // might be able to change this to instead sit on the player and have it check objects that it walks over
    [HideInInspector]
    public bool isInteractable;

    public bool disableInteraction = false;
    // Start is called before the first frame update
    void Start()
    {
        OnStart();
    }

    public virtual void Interact()
    {
        //this is the place holder!!
    }
    public virtual void Interact(GameObject user)
    {
        
    }
    public virtual void OnUse(GameObject user)
    {
        
    }
    public virtual void OnStart()
    {
        // this exists so that children of interactables can still do stuff on start without changing any of the parents code;
    }

    /*public virtual bool OnInteract(NPC user)
    {
        return false;
    }*/

    public void DisableInteraction()
    {
        disableInteraction = true;
    }
}
public interface IReceiver

{

    bool Receive(ItemData input, ItemGridSpaceInteractable component);
}
public interface IReceiveable
{
    ItemData helditem{get; set;}   
    GameObject Origin{get; set;}
}
