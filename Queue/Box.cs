using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LinkBox<T> : BoxInterface<T>
{
    public T value { 
        get { if (m_getter == null) return default(T); return m_getter.Invoke(); } 
        set { if (m_setter == null) return; m_setter.Invoke(value); }
    }

    private Action<T> m_setter;
    private Func<T> m_getter;

    public LinkBox(Func<T> getter, Action<T> setter){
        m_getter = getter;
        m_setter = setter;
    }
    
    public bool isDesroyed()
    {
        return m_getter == null;
    }
    public void Destroy()
    {
        m_setter = null;
        m_getter = null;
    }
}

public class Box<T> : BoxInterface<T>
{
    public virtual T value { get; set; }

    public Box(T v)
    {
        value = v;
    }
}

public interface BoxInterface<T>
{
    public abstract T value { get; set; }

}