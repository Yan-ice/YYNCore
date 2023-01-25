using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AnimPlayer : Singleton<AnimPlayer>
{
    public List<IEnumerator> addList = new List<IEnumerator>();
    public List<IEnumerator> removeList = new List<IEnumerator>();
    public List<IEnumerator> playList = new List<IEnumerator>();

    public AnimPlayer()
    {
        MonoUpdateManager.Instance.AddFixUpdateListener(Update);
    }
    public void Destroy()
    {
        MonoUpdateManager.Instance.RemoveFixUpdateListener(Update);
    }

    public void Play(IEnumerable<int> p)
    {
        IEnumerator<int> i = p.GetEnumerator();
        i.MoveNext();
        addList.Add(i);
    }

    //一个会被每帧调用的函数。用它来负责让动画move next吧
    public void Update()
    {
        foreach (IEnumerator now in playList)
        {
            if (!now.MoveNext())
            {
                removeList.Add(now);
            }
        }
        foreach(IEnumerator delete in removeList)
        {
            playList.Remove(delete);
        }
        foreach(IEnumerator add in addList)
        {
            playList.Add(add);
        }
        removeList.Clear();
        addList.Clear();
    }

}
