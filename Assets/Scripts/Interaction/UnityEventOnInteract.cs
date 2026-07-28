using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Utility;

[AddComponentMenu("BobaTeaScripts/InteractableObjects/UnityEventOnInteract")]
public class UnityEventOnInteract : InteractableComponent
{
    
    
    public UnityEvent m_Event; // the event that will be raised when this is interacted with
// this clas will activate a Unity Event on interaction!!!!!
// YAAAAHOOOOOOOOOOO
    public GameEvent m_GameEvent;
    
    public bool disabledByDefault = false;

    public override void Interact(GameObject user)
    {
        if (isInteractable)
        {
            m_Event.Invoke();
            if (m_GameEvent != null)
            {
                m_GameEvent.Raise();
            }
        }
    }

    public override void OnStart()
    {
        isInteractable = !disabledByDefault;
    }
    


}