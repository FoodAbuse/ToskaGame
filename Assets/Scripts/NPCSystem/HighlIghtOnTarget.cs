using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Behaviour", menuName = "NPCBehaviour/PassiveBehaviours/HighlightOnTarget")]
public class HighlightOnTarget : NPCPassiveBehaviour, INPCDeathCleanup, ITargetListeners
{
    // Start is called before the first frame update
    private NPC _npcOwner;
    private GameObject thisObject;
    
    [Header("Highlight")]
    public Color highlightColor = Color.red;
    
    private Renderer currentRenderer;
    private Color[] originalColors;

    public ITargetable OwnersTargetable
    {
        get
        {
            if (_ownersTargetable == null)
            {
                AssignAttackTargetableBehaviour targsys
                    = Array.Find(_npcOwner.CurrentPassiveBehaviours,g => g is AssignAttackTargetableBehaviour) as AssignAttackTargetableBehaviour;
                if(targsys != null)
                    _ownersTargetable = targsys.Targetable;
            }
            return _ownersTargetable;
        }
    }

    private ITargetable _ownersTargetable;
    public void Cleanup()
    {
        ITargetListeners thisInterface = this;      // this feels stupid as hell
        thisInterface.UnregisterFromListenerList();         // but it works apparently!
    }

    public void Response(ITargetable newTarget)
    {
        // here is where we get the owners stuff and change their colors or whatever
        if (currentRenderer != null)
        {
            if (OwnersTargetable == ITargetableRuntimeSet.PlayerTarget)
            {
                // this npc is the players target. so paint it
                for (int i = 0; i < currentRenderer.materials.Length; i++)
                {
                    currentRenderer.materials[i].color = highlightColor;
                }
            }
            else
            {
                for (int i = 0; i < currentRenderer.materials.Length; i++)
                {
                    currentRenderer.materials[i].color = originalColors[i];
                }
            }
            
        }
    }

    public override IEnumerator BehaviourCoroutine(NPC owner)
    {
        
        _npcOwner = owner;
        ITargetListeners thisInterface = this;
        currentRenderer = owner.gameObject.GetComponent<Renderer>();
        originalColors = new Color[currentRenderer.materials.Length];
        for (int i = 0; i < currentRenderer.materials.Length; i++)
        {
            originalColors[i] = currentRenderer.materials[i].color;
        }

        // find our targety
        thisInterface.RegisterToListenerList();
        yield return null;
    }
    
    
}
