using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class CreatureDictionary : RuntimeSet<FactionBehaviour>
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
    
}
