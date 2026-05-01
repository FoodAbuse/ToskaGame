using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDolly : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool InitialCamera = false;
    public bool LookAtFocus = false;
    public bool constrainPosX = false;
    public bool constrainPosY = false;
    public bool constrainPosZ = false;
    public bool constrainRotX = false;
    public bool constrainRotY = false;
    public bool constrainRotZ = false;
    [HideInInspector]
    public Vector3 offset;
    [HideInInspector]
    public GameObject focus;
    
    // will have a object that can be used by this class to "follow" a path. or lerp between two points based on the focus
    void Start()
    {
        if(focus != null)
        offset = transform.position - focus.transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        if (LookAtFocus)
        {
            Vector3 direction = focus.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;
        }

        if (!constrainPosY)
        {
            float y = focus.transform.position.y + offset.y;
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }

        if (!constrainPosX)
        {
            // this allows camera movement on the x axis
            // 
            float x = focus.transform.position.x + offset.x;
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

        if (!constrainPosZ)
        {
            float z = focus.transform.position.z + offset.z;
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
        }
    } 


    public void UpdateFocus(GameObject newFocus)
    {
        focus = newFocus;
        offset = transform.position - focus.transform.position;
    }

    public void OnEnable()
    {
        CameraOperator.Instance.RegisterDolly(this);
    }

    public void OnDisable()
    {
        CameraOperator.Instance.DeregisterDolly(this);
    }
}
