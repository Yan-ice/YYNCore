using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;


public interface EventListener<T> where T : Event 
{
    /// <summary>
    /// 优先级越高的监听器将会越 先 触发！
    /// </summary>
    /// <param name="evt"></param>
    /// <returns></returns>
    public int priority(T evt);

    public void callback(T evt);

}

class SimpleEventListener<T> : EventListener<T> where T : Event
{
    public Action<T> m_action;
    int m_priority;
    public SimpleEventListener(Action<T> ac, int pri)
    {
        m_action = ac;
        m_priority = pri;
    }
    public void callback(T evt)
    {
        m_action?.Invoke(evt);
    }

    public int priority(T evt)
    {
        return m_priority;
    }
}

public class EventCenter : Singleton<EventCenter>
{
    private Dictionary<Type, List<object>> actions = new Dictionary<Type, List<object>>();

    /// <summary>
    /// 注册一个事件监听器。
    /// </summary>
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

    /// <summary>
    /// 注册一个简单事件监听器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <param name="priority"></param>
    public void regSimpleAction<T>(Action<T> action, int priority) where T : Event
    {
        SimpleEventListener<T> al = new SimpleEventListener<T>(action, priority);
        if (actions.ContainsKey(typeof(T)))
        {
            actions[typeof(T)].Add(al);
        }
        else
        {
            actions.Add(typeof(T), new List<object>());
            actions[typeof(T)].Add(al);
        }
    }

    /// <summary>
    /// 注销一个简单事件监听器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    public void unregSimpleAction<T>(Action<T> action) where T : Event
    {
        if (actions.ContainsKey(typeof(T)))
        {
            foreach (EventListener<T> el in actions[typeof(T)])
            {
                if(el is SimpleEventListener<T> && ((SimpleEventListener<T>)el).m_action == action)
                {
                    actions[typeof(T)].Remove(el);
                    return;
                }
                
            }
        }
    }

    /// <summary>
    /// 注销一个事件监听器。
    /// </summary>
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
    
    /// <summary>
    /// 立刻注销一类事件的所有监听器（用于状态复位）
    /// </summary>
    /// <param name="type"></param>
    public void unregAllActions<T>() where T: Event
    {
        foreach(Type key in actions.Keys)
        {
            if (typeof(T) == key)
            {
                actions[key].Clear();
            }
            
        }
    }

    /// <summary>
    /// 触发一个事件，通知所有监听该事件的监听器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="e"></param>
    public void trigger<T>(T e) where T : Event
    {
        if (!actions.ContainsKey(typeof(T))){
            return;
        }
            actions[typeof(T)].Sort(
                (object a, object b) =>
                {
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

