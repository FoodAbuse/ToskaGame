using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFactionFollower  
{
    // an interface to Marry the Faction Behaviour of npcs with the players combat controller so that they can both be
    // held by a runtime set
    // 
    // we just need position or a way to recieve position
    public Vector3 GetPosition();
    public CreatureTolerances GetCreatureTypes();
    
    public IHealthSystem GetHealthSystem();

}
public interface IHealthSystem
{
    public float GetHealth();
    public void RecieveAttack(AttackCharacteristic incomingAttack);
}

