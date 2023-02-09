using System;
using System.Collections.Generic;
using UnityEngine;
 
public class BaseGameEvent : ScriptableObject
{
    #if UNITY_EDITOR
        [TextArea] public string developerDescription = "";
    #endif

    private readonly List<Action> listeners = new ();
 
    public void Raise()
    {
        for (var i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].Invoke();
        }
    }
    public void RegisterListener(Action listener)
    {
        if (!listeners.Contains(listener)) { listeners.Add(listener); }
    }
    public void UnregisterListener(Action listener)
    {
        if (listeners.Contains(listener)) { listeners.Remove(listener); }
    }
}

public abstract class BaseGameEvent<T1> : BaseGameEvent
{
    private readonly List<Action<T1>> listeners = new ();
 
    public void Raise(T1 t1)
    {
        for (var i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].Invoke(t1);
        }
    }
    public void RegisterListener(Action<T1> listener)
    {
        if (!listeners.Contains(listener)) { listeners.Add(listener); }
    }
    public void UnregisterListener(Action<T1> listener)
    {
        if (listeners.Contains(listener)) { listeners.Remove(listener); }
    }
}

public abstract class BaseGameEvent<T1,T2> : BaseGameEvent
{
    private readonly List<Action<T1,T2>> listeners = new ();
 
    public void Raise(T1 t1, T2 t2)
    {
        for (var i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].Invoke(t1, t2);
        }
    }
    public void RegisterListener(Action<T1,T2> listener)
    {
        if (!listeners.Contains(listener)) { listeners.Add(listener); }
    }
    public void UnregisterListener(Action<T1,T2> listener)
    {
        if (listeners.Contains(listener)) { listeners.Remove(listener); }
    }
}


/* USE:
*************

[CreateAssetMenu()]
public class GameEvent : BaseGameEvent {}
public class IntGameEvent : BaseGameEvent<int> {}
public class IntStringGameEvent : BaseGameEvent<int,string> {}

**************
*/