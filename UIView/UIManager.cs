using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

class UIState
{
    public Canvas m_root;
    public Dictionary<Type, UIWindow> m_windows = new Dictionary<Type, UIWindow>();

    public void setEnable(bool e)
    {
        m_root.gameObject.SetActive(e);
    }

    public void destroy()
    {
        foreach(UIWindow w in m_windows.Values)
        {
            w.DestroyImmediate();
        }
        GameObject.DestroyImmediate(m_root.gameObject);
    }
}
public class UIManager : Singleton<UIManager>
{

    public static Camera m_UICamera;
    public static EventSystem m_eventSystem;

    //UI��Դ����Դ
    private DynamicResourceLoader dyn_loader;

    private GameObject root_model;
    private UIState state;
    private Stack<UIState> state_stack = new Stack<UIState>();
    public Canvas m_root { get { return state.m_root; } }
    private Dictionary<Type, UIWindow> m_windows { get { return state.m_windows; } }

    private List<UIWindow> m_winOrder = new List<UIWindow>();

    public UIManager()
    {
        ResourceLoaderInterface loader = new BundlePackLoader("AssetBundlePack/uiview");
        dyn_loader = new DynamicResourceLoader(loader,new Dictionary<string, Type>()
        {
            { "window", typeof(GameObject) },
            { "component", typeof(GameObject) },
            { "lang", typeof(LanguageText) },
        });

        GameObject UI_root = GameObject.Instantiate(Resources.Load<GameObject>("UICamera"));
        GameObject.DontDestroyOnLoad(UI_root);

        
        m_UICamera = UI_root.GetComponent<Camera>();
        root_model = UI_root.transform.GetChild(0).gameObject;
        m_eventSystem = UI_root.transform.GetChild(1).GetComponent<EventSystem>();

        state = getNewState();
    }

    public DynamicResourceLoader UILoader()
    {
        return dyn_loader;
    }
    
    private UIState getNewState()
    {
        UIState new_state = new UIState();
        new_state.m_root = GameObject.Instantiate(root_model, m_UICamera.transform).GetComponent<Canvas>();
        new_state.m_windows = new Dictionary<Type, UIWindow>();
        return new_state;
    }

    public void startNewState()
    {
        state.setEnable(false);
        state_stack.Push(state);
        state = getNewState();
    }

    public void restoreState()
    {
        state.destroy();
        if (state_stack.Count > 0)
        {
            state = state_stack.Pop();
            state.setEnable(true);
        }
        else
        {
            state = getNewState();
            Debug.LogAssertion("restoring to a default UI state.");
        }
        
    }
    public static Vector2 ScreenToCanvas(Vector2 screenPoint)
    {
        Vector2 v;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(Instance.state.m_root.GetComponent<RectTransform>(),
            screenPoint, m_UICamera, out v);
        return v;

    }

    /// <summary>
    /// ����һ�����ڵ�0�㼶��
    /// ��������Ѵ�����Ϊ0�㼶���������κ��¡�
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    public void InitWindow<T>() where T : UIWindow, new()
    {
        InitWindow<T>(0);
    }
    /// <summary>
    /// ����һ�����ڡ�
    /// ��������Ѵ��ڣ��������κ��¡�
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
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
    /// ��ʾһ������,����֮ǰ��layer��
    /// ������ڲ����ڣ����Զ���������
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
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
    /// ��ʾһ�����ڵ��ض��㼶��
    /// ������ڲ����ڣ����Զ���������
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
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
    /// ���һ������ʵ����
    /// ������ڲ����ڣ����Զ���������
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    public T GetWindow<T>() where T : UIWindow, new()
    {
        if (!m_windows.ContainsKey(typeof(T)))
        {
            InitWindow<T>();
        }else if (m_windows[typeof(T)].m_isDestroyed)
        {
            m_windows.Remove(typeof(T));
            InitWindow<T>();
        }
        return (T)m_windows[typeof(T)];
    }

    /// <summary>
    /// ����һ�����ڡ�
    /// ������ڲ����ڣ������辯�沢�������κβ�����
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
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
            Log.Assertion(string.Format("������ͼ����һ�������ڵĴ���{0}��", typeof(T).Name));
        }
    }

    /// <summary>
    /// ����һ�����ڡ�
    /// ������ڲ����ڣ������辯�沢�������κβ�����
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
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
                w.DestroyImmediate();
            }

            m_winOrder.Remove(w);
            m_windows.Remove(typeof(T));
        }
        else
        {
            Debug.Log(string.Format("������ͼ����һ�������ڵĴ���{0}��", typeof(T).Name));
        }
    }

    /// <summary>
    /// ���������������ڣ�Ȼ��չʾ�ô��ڡ�
    /// </summary>
    public void LoadOnlyWindow<T>() where T : UIWindow, new()
    {
        foreach (UIWindow w in m_windows.Values)
        {
            if (!(w is T))
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
    /// �������д��ڡ�
    /// </summary>
    public void HideAll()
    {
        foreach (UIWindow w in m_windows.Values)
        {
            w.Hide();
        }
    }
}


