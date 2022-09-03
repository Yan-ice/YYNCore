using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


internal interface IEventInfo
{

}

public class EventInfo<T,U> : IEventInfo
{
    public UnityAction<T,U> actions;
    public EventInfo(UnityAction<T,U> action)
    {
        actions = new UnityAction<T,U>(init);
        actions += action;
    }
    private void init(T t,U u)
    {

    }
}
public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;
    public EventInfo(UnityAction<T> action)
    {
        actions = new UnityAction<T>(init);
        actions += action;
    }
    private void init(T ms)
    {

    }
}
public class EventInfo : IEventInfo
{
    public UnityAction actions;
    public EventInfo(UnityAction action)
    {
        actions = new UnityAction(init);
        actions += action;
    }
    private void init()
    {

    }
}

/// <summary>
/// 事件中心
/// </summary>
public class EventCenter : Singleton<EventCenter>
{

    private static int SPECTATOR = "Event.Spectator".GetHashCode();

    //Key    --- 事件名称
    //Value  --- 监听这个事件对应函数的委托
    private Dictionary<int, IEventInfo> eventDic = new Dictionary<int, IEventInfo>();

    /// <summary>
    /// 添加事件旁观者
    /// </summary>
    /// <param name="name">事件的名字</param>
    /// <param name="action">准备用来处理事件的委托函数</param>
    public void AddSpectator(UnityAction<int,object> action)
    {
        if (eventDic.ContainsKey(SPECTATOR))
        {
            (eventDic[SPECTATOR] as EventInfo<int,object>).actions += action;
        }
        else
        {
            eventDic.Add(SPECTATOR, new EventInfo<int, object>(action));
        }
    }

    public static int reg_count = 0;
    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="name">事件的名字</param>
    /// <param name="action">准备用来处理事件的委托函数</param>
    public void AddEventListener<T>(int name, UnityAction<T> action)
    {
        reg_count++;
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }

    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        AddEventListener<T>(name.GetHashCode(), action);
    }

    public void AddEventListener(int name, UnityAction action)
    {
        reg_count++;
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions+=action;
        }
        else
        {
            eventDic.Add(name, new EventInfo(action));
        }
    }

    public void AddEventListener(string name, UnityAction act){
        AddEventListener(name.GetHashCode(),act);
    }

    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void RemoveEventListener<T>(int name, UnityAction<T> act)
    {
        
        if (act == null)
        {
            return;
        }
        reg_count--;
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions-=act;
        }
    }

    public void RemoveEventListener(int name, UnityAction action)
    {
        if (action == null)
        {
            return;
        }
        reg_count--;
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions-=action;
        }
    }

    /// <summary>
    /// 事件触发 带一个参数
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="name"></param>
    /// <param name="info">参数实例</param>
    public void EventTrigger<T>(int name, T info)
    {
        try {
        if (eventDic.ContainsKey(name))
        {
            //直接执行委托eventDic[name]();
            if ((eventDic[name] as EventInfo<T>).actions != null)
            {
                (eventDic[name] as EventInfo<T>).actions.Invoke(info);
                
            }
        }
        }
        catch (Exception e)
        {
            Log.Error("在触发事件时出现异常: " + e.Message + " \n" + e.StackTrace);
        }
        spectator(name, info);
    }

    /// <summary>
    /// 事件触发 无参数
    /// </summary>
    /// <param name="name"></param>
    public void EventTrigger(int name)
    {
        try
        {
            if (eventDic.ContainsKey(name))
            {
                //直接执行委托eventDic[name]();
                (eventDic[name] as EventInfo).actions.Invoke();
            }
        }catch(Exception e)
        {
            Log.Error("在触发事件时出现异常: "+e.Message+" "+e.StackTrace);
        }
        
        spectator(name, null);
    }

    public void EventTrigger(string name)
    {
        EventTrigger(name.GetHashCode());
    }

    public void EventTrigger<T>(string name, T info)
    {
        EventTrigger<T>(name.GetHashCode(),info);
    }
    /// <summary>
    /// 清除事件中心
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }

    private void spectator(int name,object obj){
        if(name!=SPECTATOR){
            if (eventDic.ContainsKey(SPECTATOR))
            {
                //直接执行委托eventDic[name]();
                if ((eventDic[SPECTATOR] as EventInfo<int,object>).actions != null)
                {
                    (eventDic[SPECTATOR] as EventInfo<int,object>).actions.Invoke(name,obj);
                }
            }
        }
    }

}


