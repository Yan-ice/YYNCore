using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueQueue : Singleton<DialogueQueue>
{
    private struct Task
    {
        public UnityAction run;
        public int timecost;
        public Box<bool> waituntil;

        public Task(UnityAction run, int timecost)
        {
            this.run = run;
            this.timecost = timecost;
            waituntil = null;
        }
        public Task(UnityAction run, int timecost, Box<bool> waituntil)
        {
            this.run = run;
            this.timecost = timecost;
            this.waituntil = waituntil;
        }
    }

    Queue<Task> m_runnable = new Queue<Task>();

    bool m_pause = false;

    public void addAction(UnityAction run, int timecost)
    {
        m_runnable.Enqueue(new Task(run,timecost));
    }
    public void addAction(UnityAction run)
    {
        m_runnable.Enqueue(new Task(run, 10));
    }
    public void addAction(UnityAction run, Box<bool> waituntil, int timecost = 10)
    {
        m_runnable.Enqueue(new Task(run, timecost,waituntil));
    }
    public void addWaiting(Box<bool> waituntil,int checktime)
    {
        m_runnable.Enqueue(new Task(null, checktime, waituntil));
    }
    public DialogueQueue()
    {
        MonoUpdateManager.Instance.AddFixUpdateListener(onUpd);
    }
    
    public void setPause(bool pause)
    {
        m_pause = pause;
    }
    public void clearQueue()
    {
        m_runnable.Clear();
    }

    public bool isClear()
    {
        return m_runnable.Count==0;
    }


    private int cooldown = 0;
    private void onUpd()
    {
        if (m_pause)
        {
            return;
        }
        if (m_runnable.Count == 0)
        {
            return;
        }
        if (cooldown > 0)
        {
            cooldown--;
            return;
        }
        
        startNext();

    }
    
    private void startNext()
    {
        if (m_runnable.Count > 0)
        {
            Task next = m_runnable.Peek();
            cooldown = next.timecost;
            try
            {
                if (next.run != null)
                {
                    next.run.Invoke();
                }
                
            }catch(Exception e)
            {
                Log.Error("Error in Queue: " + e.Message + " " + e.StackTrace);
            }
            if (next.waituntil==null || next.waituntil.value)
            {
                m_runnable.Dequeue();
            }
            if(cooldown==0 && m_runnable.Count > 0)
            {
                startNext();
            }
        }
    }
}
