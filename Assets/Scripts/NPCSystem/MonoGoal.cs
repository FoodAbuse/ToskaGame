using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.AI;

public abstract class MonoGoal : ScriptableObject
{
    public int priority;
    public  MonoGoal next;

    public MonoGoal()
    {
            
    }

    public abstract void StartGoal(GameObject owner);
    public abstract bool isPossibleCheck(GameObject owner);

    public abstract void GoalTasker(GameObject owner);
    public abstract void GoalFinished(GameObject owner); // a method to be run when the goal is completed.
    public abstract void EvaluatePriority(GameObject owner);
    
}




