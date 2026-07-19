using System.Collections.Generic;
using UnityEngine;

namespace Utility.GameEvents
{
    [CreateAssetMenu(menuName = "GameEvents/EventOnEmptyRuntimeSet")]
    public class EventOnEmptyRuntimeSet : GameEvent
    {
        //this is a Runtime set that records any UI elements that report to it if they require a Mouse to be available to the player. It can do other things. why shouldnt it
        private List<EventReporter> reporters=          // these are the reporters. a script on gameobjects that will report to this to be added to the list
            new List<EventReporter>();
        bool Active = false;
        public void AddReporter(EventReporter reporter)
        {
            if(!reporters.Contains(reporter)) reporters.Add(reporter);          // this adds the reporter to the list and calls the listeners if the Active bool is swapped to true
            {
                if(!Active)
                {
                    Active = true;
                    for(int i = listeners.Count -1; i >= 0; i--)
                        listeners[i].OnEventRaised(Active);
                }
            }
        }
        public void RemoveReporter(EventReporter reporter)           //this removes the reporter from the list. and if there are no more reporters it sets the active bool to false and Calls all the listeners with the change
        {
            if (reporters.Contains(reporter)) reporters.Remove(reporter);
            if(reporters.Count == 0)
            {
                Active = false;
                for(int i = listeners.Count -1; i >= 0; i--)
                    listeners[i].OnEventRaised(Active);
            }
        }

    }
}
