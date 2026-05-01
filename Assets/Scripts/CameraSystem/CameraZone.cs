using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public CameraSettingsData cameraSettingsData;

    public Camera zoneCamera;
    void Start()
    {
        if (zoneCamera == null)
        {
            zoneCamera = GetComponentInChildren<Camera>(true);
        }
    }
    void Update()
    {
        
    }
    
    //needs to have a method of handling the camera Settings data being entered.
    
    // On zone Enter it will check if the object is holding the desired component if so it will trigger a camera swap
    private void OnTriggerEnter(Collider other)
    {
        // here we check if it has the matching component, these collisions will only be checked on a special layer
        // so it wont be super costly
        Debug.Log("triggered");
        if(other.gameObject.GetComponentInChildren<CameraFocus>())
        { 
            CameraFocus cameraFocus = other.gameObject.GetComponentInChildren<CameraFocus>();
            Debug.Log("CameraSwapDetected");// then we ping the Camera Operator and pass them the CameraData
            //Debug.Log(Camera.current.name);
            CameraOperator.Instance.SwapCamera(zoneCamera);
            zoneCamera.gameObject.GetComponent<CameraDolly>().UpdateFocus(cameraFocus.gameObject);
        }
    }

    void OnEnable()
    {
        CameraOperator.Instance.RegisterZone(this);// this is where it will sign on to the register
    }

    void OnDisable()
    {
        CameraOperator.Instance.DeregisterZone(this);//this is where it will unassign itself
    }

}
