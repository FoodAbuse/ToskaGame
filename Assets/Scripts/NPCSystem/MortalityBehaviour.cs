using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Behaviour", menuName = "NPCBehaviour/PassiveBehaviours/MortalityBehaviour")]
public class MortalityBehaviour : NPCPassiveBehaviour
{
    // this is the Behaviour that handles NPCS being able to have hp and die. 
    // At present this is a MonoGoal when really it should be some other thing that npcs possess that they always check
    // passively
    public float maxHP = 20f;
    private float _currentHP = 20f;
    private IEnumerator _mortalityCoroutine;
    private bool _coroutineRunning = false;
   
    
    
    
    
    public override IEnumerator BehaviourCoroutine(NPC owner)
    {
        _coroutineRunning = true;
        while(_currentHP > 0)
        {
            // means the character should be alive
            yield return null;
        }
        Debug.Log("This Creature has died!");
        Death(owner);
        _coroutineRunning = false;
    }
    
    

    private void Death(NPC owner)
    {
        Destroy(owner.gameObject);
    }

    public void RecieveAttack(AttackCharacteristic incomingAttack)
    {
        _currentHP -= incomingAttack.damage;
    }
}
