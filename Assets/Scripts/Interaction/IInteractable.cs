using UnityEngine;
public interface IInteractable
{
    public bool IsInteractable()
    {
        return true;
    }
    void Interact();
}

public interface IInteractableExtended : IInteractable
{
    // hacky way to get around needing an method that requires a parameter for passing player Inventory

    void Interact(GameObject user);
    
}
