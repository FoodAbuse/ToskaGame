using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    Animator animator;
    
    bool _isAiming = false;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w"))
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            if (Input.GetKey("s"))
            {
                animator.SetBool("isWalkingBackwards", true);
            }
            else
                animator.SetBool("isWalkingBackwards", false);
        }
        if (!Input.GetKey("w"))
        {
            animator.SetBool("isWalking", false);
        }
        animator.SetBool("isAiming", _isAiming);
    }

    public void toggleAiming(bool input)
    {
        _isAiming = input;
    }
    
}
