using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INPCDeathCleanup
{
    
    //this interface will be on NPC behaviours so that the NPC can tell them to unnassign themselves from their runtimeSet

    public void Cleanup();
}
