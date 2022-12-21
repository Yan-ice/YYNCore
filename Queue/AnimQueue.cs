using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum AnimType
{
    After, OnTime
}

public class DelayedAnim
{
    public AnimType type;
    private IEnumerator<int> e;
    private int delayTime;
    private bool hasNext = true;

    public DelayedAnim (IEnumerator<int> e, int delayTime)
    {
        this.e = e;
        this.delayTime = delayTime;
    }

    public bool Next()
    {
        if (!hasNext) return false;

        if(delayTime > 0)
        {
            delayTime--;
            return true;
        }
        if (e == null)
        {
            hasNext = false; return false;
        }
        hasNext = e.MoveNext();
        return hasNext;
    }
};
public class AnimQueue : Destroyable
{
    private Queue<DelayedAnim> waitQueue = new Queue<DelayedAnim>();
    private DelayedAnim wait_top = null;
    private List<DelayedAnim> playingQueue = new List<DelayedAnim>();

    bool isDestroyed = false;
    bool isUsed = false;

    public void Destroy()
    {
        isDestroyed = true;
    }
    public void DestroyImmediate()
    {
        isDestroyed = true;
        if (isUsed)
        {
            isUsed = false;
            waitQueue = null;
            playingQueue = null;
            wait_top = null;
            MonoUpdateManager.Instance.RemoveFixUpdateListener(Update);
        }
    }

    private void Update()
    {
        //播放当前动画
        if (playingQueue.Count > 0)
        {
            bool hs = false;
            foreach(DelayedAnim a in playingQueue)
            {
                if (a.Next()) hs = true;
            }

            if (!hs)
            {
                playingQueue.Clear();
            }

            return;
        }

        //已销毁
        if (isDestroyed) DestroyImmediate();

        //没有新的动画
        if (wait_top == null) return;

        //新增一批动画
        do
        {
            playingQueue.Add(wait_top);
            wait_top = waitQueue.Count > 0 ? waitQueue.Dequeue() : null;
        } while (wait_top != null && wait_top.type == AnimType.OnTime);

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
        if (isDestroyed) return;

        if (!isUsed)
        {
            MonoUpdateManager.Instance.AddFixUpdateListener(Update);
            isUsed = true;
        }

        DelayedAnim insert = new DelayedAnim(e.GetEnumerator(), delay);
        insert.type = type;

        if (wait_top == null)
        {
            wait_top = insert;
        }
        else
        {
            waitQueue.Enqueue(insert);
        }
    }

    class SimpleFuncEntity
    {
        Action action;
        public SimpleFuncEntity(Action a)
        {
            action = a;
        }
        public IEnumerable<int> act()
        {
            if(action!=null)
                action.Invoke();
            yield return 0;
        }
    }

    public void EnqueueSimpleAction(Action action, AnimType type = AnimType.After, int delay = 0)
    {
        IEnumerable<int> i = new SimpleFuncEntity(action).act();
        EnqueueAction(i,type,delay);
    }

}
