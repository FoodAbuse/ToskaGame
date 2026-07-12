using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventReporter : MonoBehaviour
{
    // Start is called before the first frame update
    public EventOnEmptyRuntimeSet runtimeSet;
    void OnEnable()
    {
        runtimeSet.AddReporter(this);
    }
    void OnDisable()
    {
        runtimeSet.RemoveReporter(this);
    }
}
