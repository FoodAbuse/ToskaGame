using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Behaviour", menuName = "NPCBehaviour/PassiveBehaviours/TargetableBehaviour")]
public class AssignAttackTargetableBehaviour : NPCPassiveBehaviour, INPCDeathCleanup
{
    public ITargetable Targetable
    {
        get { return NpcTargetable as ITargetable; }
    }
    private NPCAttackTargetable NpcTargetable
    {
        get
        {
            if (_targetable == null)
            {
                MortalityBehaviour hpsys = Array.Find(_owningNPC.CurrentPassiveBehaviours,g => g is MortalityBehaviour) as  MortalityBehaviour;
                _targetable = new NPCAttackTargetable(_owningNPC.gameObject,  hpsys);
            }
            return _targetable;
        }
    }
    private NPC _owningNPC;
    private NPCAttackTargetable _targetable;

    //private bool hasBeenSetTargetable = false;

    public override IEnumerator BehaviourCoroutine(NPC owner)
    {
        _owningNPC = owner;
        NpcTargetable.ReportToRuntimeSet();
        yield return null;
    }

    public void Cleanup()
    {
        NpcTargetable.RemoveFromRuntimeSet();
    }

     

    private class NPCAttackTargetable :  ITargetable
    {
        public GameObject owner;
        public MortalityBehaviour healthBehaviour;

        private ITargetableRuntimeSet IruntimeSet
        {
            get { return ITargetableRuntimeSet.Instance; }
        }
        
        public Vector3 GetPosition()
        {
            return owner.transform.position;
        }
        
        public NPCAttackTargetable(GameObject newOwner,  MortalityBehaviour newHealthBehaviour)
        {
            owner = newOwner;
            healthBehaviour = newHealthBehaviour;
        } 

        public bool RecieveAttack(AttackCharacteristic incomingAttack)
        {
            healthBehaviour.RecieveAttack(incomingAttack);
            return true;
        }

        public void ReportToRuntimeSet()
        {
            //yeehaw
            IruntimeSet.Add(this);
        }

        public void RemoveFromRuntimeSet()
        {
            IruntimeSet.Remove(this);
        }
        
    }
}
