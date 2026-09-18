using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.WebCam;
using Utility;

public class CreatureDictionary : RuntimeSet<IFactionFollower>
{
    
    private static CreatureDictionary _activeDictionary;

    public static CreatureDictionary ActiveDictionary
    {
        get
        {
            if (_activeDictionary == null)
            {
                _activeDictionary = CreateInstance<CreatureDictionary>();
                SceneDictionaries.Add(_activeDictionary);
            }
            return _activeDictionary;
        }
    }

    private static List<CreatureDictionary> _SceneDictionarys;

    public static List<CreatureDictionary> SceneDictionaries
    {
        get
        {
            if(_SceneDictionarys == null)
                _SceneDictionarys = new List<CreatureDictionary>();
            return _SceneDictionarys;
        }
        set
        {
            _SceneDictionarys = value;
        }
    }

    public static int GetActiveCount()
    {
        return ActiveDictionary.GetCount();
    }
    public static List<IFactionFollower> GetWithinRange(Vector3 position, float range)
    {
        //this method will return a list with all of the NPC that are within a range of the passed position
        // only works for a Searcher within the active dictionary
        List<IFactionFollower> result = new List<IFactionFollower>(CreatureDictionary.ActiveDictionary.GetItems());
        List<IFactionFollower> removals = new List<IFactionFollower>();
        foreach (IFactionFollower faction in result)
        {
            if (Vector3.Distance(faction.GetPosition(), position) > range)
            {
                removals.Add(faction);
            }
        }
        foreach  (IFactionFollower faction in removals)
            result.Remove(faction);
        return result;
    }
}
