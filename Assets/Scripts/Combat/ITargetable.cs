using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable

{
    // this will be the interface for targetable objects
    // it will be put onto enemies and stuff to have them be targeted
    public Vector3 GetPosition(); 
    // this will return the position of the targetable entity
    public bool RecieveAttack(AttackCharacteristic incomingAttack)
    {
        // we will have a class that contains attack characteristics. Accuracy/ damage and such 
        return true;
    }
}

public class AttackCharacteristic
{
    public AttackCharacteristic(float newdamage)
    {
        damage = newdamage;
    }
    public float damage = 0;
    
}
