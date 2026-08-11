using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Goal", menuName = "NPCBehaviour/Goals/WanderAbout")]
public class WanderGoal : MonoGoal
{
    public float wanderTime;
    public float maxRandomTimeOffset;
    public float wanderRadius;  // this is how long a character will spend wandering in a direction (including waiting at its end position.
    private UnityEngine.AI.NavMeshAgent agent;
    private Vector3 targetPosition;
    private float _adjustedWanderTime;
    Animator _animator;

    private float _time;
    // we could have an bool here to check if the NPC needs a special conversation (say they are an old lost lady)
    // this will simply grab a position near the npc and tell it to wander there.
    public override bool isPossibleCheck(GameObject owner)
    {   
        return true; // will probably always be possible. unless the npc doesnt have legs
    }

    public override void StartGoal(GameObject owner)
    {
        _animator = owner.GetComponent<NPC>().animator;
        agent = owner.GetComponent<UnityEngine.AI.NavMeshAgent>();
        targetPosition = new Vector3(
                                    owner.transform.position.x + Random.Range(-wanderRadius, wanderRadius),
                                    owner.transform.position.y,
                                    owner.transform.position.z+Random.Range(-wanderRadius, wanderRadius));
        _time = 0f;
        _adjustedWanderTime = Random.Range(wanderTime, wanderTime+maxRandomTimeOffset);
    }
    public override void GoalTasker(GameObject owner)
    {
        // we will grab the position on the npc and then add randomly to its x and y for the target position.
        
        agent.SetDestination(targetPosition);
        // then we check to see if its reached its destination
        float distance = Vector3.Distance(owner.transform.position, agent.destination);
        if(distance > agent.stoppingDistance)
            if(_animator != null)
                _animator.SetBool("Walkin", true);
        else
        {
            if(_animator != null)
                _animator.SetBool("Walkin", false);
        }
        _time += Time.deltaTime;
        if (_time >= _adjustedWanderTime)
        {
            GoalFinished(owner);
        }
        // we need a check to see if the character has reached their goal position before doing the walking animation
    }

    public override void GoalFinished(GameObject owner)
    {
        owner.GetComponent<NPC>().ClearCurrentGoal();
        if(_animator != null)
            _animator.SetBool("Walkin", false);
    }

    public override void EvaluatePriority(GameObject owner)
    {
        // this task should always be a low priority
        Debug.Log("wander goals Priority" + priority);
    }
}