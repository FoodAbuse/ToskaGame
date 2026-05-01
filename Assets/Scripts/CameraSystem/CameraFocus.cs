using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    // all this component will do is trigger the camera zones to flip.
    public bool isInitialFocus = false;

    private void OnEnable()
    {
        if(isInitialFocus)
            CameraOperator.Instance.AssignInitialFocus(this);
    }
}
