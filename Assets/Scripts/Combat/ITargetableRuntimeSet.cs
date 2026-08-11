using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class ITargetableRuntimeSet : RuntimeSet<ITargetable>
{
    // Start is called before the first frame update
    private static ITargetableRuntimeSet _instance;

    public static ITargetableRuntimeSet Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = CreateInstance<ITargetableRuntimeSet>();   
            }
            return _instance;
        }
    }

    private static ITargetable _playerTarget;

    public static ITargetable PlayerTarget
    {
        get
        {
            return _playerTarget;
        }
        set
        {
            // this is for the debug targetting colo
            _playerTarget = value;
            TargetChanged();
        }
    }

    private static List<ITargetListeners> _playerTargetListeners;

    public static List<ITargetListeners> PlayerTargetListeners
    {
        get
        {
            if (_playerTargetListeners == null)
            {
                _playerTargetListeners = new List<ITargetListeners>();
            }
            return _playerTargetListeners;
        }
    }
    private static void TargetChanged()
    {
        foreach (ITargetListeners listener in _playerTargetListeners)
        {
            listener.Response(_playerTarget);
        }
    }

}

public interface ITargetListeners
{
    // this interface is for types that need to be informed when the players current target is updated
    public void RegisterToListenerList()
    {
        ITargetableRuntimeSet.PlayerTargetListeners.Add(this);
    }

    public void UnregisterFromListenerList()
    {
        ITargetableRuntimeSet.PlayerTargetListeners.Remove(this);
    }
    public void Response(ITargetable newPlayerTarget);
}
