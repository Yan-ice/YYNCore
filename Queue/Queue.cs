using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class GameQueue : Singleton<GameQueue>
{

    private struct Task
    {
        public int run;
        public int param;
        public int param2;
        public int param3;
        public byte timecost;

        public Task(string name, int para, int timecost, int para2 = 0, int para3 = 0)
        {
            if (name != null)
            {
                run = reflect1[name];
            }
            else
            {
                run = -1;
            }
            
            param = para;
            param2 = para2;
            param3 = para3;
            this.timecost = (byte)timecost;
        }
    }

    public GameQueue(){
        MonoUpdateManager.Instance.AddUpdateListener(onUpd);
    }
    Queue<Task> m_runnable = new Queue<Task>();

    static Dictionary<string, int> reflect1 = new Dictionary<string, int>();
    static Dictionary<int, UAction> reflect2 = new Dictionary<int, UAction>();
    int counter = 0;

    public void RegisterAction(string name, Action act)
    {
        put(name, new UAction(act));
    }
    public void RegisterAction(string name,Action<int> act)
    {
        put(name, new UAction(act));
    }
    public void RegisterAction(string name, Action<int,int> act)
    {
        put(name, new UAction(act));
    }
    public void RegisterAction(string name, Action<int, int, int> act)
    {
        put(name, new UAction(act));
    }

    private void put(string name,UAction action)
    {
        if (reflect1.ContainsKey(name))
        {
            reflect2[reflect1[name]] = action;
        }
        else
        {
            reflect1.Add(name, counter);
            reflect2.Add(counter, action);
            counter++;
        }
    }


    public void addAction(string id, int timecost = 10, int param1 = 0, int param2 = 0, int param3 = 0)
    {
        m_runnable.Enqueue(new Task(id,param1,timecost,param2,param3));
    }

    public void clearQueue()
    {
        m_runnable.Clear();
    }

    public bool isClear()
    {
        return m_runnable.Count==0;
    }
    public int getRest()
    {
        return m_runnable.Count;
    }

    private int cooldown = 0;
    private void onUpd()
    {
        
        if (cooldown > 0)
        {
            cooldown--;
            return;
        }
        if(m_runnable.Count == 0)
        {
            return;
        }
        startNext();

    }
    
    private void startNext()
    {
        Task next = m_runnable.Dequeue();
        cooldown = next.timecost;
        try
        {
            if (next.run != -1)
            {
                reflect2[next.run].Invoke(next.param, next.param2, next.param3);
            }
            
        }
        catch (Exception e)
        {
            Log.Error("Error in Queue: " + e.Message + " " + e.StackTrace);
        }
        if (m_runnable.Count > 0 && cooldown == 0)
        {
            startNext();
        }
    }
}
