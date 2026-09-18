using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Faction Behaviour", menuName = "NPCBehaviour/PassiveBehaviours/FactionBehaviour")]
public class FactionBehaviour : NPCPassiveBehaviour, INPCDeathCleanup, IFactionFollower
{
    // faction behaviour is both The faction the Creature belongs too and its feelings towards different factions
    // also the creatures NPC is attached to it
    
    public CreatureTolerances CreaturesTypes;
    public CreatureTolerances EnemieTypes;
    public CreatureDictionary owningDictionary;
    NPC owningNPC;
    public Vector3 position
    {
        get
        {
            return owningNPC.transform.position;
        }
    }

    public IHealthSystem GetHealthSystem()
    {
        return Array.Find(owningNPC.CurrentPassiveBehaviours,g => g is MortalityBehaviour) as IHealthSystem;
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

    public Vector3 GetPosition()
    {
        return position;
    }

    public CreatureTolerances GetCreatureTypes()
    {
        return CreaturesTypes;
    }
}
    
[Flags]
public enum CreatureTolerances
{
    None        = 0b_0000_0000,
    Player      = 0b_0000_0001,
    WildAnimal  = 0b_0000_0010,
    Scavenger   = 0b_0000_0100,
    Other       = 0b_0000_1000,
    Warped      = 0b_0001_0000,
    Sick        = 0b_0010_0000
}

