using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AggressionBehaviour", menuName = "NPCBehaviour/PassiveBehaviours/AggressionOnSightBehaviour")]
public class AggressionOnSightBehaviour : NPCPassiveBehaviour
{
    // this behaviour will make the NPC become aggressive towards something from the creature list
    // it will have a current target that it is attackign.
    // this is kinda like the simplified version of what will probably be a bunch of little things in the future
    private FactionBehaviour _thisFactionBehaviour
    {
        get
        {
            return Array.Find(owningNPC.CurrentPassiveBehaviours,g => g is FactionBehaviour) as FactionBehaviour;
        }
    }
    NPC owningNPC;
    List<IFactionFollower> _currentTargets;
    public float targetingDistance;
    public float detectionAngle;
    public IFactionFollower currentTarget;
    
    public override IEnumerator BehaviourCoroutine(NPC owner)
    {
        
        // creature Dictionary will be a RuntimeSet with a Specific runtime set for each Scene
        owningNPC = owner;
        while (true)
        {
            


            _currentTargets = CreatureDictionary.GetWithinRange(owningNPC.transform.position, targetingDistance);

            // now check that they are within the cone
            (float angle,IFactionFollower creature) currentBestTarget = (360, null);
            bool targetFound = false;
        
            foreach (IFactionFollower creature in _currentTargets)
            {
                // if the targeted enemies belonging factions is marked as true on the enum flags
                CreatureTolerances check = creature.GetCreatureTypes() & _thisFactionBehaviour.EnemieTypes;
                if ((creature.GetCreatureTypes() & _thisFactionBehaviour.EnemieTypes) != 0)
                {

                    Vector3 targetPos = creature.GetPosition();
                    var direction = Vector3.Normalize(targetPos - owner.transform.position);
                    float angle = Vector3.Angle(owner.transform.forward, direction);
                    if (angle < detectionAngle && angle < currentBestTarget.angle)
                    {
                        targetFound = true;
                        currentBestTarget = (angle,creature );
                        if (currentBestTarget.creature != currentTarget)
                        {
                            // here we have a new target so tell the dog to stop its current task and reprioritise
                            Debug.Log("target found!");
                            owningNPC.HaltCurrentGoal();
                            UpdateTarget(currentBestTarget.creature);
                        }
                    }
                    
                }

            }
            
            yield return null;
        }
        // we grab everything from the active creature dictionary thats in range
       // we put that in a list
       // then check if its in the forwards arc of the dog
       
       // then find the closest target 
        yield return null;
    }

    void UpdateTarget(IFactionFollower creature)
    {
        currentTarget = creature;
    }
}
