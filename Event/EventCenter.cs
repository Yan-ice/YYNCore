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
    public int priority(T evt);

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
        //Debug.Log("Triggering event " + e.GetType());
        if (!actions.ContainsKey(e.GetType())){
            return;
        }
        foreach(object t in actions[typeof(T)])
        {
            if(!(t is EventListener<T>))
            {
                Debug.LogError("WARN: inconsist type!");
            }
        }
            actions[typeof(T)].Sort(
                (object a, object b) =>
                {
                    if (!(a is EventListener<T>) || !(b is EventListener<T>))
                    {
                        Debug.Log("WARN: inconsist type!");
                    }
                    if (a == null || b == null)
                    {
                        Debug.Log("WARN: null!");

                    }
                    int p1 = ((EventListener<T>)a).priority(e);
                    int p2 = ((EventListener<T>)b).priority(e);
                    return p1.CompareTo(p2);
                }
            );

        for (int i = actions[typeof(T)].Count - 1; i >= 0; i--)
        {
            ((EventListener<T>)(actions[typeof(T)][i])).callback(e);
        }
    }
}

