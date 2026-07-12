using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventOnEmptyBroker : EventReporter
{
    // this subscribes to a runtime set but only when its method is called. this means it can be added at runtime.
    // and then have its chosen script called

    public void Register()
    {
        if (runtimeSet != null)
            runtimeSet.AddReporter(this);
    }

    public void Unregister()
    {
        if(runtimeSet != null)
            runtimeSet.RemoveReporter(this);
    }

    private void OnEnable() // this should be found before the parent classes OnEnalbe
    {
        Debug.Log("EventBroker enabled");
    }

    private void OnDisable() //this should be found before the parent classes
    {
        
    }
    // this is a component that contains methods to unsubscribe and resubscribe to its held event
    // its mostly to be used as a middle man between Pixelcrusher event calls and regular ITem system calls
    
}
