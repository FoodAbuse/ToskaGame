using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System;
using UnityEditor;
using System.Linq;
using PixelCrushers.DialogueSystem;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Serialization;

[AddComponentMenu("BobaTeaScripts/NPCScripts/NPC")]
public class NPC : MonoBehaviour
{
    private MonoGoal _currentGoal;
    private MonoGoal[] _goals;      // this is the copy of the goals at runtime

    public MonoGoal[] Goals
    {
        get { return _goals; }
    }
    [ActorPopup]
    public string Actor;
    [Tooltip("characters animator here")]
    public Animator animator;

    
    
    public List<NPCAction> ActionsRunning = new List<NPCAction>(); // this is the public list of coroutines that are running.
    //private List<IEnumerator> _thoughtsRunner = new List<IEnumerator>(); // this is the private list of coroutines. It tracks whether or not a coroutine is running.
    
    public List<NPCPassiveBehaviour> BasePassiveBehaviours = new List<NPCPassiveBehaviour>();
    private NPCPassiveBehaviour[] _currentPassiveBehaviours;

    public NPCPassiveBehaviour[] CurrentPassiveBehaviours
    {
        get { return _currentPassiveBehaviours; }
    }
    private int _currentPriority
    {
        get {return _currentGoal != null ? _currentGoal.priority : -100;}
    }
    
    [FormerlySerializedAs("goalList")] [SerializeField]private MonoGoal[] startingGoalList;  // this is the goals in editor

    // here we will define NPC behaviour through the goals they have.
    public void ChooseCurrentGoal()
    {
        // this will simply loop through the goals the NPC assigning the one with the highest priority to its current goal
        //this will be called when the current goal is completed
        foreach (MonoGoal g in _goals)
        {
            //Debug.Log(g.name+"   "+ g.priority);
            if (g == null)
                continue;       // this means it will break from this iteration of the loop and begin a new one. should save us from a null reference error if the goal go missing
            if (g.priority > _currentPriority)
            {
                if(g.isPossibleCheck(gameObject) == true)
                {
                    _currentGoal = g;
                    
                }
            }
            
        }

        if (_currentGoal != null)
        {
            //Debug.Log(_currentGoal.name);
            Debug.Log("current Goal set to:" + _currentGoal.name);
            _currentGoal.StartGoal(gameObject);

        }
        else
        {
            //Debug.Log("No goal");
        }
    }

    void Start()
    {
        CreatePersonalGoalBehaviourCopies();   // this calls the Method to create a copy of the Scriptable objects without risking changing them
        StartPassiveBehaviours();
        ChooseCurrentGoal();
        
    }

    void Update()
    {
        // we will need to check if we are currently Moving on the goals objective(maybe handle this on goal)
        if(_currentGoal != null)
            _currentGoal.GoalTasker(gameObject);
        else
        {
            ChooseCurrentGoal();
        }
    }

    private void CreatePersonalGoalBehaviourCopies()
    {
        // this will be were we create instances of each goal object so we dont overwrite them!
        _goals = new MonoGoal[startingGoalList.Length];
        for (int i = 0; i < startingGoalList.Length; i++)
        {
            var goal = Instantiate(startingGoalList[i]);
            _goals[i] = goal; 
            Debug.Log(_goals[i].name +"" + _goals[i]);
            // silly boy created the blank copy but didnt actually write any data across
        }
        _currentPassiveBehaviours = new NPCPassiveBehaviour[BasePassiveBehaviours.Count];
        for (int i = 0; i < BasePassiveBehaviours.Count; i++)
        {
            var behaviour = Instantiate(BasePassiveBehaviours[i]);
            _currentPassiveBehaviours[i] = behaviour;
        }
    }

    public void ClearCurrentGoal()
    {
        _currentGoal = null;
        foreach(MonoGoal g in _goals)
            g.EvaluatePriority(gameObject);
    }

    public void SetCurrentGoal(MonoGoal goal)
    {
        _currentGoal = goal;
        _currentGoal.isPossibleCheck(gameObject); // we just call this to run the goals check
        _currentGoal.StartGoal(gameObject);         // then we call its start
        
    }   
    // this will be a coroutine called by goals to do things.
    public void StartAction(NPCAction action)
    {
        bool actionTaken = false;
        foreach ( NPCAction storedAction in ActionsRunning)
        {
            if (storedAction.actionType == action.actionType)
            {
                actionTaken = true;
            }
        }

        if (actionTaken == false)
        {
            StartCoroutine(ActionRunner(action));
        }
        
    }

    IEnumerator ActionRunner(NPCAction action)
    {
        
        ActionsRunning.Add(action);//this will add its coroutine to the Actions Running list
        yield return StartCoroutine(action.actionCoroutine);
        ActionsRunning.Remove(action);
        // then it will run its inner coroutine.
        // then it will remove its inner coroutine from the list when its finished
        
        
    }

    private void StartPassiveBehaviours()
    {
        foreach (NPCPassiveBehaviour passiveBehaviour in _currentPassiveBehaviours)
        {
            //passiveBehaviour.RunningCoroutine = passiveBehaviour.BehaviourCoroutine(this);
            StartCoroutine(passiveBehaviour.BehaviourCoroutine(this));
            
            
            //[TODO] should be able to remove pb.RunningCoroutine and just pass the IEnumerator method
        }
    }

    public void OnDisable()
    {
        foreach(var behaviour in CurrentPassiveBehaviours)
        {
            if (behaviour is INPCDeathCleanup cleaner)
            {
                cleaner.Cleanup();
            }
        }
    }
}

public class NPCAction          // this is a class that contains an Ienumerable to be run as a coroutine
{
    public NPCAction(IEnumerator action, ActionType TypeofAction)
    {
        actionType = TypeofAction;
        actionCoroutine = action;
    }
    public enum ActionType{Movement,DroppingItem,Searching,LookingAtHeldItem,mortalityTracking} // the different types of actions. an NPC can only be running one at a time.
    public IEnumerator actionCoroutine;
    public ActionType actionType;
}