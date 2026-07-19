using UnityEngine;
using UnityEngine.UI;

namespace Utility
{
    [CreateAssetMenu(fileName = "RunTimeSets/RunTimeSet GraphicRaycaster")]
    public class GraphicRaycasterRuntimeSet : RuntimeSet<GraphicRaycaster>
    {
        public static GraphicRaycasterRuntimeSet instance;
        void OnAwake()
        {
            if(instance == null)
            {
                instance = this;
            }
            if (instance != null && instance != this)
                throw new System.InvalidOperationException("Attempted to create multiple Graphic raycaster instances!. please assign all graphic raycasters to the graphic raycaster runtime instance.");
        
        }
        public override void Initializers()
        {
            if(instance == null)
            {
                instance = this;
            }
            if (instance != null && instance != this)
                throw new System.InvalidOperationException("Attempted to create multiple Graphic raycaster instances!. please assign all graphic raycasters to the graphic raycaster runtime instance.");
        }
        
    
    }
}