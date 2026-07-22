using System.Collections.Generic;
using UnityEngine;
using Utility.GameEvents;

namespace Utility
{
    [CreateAssetMenu(menuName = "GameEvents/GameEvent")]
    public class GameEvent : ScriptableObject
    {
        protected List<GameEventListener> listeners =
            new List<GameEventListener>();
        public void Raise()
        {
            for(int i = listeners.Count -1; i >= 0; i--)
                listeners[i].OnEventRaised();
        }
        public void RegisterListener(GameEventListener listener)
        {
            listeners.Add(listener);
        }
        public void UnregisterListener(GameEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}
