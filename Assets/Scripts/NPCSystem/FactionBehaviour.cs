using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionBehaviour : NPCPassiveBehaviour, INPCDeathCleanup
{
    public CreatureTolerances CreaturesTypes;
    public CreatureDictionary owningDictionary;
    NPC owningNPC;

    public Vector3 position
    {
        get
        {
            return owningNPC.transform.position;
        }
    }
    
    public override IEnumerator BehaviourCoroutine(NPC owner)
    {
        
        // creature Dictionary will be a RuntimeSet with a Specific runtime set for each Scene
        //CreatureDictionary.ActiveDictionary.Add(this);
        owningNPC = owner;
        owningDictionary = CreatureDictionary.ActiveDictionary;
        owningDictionary.Add(this);
        yield return null;
    }
    // this class will tell the Creature to belong to a faction or factions that will be checked by enemies
    public void Cleanup()
    {
        owningDictionary.Remove(this);
    }
}

[Flags]
public enum CreatureTolerances
{
    Player = 0,
    WildAnimal = 1,
    Scavenger = 2,
    Other = 3,
    Warped = 4,
    Sick= 5
}
