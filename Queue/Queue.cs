using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ResetableEnumerator<T> : IEnumerator<T>
{
    public IEnumerator<T> Enumerator { get; set; }
    public Func<IEnumerator<T>> ResetFunc { get; set; }

    public T Current { get { return Enumerator.Current; } }
    public void Dispose() { Enumerator.Dispose(); }
    object IEnumerator.Current { get { return Current; } }
    public bool MoveNext() { return Enumerator.MoveNext(); }
    public void Reset() { Enumerator = ResetFunc(); }
}

public enum AnimType
{
    After, OnTime
}

public class st
{
    public IEnumerator<int> e;
    public long startDelayTime, delayTime;
    public List<st> startOnTime;
    public List<st> startFinish;
    public st (IEnumerator<int> e, int delayTime)
    {
        this.e = e;
        this.delayTime = delayTime;
        startOnTime = new List<st>();
        startFinish = new List<st>();
    }
};
public class Queue : Destroyable
{
    public int currentTime = 0;
    public List<st> playingQueue = new List<st>();
    public List<st> addList = new List<st>();
    public st previous = null;
    public Queue()
    {
        MonoUpdateManager.Instance.AddUpdateListener(Update);
    }
    public void Destroy()
    {
        MonoUpdateManager.Instance.RemoveUpdateListener(Update);
    }

    private IEnumerable<int> empty()
    {
        yield return 0;
    }

    public void BeginPlaying(st p)
    {
        p.startDelayTime = currentTime + 1;
        addList.Add(p);
    }

    //一个会被每帧调用的函数。用它来负责让动画move next吧
    public void Update()
    {
        currentTime += 1;
        List<st> deleteList = new List<st>();
        foreach(st now in playingQueue)
        {
            if(now.delayTime + now.startDelayTime <= currentTime)
            {
                if (now.delayTime + now.startDelayTime == currentTime)
                {
                    foreach(st nex in now.startOnTime)
                    {
                        BeginPlaying(nex);
                    }
                }
                bool hasNext = now.e.MoveNext();
                if(hasNext == false)
                {
                    foreach (st nex in now.startFinish)
                    {
                        BeginPlaying(nex);
                    }
                    deleteList.Add(now);
                }
            }
        }
        foreach(st delete in deleteList)
        {
            playingQueue.Remove(delete);
        }
        foreach(st add in addList)
        {
            playingQueue.Add(add);
        }
        addList.Clear();
    }

    //该函数为对外的接口：
    //使用者在使用该系统时，希望能够通过该函数让一个个动画入栈，
    //入栈的动画应自动播放。
    //对于传入的动画，应每帧move next一次（同Unity的协程机制），
    //在一个动画播完后，从队列中移除，开始move next队列中的另一动画。
    //
    //参数：
    //e: 动画
    //type: 动画类型，与上一动画同时，或上一动画后。
    //delay: 延迟多久后才开始动画，一定大于等于0。
    public void EnqueueAction(IEnumerable<int> e, AnimType type = AnimType.After, int delay = 0)
    {
        
        st insert = new st(e.GetEnumerator(), delay);
        if (previous == null || playingQueue.Count==0)
        {
            insert.startDelayTime = currentTime + 1;
            playingQueue.Add(insert);
        }
        else
        {
            if(type == AnimType.OnTime)
            {
                previous.startOnTime.Add(insert);
            }
            else
            {
                previous.startFinish.Add(insert);
            }
        }
        Debug.Log(playingQueue.Count);
        previous = insert;
    }

    //该函数为对外的接口：
    //使用者在使用该系统时，希望能够通过该函数让一个个动画入栈，
    //入栈的动画应自动播放。
    //对于传入的动画，应每帧move next一次（同Unity的协程机制），
    //在一个动画播完后，从队列中移除，开始move next队列中的另一动画。
    //
    //参数：
    //e: 动画
    //type: 动画类型，与上一动画同时，或上一动画后。
    //delay: 延迟多久后才开始动画，一定大于等于0。
    public void EnqueueEmpty(AnimType type = AnimType.After, int delay = 0)
    {
        st insert = new st(empty().GetEnumerator(), delay);
        if (previous == null)
        {
            insert.startDelayTime = currentTime + 1;
            playingQueue.Add(insert);
        }
        else
        {
            if (type == AnimType.OnTime)
            {
                previous.startOnTime.Add(insert);
            }
            else
            {
                previous.startFinish.Add(insert);
            }
        }
        previous = insert;
    }

}
