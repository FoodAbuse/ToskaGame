using UnityEngine;

namespace Utility.GameEvents
{
    public class EventReporter : MonoBehaviour
    {
        // Start is called before the first frame update
        public EventOnEmptyRuntimeSet runtimeSet;
        void OnEnable()
        {
            runtimeSet.AddReporter(this);
        }
        void OnDisable()
        {
            runtimeSet.RemoveReporter(this);
        }
    }
}
