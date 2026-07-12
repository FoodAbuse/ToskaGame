using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public GameEvent Event;
    [SerializeField]
    private UnityEvent response;

    public UnityEvent Response
    {
        get
        {
            if (response == null)
                response = new UnityEvent();
            return response;
        }
    }

    public UnityEvent ResponseFalse;
    private void OnEnable()
    { 
        if (Event != null)
            Event.RegisterListener(this);
    }
    private void OnDisable()
    {
        if( Event != null)
            Event.UnregisterListener(this);
    }
    public void OnEventRaised()
    {
        Response.Invoke();
    }
    public void OnEventRaised(bool value)
    {
        if(value)
        {
            Response.Invoke();
        }
        else
        {
            if(ResponseFalse != null)
            {
                ResponseFalse.Invoke();
            }
        }
    }

    public void RegisterToEvent()
    {
        Event.RegisterListener(this);
    }
}
