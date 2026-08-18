using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoredBehaviour : StateMachineBehaviour
{
    [SerializeField]
    private float _timeUntilBored;

    [SerializeField]
    private int _numberOfBoredAnimations;

    private bool _isBored;
    private float _idleTime; 
    private int _boredAnimation; 

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetIdle(animator);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
            if (_isBored == false)
            {
                _idleTime += Time.deltaTime;

                if (_idleTime > _timeUntilBored)
                {
                    _isBored = true;
                    int boredAnimation = Random.Range(1, _numberOfBoredAnimations +1);
                    animator.SetFloat("BoredAnimation", boredAnimation);

                }
            }
            else if (stateInfo.normalizedTime % 1 > 0.98)
            {
                ResetIdle(animator);
            }
    }
    private void ResetIdle(Animator animator)
    {
        _isBored = false;
        _idleTime = 0;
        animator.SetFloat("BoredAnimation", 0);
    }
}
