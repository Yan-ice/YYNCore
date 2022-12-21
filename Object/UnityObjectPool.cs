using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UnityObjectPool : Pool<GameObject> 
{
    protected readonly Action<GameObject> m_init;
    protected readonly GameObject m_obj;
    public UnityObjectPool(GameObject obj, Action<GameObject> init, Action<GameObject> reset, int initialCapacity, int maxCapacity) : base()
    {
        m_resetAction = reset;
        m_maxCapacity = maxCapacity;
        m_obj = obj;
        m_init = init;
        if (initialCapacity > 0)
        {
            Grow(initialCapacity);
        }
    }

    /// <summary>
    /// 从对象池中获得一个物品
    /// </summary>
    /// <returns></returns>
    public override GameObject Acquire(Action<GameObject> reset)
    {
        GameObject element = base.Acquire(reset);
        element.SetActive(true);
        return element;
    }

    protected override void RecycleInternal(GameObject element)
    {
        element.SetActive(false);
        base.RecycleInternal(element);
    }

    protected override GameObject AddNewElement()
    {
        GameObject newElement = UnityEngine.Object.Instantiate(m_obj);
        m_init?.Invoke(newElement);
        m_allObj.Add(newElement);
        m_availableObj.Add(newElement);
        newElement.SetActive(false);
        return newElement;
    }
}