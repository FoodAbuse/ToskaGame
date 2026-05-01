using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "CameraOperator", menuName = "Scriptable Objects/CameraOperator")]
public class CameraOperator : ScriptableObject
{
    // this will hold a list of all the Camera zones in the scene.
    // it will hold a static list.
    private List<CameraZone> CameraZones = new List<CameraZone>();
    private List<CameraDolly> cameraDollys= new List<CameraDolly>();
    private static CameraOperator _instance;
    public static Camera CurrentCamera { get; private set; }
    public static CameraOperator Instance
    {
        get{
            if (_instance == null)
            {
                _instance = ScriptableObject.CreateInstance<CameraOperator>(); 
                if(Camera.current != null)// check if a maincamera exists
                    CurrentCamera = Camera.current;
            } return _instance; }
    }
    public void RegisterZone(CameraZone zone)
    {
        if(!CameraZones.Contains(zone)) // check that it does not contain a zone before adding it.
            CameraZones.Add(zone);          // if not then add it
    }

    public void DeregisterZone(CameraZone zone)
    {
        if(CameraZones.Contains(zone))  // check that it contains the zone
            CameraZones.Remove(zone); //then remove it
    }

    public void RegisterDolly(CameraDolly dolly)
    {
        if(!cameraDollys.Contains(dolly))
            cameraDollys.Add(dolly);
        // then we check if the runtime set contains more than one member
        DollyCountCheck();

    }

    public void DollyCountCheck()
    {
        if (cameraDollys.Count > 1)
        {
            // if there is more than one camera dolly, check if either of them is not current then turn off one
            foreach (CameraDolly dolly in cameraDollys)
            {
                if (dolly.gameObject.GetComponent<Camera>() != CurrentCamera)
                {
                    dolly.gameObject.SetActive(false);// if not the current camera turn off
                    Debug.Log("Turning Of Dolly Camera");
                    if (cameraDollys.Count == 1)
                        return;
                }
                
            }
        }
    }
    public void DeregisterDolly(CameraDolly dolly)
    {
        if(cameraDollys.Contains(dolly))
            cameraDollys.Remove(dolly);
    }

    public void CameraModify(CameraSettingsData newData)
    {   
        //this will be the method that handles the transfer of camera settings to the object that is following the player.
    }

    public void SwapCamera(Camera newCam)
    {
        CurrentCamera.gameObject.SetActive(false);
        CurrentCamera = newCam;
        CurrentCamera.gameObject.SetActive(true);
    }

    public void AssignInitialFocus(CameraFocus newFocus)
    {
        // find the first Camera and tell it its this
        CurrentCamera = Camera.main;
        CameraDolly currentDolly = CurrentCamera.gameObject.GetComponent<CameraDolly>(); // get the first cameras dolly (since the CurrentCamera might not be the right one)
        if (currentDolly != null)
        {
            currentDolly.focus = newFocus.gameObject;
        }
        else
        {
            currentDolly = CurrentCamera.gameObject.AddComponent<CameraDolly>();
            currentDolly.focus = newFocus.gameObject;
        }
    }
}

public class CameraSettingsData
{
    public bool xPosConstrain = false;
    public bool yPosConstrain = false;
    public bool zPosConstrain = false;
    public bool xRotConstrain = false;
    public bool yRotConstrain = false;
    public bool zRotConstrain = false;
    public Vector3 Pos;
    public Vector3 Rot;
    

    // this will just be a custom class for transferring the data about the desired camera setting to the camera operator
    // it will need to have data such as:
    // transform data. // constraint information // and how it tracks and if it tracks an object.
}