using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using Utility;

public class RangedCombatController : MonoBehaviour
{
    
    // every update this will draw a Cone collider out of the player and will look for targetable enemies. it will go from the Centre of the Cone outwards\
    [HideInInspector] public ITargetable target;
    public float TargetingRange;
    public float ConeWidth;

    public float DetectionAngle;
    private List<ITargetable[]> targets;
    public int maximumTargetAllocation;
    public LayerMask interactableMask = Physics.DefaultRaycastLayers;

    
    

    //public AttackCharacteristic attackInfo;

    public KeyCode FireKey = KeyCode.Space;

    public float attackDamage = 5f;

    private ITargetableRuntimeSet _targetableRuntimeSet
    {
        get
        {
            return ITargetableRuntimeSet.Instance;
        }
    }
    
    //public List<Collider> validTargets = new List<Collider>();

    public ITargetable currentTarget;

    public void Update()
    {
        if (Input.GetKeyDown(FireKey))
        {
            Debug.Log("Bang!");
            Attack();
        }
    }
    public void FixedUpdate()
    {
        //OverlapSphereForTargets();
        CheckForValidTargetsOfRuntimeSet();
        PrioritiseTarget();

    }

    //this class finds targets inside a cone in front of the player
    // first it draws an overlap sphere to find possible targets that could be inside the cone
    // then it uses a triangle over the sphere to filter out things that wont be inside the cone
    // this leaves us with a Orange slice shaped Intersection
    // rotating the Triangle to match a plane that contains both the possible targets vector and the Origin of the sphere
    // will give us a true cone
    public void Start()
    {
        Debug.Log(transform.forward);
    }

    private void Attack()
    {
        //here we tell the target that its been attacked
        
        currentTarget.RecieveAttack(new AttackCharacteristic(attackDamage));
    }
    private void OverlapSphereForTargets()
    {
        // [TODO] Write Sphere Raycast to collect all enemies in range to be filtered by the cone tri
        
        Collider[] possibleTargets = new Collider[maximumTargetAllocation]; // should be faster to allocate on the stack
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, TargetingRange, possibleTargets, interactableMask);
        if(numColliders > 0)
            CheckForValidTargetsOSlice(numColliders, possibleTargets);
    }

    private void CheckForValidTargetsOSlice(int numPTargets, Collider[] pTargets)
    {
        // [TODO] Write Orange Slice Trianle filter method to filter out targets not in range of Cone
        
        // [TODO] Calculate Area of Triangle
        
        //validTargets.Clear();
        Vector3 triangleCentrePoint = transform.forward * TargetingRange;
        Vector3 trianglePoint1 = transform.position;
        Vector3 trianglePoint2 = triangleCentrePoint + transform.right * (ConeWidth/2);
        Vector3 trianglePoint3 = triangleCentrePoint - transform.right * (ConeWidth/2);
        (float angle,Collider collider,ITargetable target) currentBestTarget = (360,null, null);
        for (int i = 0; i < numPTargets; i++)
        {
            bool isthisInCone = false;
            ITargetable targetable = pTargets[i].GetComponent<ITargetable>();
            // check that its inside the front of the cone first
            if (targetable != null)
            {
                //now check if direction towards the Target, is similar to the direction forwards of the origin of the triangle
                // (the player)
                var direction = Vector3.Normalize(pTargets[i].transform.position - transform.position);
                var dot = Vector3.Dot(direction, transform.forward);
                // should be able to use this to tell if the target is within the cone.
                //check if the direction is in front of the Cone 
                
                if (dot > 0)
                {
                    float angle = Vector3.Angle(transform.forward, direction);
                    if (angle < DetectionAngle && angle < currentBestTarget.angle)
                    {
                        currentBestTarget = (angle, pTargets[i], targetable);
                        
                    }
                }
                
            }
        }
        
        UpdateTarget(currentBestTarget.target);


    }

    private void CheckForValidTargetsOfRuntimeSet()
    {
        (float angle,ITargetable target) currentBestTarget = (360, null);
        bool targetFound = false;
        
        foreach (ITargetable target in _targetableRuntimeSet.GetItems())
        {
            Vector3 targetPos = target.GetPosition();
            if (Vector3.Distance(targetPos, transform.position) <= TargetingRange)
            {
                var direction = Vector3.Normalize(targetPos - transform.position);
                float angle = Vector3.Angle(transform.forward, direction);
                if (angle < DetectionAngle && angle < currentBestTarget.angle)
                {
                    targetFound = true;
                    currentBestTarget = (angle, target);
                    if (currentBestTarget.target != currentTarget)
                    {

                    }
                }
            }
        }
        UpdateTarget(currentBestTarget.target);
    }

    void UpdateTarget(ITargetable newTarget)
    {
        if (currentTarget == null || currentTarget != newTarget)
        {
            currentTarget = newTarget;
            //ITargetableRuntimeSet.PlayerTarget = currentTarget;
            // here is where we tell all targets that there is a new target
            // so probably telling some runtime set to update itself
            
            
            
            
        }
        ITargetableRuntimeSet.PlayerTarget = currentTarget;
    }
    private void PrioritiseTarget()
    {
        
    //[TODO] Write code for Priotising valid targets based off of closest to centre line
    }
    // [TODO] Write modification to Orange slice method so Tri rotates to encompass targets at different heights(true cone)
    // [TODO] account for colliders intersecting cone but game objects position not inside it
    
    //
    // [TODO] Create debug method for drawing a debug mesh of the colliders
}
