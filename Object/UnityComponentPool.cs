using System;
using UnityEngine;

/// <summary>
/// 可以存放所有从Unity中Component继承下来的类的对象
/// </summary>
public class UnityComponentPool<T> : Pool<T> where T : Component
{
    /// <summary>
    /// 创建一个存放Untiy中Component的对象池
    /// </summary>
    /// <param name="factory">创建物品的构造函数</param>
    /// <param name="reset">当从对象池中获得对象时执行的函数</param>
    /// <param name="initialCapacity">该对象池的大小</param>
    /// <param name="maxCapacity">该对象池的最大大小</param>
    public UnityComponentPool(Func<T> factory, Action<T> reset, int initialCapacity, int maxCapacity) : base(factory, reset, initialCapacity, maxCapacity) { }

    /// <summary>
    /// 从对象池中获得一个物品
    /// </summary>
    /// <returns></returns>
    public override T Acquire(Action<T> reset)
    {
        T element = base.Acquire(reset);
        element.gameObject.SetActive(true);
        return element;
    }

    protected override void RecycleInternal(T element)
    {
        element.gameObject.SetActive(false);
        base.RecycleInternal(element);
    }

    protected override T AddNewElement()
    {
        T newElement = base.AddNewElement();
        newElement.gameObject.SetActive(false);
        return newElement;
    }
}