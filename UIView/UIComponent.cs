using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIComponent : IComparable
{

    protected GameObject m_gameObjectOuter;
    protected Transform m_parent;
    public UIWindow m_root { get; set; }
    private string m_prefabName;

    public bool m_isDestroyed { get; private set; }
    public bool m_isShowing { get; protected set; } = false;
    private int m_currentLayer = -1;

    public UIComponent()
    {
        m_parent = UIManager.m_root.transform;
        m_prefabName = GetType().Name;
    }
    
    public void LoadByObject(GameObject loaded_object)
    {

        if (m_gameObjectOuter != null)
        {
            Destroy();
        }
        if (loaded_object == null)
        {
            Log.Message("UIComponent进行LoadByObject时发现对象为空！");
            return;
        }
        m_gameObjectOuter = loaded_object;
        m_parent = loaded_object.transform.parent;
        m_prefabName = GetType().Name;
        ComponentInit();
        OnInit(m_gameObjectOuter);
    }
    public UIComponent(Transform parent)
    {
        m_parent = parent;
    }
    public void SetParent(Transform parent)
    {
        m_parent = parent;
        if (m_gameObjectOuter != null)
        {
            m_gameObjectOuter.transform.SetParent(parent);
        }
    }
    public void SetLocation(Transform parent, Vector2 location)
    {
        m_parent = parent;
        m_gameObjectOuter.transform.SetParent(m_parent);
        m_gameObjectOuter.transform.localPosition = location;
    }

    /// <summary>
    /// 显示组件
    /// </summary>
    public virtual void Show()
    {
        m_gameObjectOuter.transform.localScale = Vector3.one;
        //m_gameObject_outer.SetActive(true);
    }

    public void LoadLayer(int id)
    {
        if (m_currentLayer == id)
        {
            return;
        }
        m_currentLayer = id;
        string path = "UIComponent/";
        if (this is UIWindow)
        {
            path = "UIPrefab/";
        }

        object obj;
        if (id == 0)
        {
            obj = Resources.Load(string.Format("{0}{1}", path, m_prefabName));
            if (obj == null)
            {
                if (GetType().BaseType.Name != m_prefabName)
                {
                    Log.Message(string.Format("未找到UI预制件资源{0}！", m_prefabName));
                    m_prefabName = GetType().BaseType.Name;
                    m_currentLayer = -1;
                    LoadLayer(id);
                }
                else
                {
                    Log.Error(string.Format("未找到UI预制件资源{0}！", m_prefabName));
                }

                return;
            }
        }
        else
        {
            obj = Resources.Load(string.Format("{0}{1}_{2}", path, m_prefabName, id));
            if (obj == null)
            {
                if (GetType().BaseType.Name != m_prefabName)
                {
                    Log.Message(string.Format("未找到UI预制件资源{0}_{1}！", m_prefabName, id));
                    m_prefabName = GetType().BaseType.Name;
                    m_currentLayer = -1;
                    LoadLayer(id);
                }
                else
                {
                    Log.Error(string.Format("未找到UI预制件资源{0}_{1}！", m_prefabName, id));
                }

                return;
            }

        }

        if (obj is GameObject)
        {
            if (m_gameObjectOuter != null)
            {
                Destroy();
            }

            m_gameObjectOuter = GameObject.Instantiate((GameObject)obj, m_parent);
            m_gameObjectOuter.transform.localScale = Vector3.zero;
            //GameObject.DontDestroyOnLoad(m_gameObject_outer);
            //加载预制件
            if (SortOrder() != 0)
            {
                if (m_gameObjectOuter.GetComponent<GraphicRaycaster>() == null)
                {
                    m_gameObjectOuter.AddComponent<GraphicRaycaster>();
                }
                Canvas c = m_gameObjectOuter.GetComponent<Canvas>();
                if (c == null)
                {
                    c = m_gameObjectOuter.AddComponent<Canvas>();
                }
                c.overrideSorting = true;
                c.sortingOrder = SortOrder();
                c.sortingLayerName = "UI";
                //设置层级
            }
            m_isDestroyed = false;
            ComponentInit();
            OnInit(m_gameObjectOuter);
            //调用初始化
            if (m_isShowing)
            {
                Show();
            }
        }
        else
        {
            Log.Error(string.Format("{0}_{1}资源并不是一个预制件！", this.GetType().Name, id));
            return;
        }

    }


    /// <summary>
    /// 按照路径寻找子对象的transform
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns></returns>
    public Transform FindChild(string path)
    {
        Transform tran = m_gameObjectOuter.transform.Find(path);
        if (tran == null)
        {
            Log.Error(string.Format("UIWindow在为{0}寻找子组件时发现了错误的路径{1}", this.GetType().Name, path));
        }
        return tran;
    }

    /// <summary>
    /// 按照路径寻找子对象的控件
    /// </summary>
    /// <typeparam name="T">控件类型</typeparam>
    /// <param name="path">路径</param>
    /// <returns></returns>
    public T FindChildComponent<T>(string path)
    {
        Transform tran = m_gameObjectOuter.transform.Find(path);
        if (tran == null)
        {
            Log.Error(string.Format("UIWindow在为{0}寻找子物体时发现了错误的路径{1}。", this.GetType().Name, path));
            return default(T);
        }
        T comp = tran.GetComponent<T>();
        if (comp == null)
        {
            Log.Error(string.Format("UIWindow未找到{0}/{1}的控件{2}。", this.GetType().Name, path, typeof(T).Name));
            return comp;
        }
        return comp;
    }

    /// <summary>
    /// 销毁窗口
    /// </summary>
    public void Destroy()
    {
        OnDestroy();
        m_isDestroyed = true;
        GameObject.Destroy(m_gameObjectOuter);
        m_gameObjectOuter = null;

    }


    /// <summary>
    /// 提供给ScriptGenerator工具，初始化时被调用。
    /// </summary>
    protected virtual void ComponentInit()
    {

    }
    /// <summary>
    /// 初始化时被调用
    /// </summary>
    /// <param name="m_go">窗口预制件</param>
    protected abstract void OnInit(GameObject m_go);

    /// <summary>
    /// 销毁时被调用
    /// </summary>
    protected abstract void OnDestroy();

    /// <summary>
    /// 提供物件层级，
    /// </summary>
    protected virtual int SortOrder()
    {
        return 0;
    }

    public int CompareTo(object obj)
    {
        return -1 * SortOrder().CompareTo(((UIComponent)obj).SortOrder());
    }
}
