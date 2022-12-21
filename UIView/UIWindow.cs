using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIWindow : UIComponent
{
    public UIWindow()
    {
        SetParent(UIManager.Instance.m_root.transform);
    }
    /// <summary>
    /// 刷新窗口
    /// </summary>
    public void UpdateWindow()
    {
        Loom.QueueOnMainThreadIfAsync(OnUpdate);
    }


    /// <summary>
    /// 创建子控件，如果parent为空，则实例化在window同级目录,创建的控件不会被保存在链表中。
    /// </summary>
    /// <typeparam name="T">控件类型</typeparam>
    /// <returns></returns>
    protected T CreateUIComponentWithOutSaving<T>(RectTransform parent) where T : UIComponent, new()
    {
        T component = new T();
        component.m_root = this;
        component.LoadLayer(0);//初始化
        if (parent != null)
        {
            component.SetLocation(parent, Vector2.zero);//设置父组件和坐标
        }
        component.Show();
        return component;
    }

    /// <summary>
    /// 创建子控件，如果parent为空，则实例化在window同级目录,创建的控件不会被保存在链表中。
    /// </summary>
    /// <typeparam name="T">控件类型</typeparam>
    /// <returns></returns>
    protected UIComponent CreateUIComponentWithOutSaving(Type t, RectTransform parent)
    {
        UIComponent component = (UIComponent)System.Activator.CreateInstance(t);
        component.m_root = this;
        component.LoadLayer(0);//初始化
        if (parent != null)
        {
            component.SetLocation(parent, Vector2.zero);//设置父组件和坐标
        }
        component.Show();
        return component;
    }

    /// <summary>
    /// 当窗口在激活状态且按下"ESC"时被调用
    /// </summary>
    public virtual void OnEscape()
    {

    }

    /// <summary>
    /// 窗口需要刷新时被调用（需要手动调用）
    /// </summary>
    protected virtual void OnUpdate()
    {

    }
}
