using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class MonoUpdateManager : Singleton<MonoUpdateManager>
{
    private MonoController controller;

    public MonoUpdateManager()
    {
        GameObject obj = new GameObject("MonoController");
        GameObject.DontDestroyOnLoad(obj);
        controller = obj.AddComponent<MonoController>();
    }

    /// <summary>
    /// 为给外部提供的 添加帧更新事件
    /// </summary>
    /// <param name="fun"></param>
    public void AddUpdateListener(UnityAction fun)
    {
        controller.AddUpdateListener(fun);
    }


    /// <summary>
    /// 移除帧更新事件
    /// </summary>
    /// <param name="fun"></param>
    public void RemoveUpdateListener(UnityAction fun)
    {
        controller.RemoveUpdateListener(fun);
    }

    /// <summary>
    /// 为给外部提供的 添加帧更新事件
    /// </summary>
    /// <param name="fun"></param>
    public void AddFixUpdateListener(UnityAction fun)
    {
        controller.AddFixedUpdateListener(fun);
    }

    /// <summary>
    /// 移除帧更新事件
    /// </summary>
    /// <param name="fun"></param>
    public void RemoveFixUpdateListener(UnityAction fun)
    {
        controller.RemoveFixedUpdateListener(fun);
    }


    public GameObject Instantiate(GameObject gameObject,Transform transform)
    {
        if (gameObject == null|| transform == null)
        {
            return null;
        }
        return Instantiate(gameObject,transform);
    }

    public void RunForEveryTick(int ticks,Action<int> action,Action callback,bool isFixed)
    {
        if (isFixed)
        {
            int count = 0;
            void p()
            {
                count++;
                action.Invoke(count);
                if (count >= ticks)
                {

                    controller.RemoveFixedUpdateListener(p);
                    callback?.Invoke();
                }
            }
            controller.AddFixedUpdateListener(p);
        }
        else
        {
            int count = 0;
            void p()
            {
                count++;
                action.Invoke(count);
                if (count >= ticks)
                {
                    controller.RemoveUpdateListener(p);
                    callback?.Invoke();
                }
            }
            controller.AddUpdateListener(p);
        }
            

    }

    public void setSpeed(int time)
    {
        if(time<=0 || time > 5)
        {
            Log.Message($"非法的速度：{time}");
            return;
        }
        controller.speed = time;
    }

    public int getSpeed()
    {
        return controller.speed;
    }
}


/// <summary>
/// Mono管理
/// </summary>
class MonoController : MonoBehaviour
{
    
    private event UnityAction updateEvent;
    private event UnityAction fixedUpdateEvent;

    public int speed = 1;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    byte x = 0;
    void FixedUpdate()
    {
        fixedUpdateEvent?.Invoke();
    }
    private void Update()
    {
        x++;
        if (speed == 3 || (x % 2 == 0 && speed == 2))
        {
            updateEvent?.Invoke();
        }
        updateEvent?.Invoke();

    }

    public void AddFixedUpdateListener(UnityAction fun)
    {
        fixedUpdateEvent += fun;
    }

    public void RemoveFixedUpdateListener(UnityAction fun)
    {
        fixedUpdateEvent -= fun;
    }

    /// <summary>
    /// 为给外部提供的 添加帧更新事件
    /// </summary>
    /// <param name="fun"></param>
    public void AddUpdateListener(UnityAction fun)
    {
        updateEvent += fun;
    }

    /// <summary>
    /// 移除帧更新事件
    /// </summary>
    /// <param name="fun"></param>
    public void RemoveUpdateListener(UnityAction fun)
    {
        updateEvent -= fun;
    }


    public void m_DontDestroyOnLoad(GameObject go)
    {
        if (go == null)
        {
            return;
        }
        DontDestroyOnLoad(go);
    }
}
