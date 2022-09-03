using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : Singleton<UIManager>
{
    public static Camera m_UICamera;
    public static EventSystem m_eventSystem;
    public static Canvas m_root;

    private Dictionary<Type, UIWindow> m_windows = new Dictionary<Type, UIWindow>();
    private List<UIWindow> m_winOrder = new List<UIWindow>();

    public UIManager()
    {
        GameObject UI_root = GameObject.Instantiate(Resources.Load<GameObject>("UICamera"));
        GameObject.DontDestroyOnLoad(UI_root);

        m_root = UI_root.transform.GetChild(0).GetComponent<Canvas>();
        m_UICamera = UI_root.GetComponent<Camera>();
        m_eventSystem = UI_root.transform.GetChild(1).GetComponent<EventSystem>();
    }

    public static Vector2 ScreenToCanvas(Vector2 screenPoint)
    {
        Vector2 v;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UIManager.m_root.GetComponent<RectTransform>(),
            screenPoint, m_UICamera, out v);
        return v;

    }

    /// <summary>
    /// 加载一个窗口的0层级。
    /// 如果窗口已存在且为0层级，将不做任何事。
    /// </summary>
    /// <typeparam name="T">窗口类型</typeparam>
    public void InitWindow<T>() where T : UIWindow, new()
    {
        InitWindow<T>(0);
    }
    /// <summary>
    /// 加载一个窗口。
    /// 如果窗口已存在，将不做任何事。
    /// </summary>
    /// <typeparam name="T">窗口类型</typeparam>
    public void InitWindow<T>(int layer) where T : UIWindow, new()
    {
        if (!m_windows.ContainsKey(typeof(T)))
        {
            if (Thread.CurrentThread != Loom.mainThread)
            {
                Log.Error(string.Format("InitWindow<{0}>() can only be called in main thread", typeof(T).Name));
                return;
            }
            T new_window = new T();
            new_window.LoadLayer(layer);
            m_windows.Add(typeof(T), new_window);
            m_winOrder.Add(new_window);
            m_winOrder.Sort();
        }
        else if (m_windows[typeof(T)].m_isDestroyed)
        {
            DestroyWindow<T>();
            InitWindow<T>(layer);
        }
        else
        {
            m_windows[typeof(T)].LoadLayer(layer);
        }
    }

    /// <summary>
    /// 显示一个窗口,保持之前的layer。
    /// 如果窗口不存在，将自动被创建。
    /// </summary>
    /// <typeparam name="T">窗口类型</typeparam>
    public void ShowWindow<T>() where T : UIWindow, new()
    {
        if (Thread.CurrentThread != Loom.mainThread)
        {
            Log.Error(string.Format("ShowWindow<{0}>() can only be called in main thread", typeof(T).Name));
            return;
        }
        if (!m_windows.ContainsKey(typeof(T)) || m_windows[typeof(T)].m_isDestroyed)
        {
            InitWindow<T>();
        }
        if (!m_windows[typeof(T)].IsShowing())
        {
            m_windows[typeof(T)].Show();
        }
    }

    /// <summary>
    /// 显示一个窗口的特定层级。
    /// 如果窗口不存在，将自动被创建。
    /// </summary>
    /// <typeparam name="T">窗口类型</typeparam>
    public void ShowWindow<T>(int layer) where T : UIWindow, new()
    {
        if (Thread.CurrentThread != Loom.mainThread)
        {
            Log.Error(string.Format("ShowWindow<{0}>() can only be called in main thread", typeof(T).Name));
            return;
        }
        InitWindow<T>(layer);
        if (!m_windows[typeof(T)].IsShowing())
        {
            m_windows[typeof(T)].Show();
        }
    }

    /// <summary>
    /// 获得一个窗口实例。
    /// 如果窗口不存在，将自动被创建。
    /// </summary>
    /// <typeparam name="T">窗口类型</typeparam>
    public T GetWindow<T>() where T : UIWindow, new()
    {
        if (!m_windows.ContainsKey(typeof(T)))
        {

            InitWindow<T>();
        }
        return (T)m_windows[typeof(T)];
    }

    /// <summary>
    /// 隐藏一个窗口。
    /// 如果窗口不存在，将给予警告并不进行任何操作。
    /// </summary>
    /// <typeparam name="T">窗口类型</typeparam>
    public void HideWindow<T>() where T : UIWindow, new()
    {
        if (Thread.CurrentThread != Loom.mainThread)
        {
            Log.Error(string.Format("HideWindow<{0}>() can only be called in main thread", typeof(T).Name));
            return;
        }
        if (m_windows.ContainsKey(typeof(T)) && !m_windows[typeof(T)].m_isDestroyed)
        {
            if (m_windows[typeof(T)].IsShowing())
            {
                m_windows[typeof(T)].Hide();
            }
        }
        else
        {
            Log.Assertion(string.Format("程序企图隐藏一个不存在的窗口{0}。", typeof(T).Name));
        }
    }

    /// <summary>
    /// 销毁一个窗口。
    /// 如果窗口不存在，将给予警告并不进行任何操作。
    /// </summary>
    /// <typeparam name="T">窗口类型</typeparam>
    public void DestroyWindow<T>() where T : UIWindow, new()
    {
        if (Thread.CurrentThread != Loom.mainThread)
        {
            Log.Error(string.Format("DestroyWindow<{0}>() can only be called in main thread", typeof(T).Name));
            return;
        }
        if (m_windows.ContainsKey(typeof(T)))
        {
            UIWindow w = m_windows[typeof(T)];

            if (!w.m_isDestroyed)
            {
                w.Destroy();
            }

            m_winOrder.Remove(w);
            m_windows.Remove(typeof(T));
        }
        else
        {
            Log.Assertion(string.Format("程序企图销毁一个不存在的窗口{0}。", typeof(T).Name));
        }
    }

    /// <summary>
    /// 隐藏所有其他窗口，然后展示该窗口。
    /// </summary>
    public void LoadOnlyWindow<T>() where T : UIWindow, new()
    {
        foreach (UIWindow w in m_windows.Values)
        {
            if (!(w is T))
            //换场景的门是个特例
            {
                if (!w.m_isDestroyed)
                {
                    w.Hide();
                }

            }
        }
        ShowWindow<T>();
    }

    /// <summary>
    /// 隐藏所有窗口。
    /// </summary>
    public void HideAll()
    {
        foreach (UIWindow w in m_windows.Values)
        {
            w.Hide();
        }
    }
}


