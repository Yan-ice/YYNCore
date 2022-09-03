using System;
using System.Collections.Generic;

/// <summary>
/// 泛型对象池
/// </summary>
public class Pool<T>
{
    protected Func<T> m_initFuction; //构造函数

    protected Action<T> m_resetAction; //从对象池中获得该对象执行的函数

    protected List<T> m_availableObj;//任然可用的对象

    protected List<T> m_allObj;//所有对象

    protected int m_maxCapacity;//该对象池的最大大小，超过该大小之后回收等于销毁（暂时没用）

    public int Remaining() 
    {
        return m_availableObj.Count; 
    }

    public int Total()
    {
        return m_allObj.Count; 
    }

    public Pool() {
        m_availableObj = new List<T>();
        m_allObj = new List<T>();
    }
    /// <summary>
    /// 创建一个对象池
    /// </summary>
    /// <param name="factory">创建物品的构造函数</param>
    /// <param name="reset">当从对象池中获得对象时执行的函数</param>
    /// <param name="initialCapacity">该对象池的大小</param>
    /// <param name="maxCapacity">该对象池的最大大小</param>
    public Pool(Func<T> factory, Action<T> reset, int initialCapacity, int maxCapacity)
    {
        m_availableObj = new List<T>();
        m_allObj = new List<T>();
        if (factory != null)
        {
            m_initFuction = factory;
        }
        else
        {
            Log.Error("初始化对象池没有给定构造函数");
            return;
        }
        m_resetAction = reset;
        m_maxCapacity = maxCapacity;
        if (initialCapacity > 0)
        {
            Grow(initialCapacity);
        }
    }

    /// <summary>
    /// 从对象池中获得一个物品
    /// </summary>
    /// <returns></returns>
    public virtual T Acquire()
    {
        return Acquire(m_resetAction);
    }

    /// <summary>
    /// 使用特定的初始化函数从对象池中获得一个物品
    /// </summary>
    /// <param name="reset">在获得时候调用的重置函数</param>
    public virtual T Acquire(Action<T> reset)
    {
        if (m_availableObj.Count == 0)
        {
            Grow(1);
        }
        if (m_availableObj.Count == 0)
        {
            Log.Error("对象池已满,无法扩容");
        }

        int itemIndex = m_availableObj.Count - 1;
        T item = m_availableObj[itemIndex];
        m_availableObj.RemoveAt(itemIndex);
        reset?.Invoke(item);
        return item;
    }

    /// <summary>
    /// 返回该对象池是否包含这个对象
    /// </summary>
    public virtual bool Contains(T pooledItem)
    {
        return m_allObj.Contains(pooledItem);
    }

    /// <summary>
    /// 回收一个对象
    /// </summary>
    public virtual void Recycle(T pooledItem)
    {
        if (m_allObj.Contains(pooledItem) && !m_availableObj.Contains(pooledItem))
        {
            RecycleInternal(pooledItem);
        }
        else
        {
            Log.Error("试图回收一个对象池中不存在的对象或已经被回收的对象 " + pooledItem + " , " + this);
        }
    }

    /// <summary>
    /// 回收所有该对象池的对象
    /// </summary>
    public virtual void RecycleAll()
    {
        RecycleAll(null);
    }

    /// <summary>
    /// 回收所有该对象池的对象,回收时调用指定函数
    /// </summary>
    public virtual void RecycleAll(Action<T> preRecycle)
    {
        for (int i = 0; i < m_allObj.Count; ++i)
        {
            T item = m_allObj[i];
            if (!m_availableObj.Contains(item))
            {
                preRecycle?.Invoke(item);
                RecycleInternal(item);
            }
        }
    }

    /// <summary>
    /// 使对象池增长
    /// </summary>
    public void Grow(int amount)
    {
        for (int i = 0; i < amount; ++i)
        {
            AddNewElement();
        }
    }

    /// <summary>
    /// 回收对象的内部函数
    /// </summary>
    protected virtual void RecycleInternal(T element)
    {
        m_availableObj.Add(element);
    }

    /// <summary>
    /// 向对象池增加新的初始化对象
    /// </summary>
    protected virtual T AddNewElement()
    {
        T newElement = m_initFuction();
        m_allObj.Add(newElement);
        m_availableObj.Add(newElement);
        return newElement;
    }
}