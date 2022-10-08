using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public abstract class Event
{
}

public interface EventListener<T> where T : Event 
{
    public void callback(T evt);
}


public class EventCenter : Singleton<EventCenter>
{

    private Dictionary<Type, List<object>> actions = new Dictionary<Type, List<object>>();

    public void regAction<T>(EventListener<T> action) where T : Event
    {
        if (actions.ContainsKey(typeof(T)))
        {
            actions[typeof(T)].Add(action);
        }
        else
        {
            actions.Add(typeof(T), new List<object>());
            actions[typeof(T)].Add(action);
        }
    }

    public void unregAction<T>(EventListener<T> action) where T : Event
    {
        if (actions.ContainsKey(typeof(T)))
        {
            if (actions[typeof(T)].Contains(action))
            {
                actions[typeof(T)].Remove(action);
            }
        }
    }

    public void trigger<T>(T e) where T : Event
    {
        if (!actions.ContainsKey(e.GetType())){
            return;
        }
        for (int i = actions[e.GetType()].Count - 1; i >= 0; i--)
        {
            ((EventListener<T>)actions[e.GetType()][i]).callback(e);
        }
    }
}

