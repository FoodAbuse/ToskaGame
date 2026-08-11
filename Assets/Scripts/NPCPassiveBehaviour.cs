using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCPassiveBehaviour : ScriptableObject
{
    // these will be like NPCActions for NPCs. 
    // however instead of being something that is triggered by something that the NPC then runs alongside their goals
    // these will all start on the NPCs waking up. as opposed to starting when told to by a MonoGoal
    public IEnumerator RunningCoroutine;
    public abstract IEnumerator BehaviourCoroutine(NPC owner);


}

