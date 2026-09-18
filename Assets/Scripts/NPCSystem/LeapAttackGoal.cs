
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "new attackGoal", menuName = "NPCBehaviour/Goals/Attacks/LeapingAttack")]
public class LeapAttackGoal : MonoGoal
{
    // we look at the Aggression on sight behaviour to find our current  target.
    // [TODO] replace agg on sight
    // with something more polymorphic like a parent class or an interface. so we can have different classes for 
    // identifying a target but things like this only need to find one of them. (or their parent type)
    public float attackDmg;
    public float leapRange;
    public bool attacking = false;
    public bool leaping = false;
    
    public NPC ownerNPC;
    private AggressionOnSightBehaviour _targetfinder;

    public AggressionOnSightBehaviour TargetFinder
    {
        get
        {
            if(_targetfinder == null)
            {
                AggressionOnSightBehaviour targFind;
                targFind = Array.Find(ownerNPC.CurrentPassiveBehaviours, g => g is AggressionOnSightBehaviour) as
                    AggressionOnSightBehaviour;
                if(targFind != null)
                    _targetfinder = targFind;
            }
            return _targetfinder;
        }
    }

    private IFactionFollower target
    {
        get
        {
            return TargetFinder.currentTarget;
        }
    }
    public override void StartGoal(GameObject owner)
    {
        attacking = true;
        NPCAction attackAction = new NPCAction(FaceTowardsTarget(), NPCAction.ActionType.Movement);
        ownerNPC.StartAction(attackAction);
        
    }

    public override bool isPossibleCheck(GameObject owner)
    {
        // check that the NPC has any passive behaviour that identifies a target for aggression
        //if so and that a target is chosen then this goal is possible and should Be accomplished asap
        ownerNPC = owner.GetComponent<NPC>();
        if(TargetFinder != null)
            if(TargetFinder.currentTarget != null)
                return true;
        return false;
    }

    public override void GoalTasker(GameObject owner)
    {
        // probably start an Attack coroutine
        // we will just do some weird coroutine stuff like. Move till aiming at enemy. then move to  jump coroutine
        // stay jumping till hitting the ground or after a second or something Idk.
        // then do move towards again
    }

    public override void GoalFinished(GameObject owner)
    {
        // this goal becomes finished when the target is dead.
        attacking = false;
    }

    public override void EvaluatePriority(GameObject owner)
    {
        // if a target is available this goal should be very high Priority
        // will probably be fine as just being a high priority regardless as the NPC will only attack if there is
        // a valid target
    }

    public override void ExitFromGoal(GameObject owner)
    {
        // heres where we will end the coroutines that this is running. 
        attacking = false;
    }

    IEnumerator FaceTowardsTarget()
    {
        // we face towards da target. den we leap at dah target. yahoo
        // check if the creature is facing towards the target
        Debug.Log("Facing towards target");
        leaping = false;
        while (attacking)
        {
            //ownerNPC.GetComponent<NavMeshAgent>().updateRotation = false;
            Vector3 targetPos = TargetFinder.currentTarget.GetPosition();
            var direction = targetPos - ownerNPC.transform.position;
            float angle = Vector3.Angle(ownerNPC.transform.forward, direction);
            if (angle < 5)
            {
                Debug.Log("NowWELEAP!!");
               
                leaping = true;
                while (leaping == true)
                {
                    //do leap stuff here

                    leaping = false;
                    yield return null;
                } 

            }
            else
            {
                Vector3 newDirection = Vector3.RotateTowards(ownerNPC.transform.forward, direction , 2.5f* Time.deltaTime, 0.0f);
                 newDirection = new Vector3(0, newDirection.y, 0); 
                ownerNPC.transform.rotation =  Quaternion.LookRotation(newDirection);
            }
            yield return null;
        }

    }
}
