using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicNPC : MonoBehaviour, ITargetable
{
    
    // this will be a cube that Sees the player and then runs at them. jumping when close enough
    public float hp = 20;
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    
}
