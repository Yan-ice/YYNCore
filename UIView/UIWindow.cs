using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIWindow : UIComponent
{
    protected List<UIComponent> m_childComponents = new List<UIComponent>();
    public UIWindow()
    {
        SetParent(UIManager.m_root.transform);
    }
    /// <summary>
    /// 刷新窗口
    /// </summary>
    public void UpdateWindow()
    {
        Loom.QueueOnMainThreadIfAsync(OnUpdate);

    }
    /// <summary>
    /// 将已存在的GameObject加载为子控件。
    /// </summary>
    /// <typeparam name="T">控件类型</typeparam>
    /// <returns></returns>
    protected T LoadIComponent<T>(GameObject obj) where T : UIComponent, new()
    {
        T component = new T();
        component.LoadByObject(obj);
        m_childComponents.Add(component);//容器储存，方便集体销毁
        return component;
    }

    /// <summary>
    /// 创建子控件，如果parent为空，则实例化在window同级目录。
    /// </summary>
    /// <typeparam name="T">控件类型</typeparam>
    /// <returns></returns>
    public T CreateUIComponent<T>(RectTransform parent) where T : UIComponent, new()
    {
        T component = new T();
        component.m_root = this;
        component.LoadLayer(0);//初始化
        if (parent != null)
        {
            component.SetLocation(parent, Vector2.zero);//设置父组件和坐标
        }
        component.Show();//展示
        m_childComponents.Add(component);//容器储存，方便集体销毁
        return component;
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
        component.Show();//展示
        return component;
    }
    /// <summary>
    /// 创建子控件，如果parent为空，则实例化在window同级目录。
    /// </summary>
    /// <typeparam name="T">控件类型</typeparam>
    /// <param name="path">路径</param>
    /// <returns></returns>
    protected UIComponent CreateUIComponent(Type t, RectTransform parent)
    {
        UIComponent component = (UIComponent)System.Activator.CreateInstance(t);
        component.m_root = this;
        component.LoadLayer(0);//初始化
        if (parent != null)
        {
            component.SetLocation(parent, Vector2.zero);//设置父组件和坐标
        }
        component.Show();//展示
        m_childComponents.Add(component);//容器储存，方便集体销毁
        return component;
    }
    /// <summary>
    /// 销毁改UIWindow下的所有子控件
    /// </summary>
    protected void DestoryAllChildComponent()
    {
        foreach (UIComponent component in m_childComponents)
        {
            component.Destroy();
        }
        m_childComponents.Clear();
    }

    /// <summary>
    /// 返回该窗口是否正在显示
    /// </summary>
    /// <returns>窗口状态</returns>
    public bool IsShowing()
    {
        return !m_isDestroyed && m_isShowing;
    }

    /// <summary>
    /// 显示窗口
    /// </summary>
    public override void Show()
    {
        if (m_gameObjectOuter != null)//有一些UI组件压根没有m_gameObject_outer
        {
            m_gameObjectOuter.transform.localScale = Vector3.one;
            m_gameObjectOuter.SetActive(true);
        }
        m_isShowing = true;
        OnShow();
    }

    /// <summary>
    /// 隐藏窗口
    /// </summary>
    public virtual void Hide()
    {
        OnHide();
        m_isShowing = false;
        if (m_gameObjectOuter != null)//有一些UI组件压根没有m_gameObject_outer
        {
            m_gameObjectOuter.transform.localScale = Vector3.zero;
            m_gameObjectOuter.SetActive(false);
        }
    }
    /// <summary>
    /// 窗口显示时被调用
    /// </summary>
    protected abstract void OnShow();

    /// <summary>
    /// 窗口隐藏时被调用
    /// </summary>
    protected abstract void OnHide();

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
