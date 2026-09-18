using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[CreateAssetMenu(fileName = "new attackGoal", menuName = "NPCBehaviour/Goals/Attacks/ChaseAttack")]
public class ChaseAttackGoal : MonoGoal
{
    // we look at the Aggression on sight behaviour to find our current  target.
    // [TODO] replace agg on sight
    // with something more polymorphic like a parent class or an interface. so we can have different classes for 
    // identifying a target but things like this only need to find one of them. (or their parent type)
    public float chaseBonusSpeed;
    public float attackDmg;
    public float biteRange;
    public bool attacking = false;
    public bool leaping = false;
    private float baseSpeed;
    public float biteCooldown;
    private float currentCooldown = 0;
    
    public NPC ownerNPC;
    private AggressionOnSightBehaviour _targetfinder;
    public NavMeshAgent agent;
    private Animator _animator;

    public AggressionOnSightBehaviour TargetFinder
    {
        get
        {
            if (_targetfinder == null)
            {
                AggressionOnSightBehaviour targFind;
                targFind = Array.Find(ownerNPC.CurrentPassiveBehaviours, g => g is AggressionOnSightBehaviour) as
                    AggressionOnSightBehaviour;
                if (targFind != null)
                    _targetfinder = targFind;
            }

            return _targetfinder;
        }
    }

    private IFactionFollower target
    {
        get { return TargetFinder.currentTarget; }
    }

    public override void StartGoal(GameObject owner)
    {
        attacking = true;
        NPCAction attackAction = new NPCAction(ChaseTarget(), NPCAction.ActionType.Movement);
        _animator = owner.GetComponent<NPC>().animator;
        agent = owner.GetComponent<UnityEngine.AI.NavMeshAgent>();
        ownerNPC.StartAction(attackAction);

    }

    public override bool isPossibleCheck(GameObject owner)
    {
        // check that the NPC has any passive behaviour that identifies a target for aggression
        //if so and that a target is chosen then this goal is possible and should Be accomplished asap
        ownerNPC = owner.GetComponent<NPC>();
        if (TargetFinder != null)
            if (TargetFinder.currentTarget != null)
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

    IEnumerator ChaseTarget()
    {
        // we face towards da target. den we leap at dah target. yahoo
        currentCooldown -= Time.deltaTime;
        baseSpeed = agent.speed;
        agent.speed = baseSpeed +  chaseBonusSpeed;
        while (true)
        {
            Vector3 targetPos = new Vector3(target.GetPosition().x, ownerNPC.transform.position.y,
                target.GetPosition().z);
            agent.SetDestination(targetPos);
            // then we check to see if its reached its destination
            float distance = Vector3.Distance(ownerNPC.transform.position, agent.destination);
            if (distance <= biteRange && currentCooldown <= 0)
            {
                // here is where we would deal damage to the thing. 
                AttackCharacteristic attack = new AttackCharacteristic(attackDmg);
                target.GetHealthSystem().RecieveAttack(attack);
                currentCooldown = biteCooldown;
            }
            if (distance > agent.stoppingDistance)
            {
                if (_animator != null)
                    _animator.SetBool("Running", true);
                // we bite da player here. Oof ouchies!
            }
            else
            {
                    if (_animator != null)
                        _animator.SetBool("Running", false);
            }


            // check if the creature is facing towards the target
            yield return null;
        }

    }
}