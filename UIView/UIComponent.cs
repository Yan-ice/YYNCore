using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class UIComponent : Destroyable, IComparable
{
    /// <summary>
    /// 该变量决定加载UI控件资源的目录。
    /// </summary>
    public static string RESOURCE_ROOT = "UIComponent";

    private AnimQueue ui_queue = new AnimQueue();
    private LanguageText lang;

    protected List<UIComponent> m_childComponents = new List<UIComponent>();

    protected GameObject m_gameObjectOuter;
    protected Transform m_parent;

    public UIComponent m_root { get; set; }
    private Type m_prefabType;

    public bool m_isDestroyed { get; private set; }
    public bool m_isShowing { get; private set; } = false;
    private int m_currentLayer = -1;

    public UIComponent()
    {
        m_parent = UIManager.Instance.m_root.transform;
        m_prefabType = GetType();
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
    public bool SetLocation(Transform parent, Vector3 location)
    {
        m_parent = parent;
        m_gameObjectOuter.transform.SetParent(m_parent);
        RectTransform r = m_gameObjectOuter.GetComponent<RectTransform>();
        if (r.anchoredPosition.Equals(location)) return false;
        r.anchoredPosition = location;
        return true;
    }

    public void LoadLayer(int id)
    {
        if (m_currentLayer == id)
        {
            return;
        }
        m_currentLayer = id;

        GameObject obj;
        if (id == 0)
        {
            UIManager.Instance.UILoader().GetAsset(m_prefabType.Name, out obj, out lang);
            if (obj == null)
            {
                if (m_prefabType.BaseType != null)
                {
                    Debug.Log(string.Format("未找到UI预制件资源{0}！", m_prefabType.Name));
                    m_prefabType = m_prefabType.BaseType;
                    m_currentLayer = -1;
                    LoadLayer(id);
                }
                else
                {
                    Debug.LogError(string.Format("未找到UI预制件资源{0}！", m_prefabType.Name));
                }

                return;
            }
        }
        else
        {
            UIManager.Instance.UILoader().GetAsset(m_prefabType.Name + "_"+id, out obj, out lang);
            if (obj == null)
            {
                if (m_prefabType.BaseType != m_prefabType)
                {
                    Log.Message(string.Format("未找到UI预制件资源{0}_{1}！", m_prefabType.Name, id));
                    m_prefabType = m_prefabType.BaseType;
                    m_currentLayer = -1;
                    LoadLayer(id);
                }
                else
                {
                    Log.Error(string.Format("未找到UI预制件资源{0}_{1}！", m_prefabType.Name, id));
                }

                return;
            }

        }

        if (obj is GameObject)
        {
            if (m_gameObjectOuter != null)
            {
                DestroyImmediate();
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

            //前面加载了lang，这里放文本替换控件
            if (lang != null)
            {
                m_gameObjectOuter.AddComponent<TextReplaceControl>().setLanguageText(lang);
            }

            m_isDestroyed = false;
            ComponentInit();
            OnInit(m_gameObjectOuter);
        }
        else
        {
            Log.Error(string.Format("{0}_{1}资源并不是一个预制件, 而是{2}！", this.GetType().Name, id, obj.GetType().Name));
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
    /// 立刻马上销毁窗口，不调用Hide()
    /// </summary>
    public void DestroyImmediate()
    {
        OnDestroy();
        m_isDestroyed = true;
        DestoryAllChildComponent();
        GameObject.Destroy(m_gameObjectOuter);
        m_gameObjectOuter = null;
    }

    /// <summary>
    /// 销毁窗口，但是如果正在显示则会先调用Hide播放隐藏动画
    /// </summary>
    public void Destroy()
    {
        Hide();
        m_isDestroyed = true;
        ui_queue.EnqueueSimpleAction(DestroyImmediate);
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
    protected virtual void OnInit(GameObject m_go) { }

    /// <summary>
    /// 销毁时被调用
    /// </summary>
    protected virtual void OnDestroy() { }

    /// <summary>
    /// 窗口显示动画
    /// </summary>
    protected virtual IEnumerable<int> OnShow() { yield break; }

    /// <summary>
    /// 窗口隐藏动画
    /// </summary>
    protected virtual IEnumerable<int> OnHide() { yield break; }

    /// <summary>
    /// 显示组件
    /// </summary>
    public void Show()
    {
        if (m_isDestroyed || m_isShowing) return;
        m_isShowing = true;

        if (m_gameObjectOuter != null)//有一些UI组件压根没有m_gameObject_outer
        {
            m_gameObjectOuter.transform.localScale = Vector3.one;
        }

        ui_queue.EnqueueAction(OnShow());
    }

    /// <summary>
    /// 隐藏组件
    /// </summary>
    public void Hide()
    {
        if (m_isDestroyed || !m_isShowing) return;
        m_isShowing = false;
        ui_queue.EnqueueAction(OnHide());
        if (m_gameObjectOuter != null)//有一些UI组件压根没有m_gameObject_outer
        {
            ui_queue.EnqueueSimpleAction(() =>
            {
                if (m_gameObjectOuter != null)
                {
                    m_gameObjectOuter.transform.localScale = Vector3.zero;
                }
                
            });
        }

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
    /// 返回该UI当前语言的指定key文本。
    /// 如果语言key不存在，返回null。
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetLang(string key)
    {
        if (lang == null)
        {
            return null;
        }
        return lang.getValue(key);
    }

    /// <summary>
    /// 提供物件层级。
    /// </summary>
    protected virtual int SortOrder()
    {
        return 0;
    }

    public int CompareTo(object obj)
    {
        return -1 * SortOrder().CompareTo(((UIComponent)obj).SortOrder());
    }

    public GameObject getGameObject()
    {
        return m_gameObjectOuter;
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
        component.Show();
        m_childComponents.Add(component);//容器储存，方便集体销毁
        return component;
    }

    public T CreateUIComponent<T>(GameObject parent) where T : UIComponent, new()
    {
        RectTransform rect = parent.GetComponent<RectTransform>();
        if (rect == null)
        {
            Debug.LogError("UIComponent的根必须含有RectTransform控件！");
            return null;
        }
        return CreateUIComponent<T>(parent.GetComponent<RectTransform>());
    }

    /// <summary>
    /// 创建子控件，如果parent为空，则实例化在window同级目录。
    /// </summary>
    /// <typeparam name="T">控件类型</typeparam>
    /// <param name="path">路径</param>
    /// <returns></returns>
    protected UIComponent CreateUIComponent(Type t, RectTransform parent)
    {
        if(!typeof(UIComponent).IsAssignableFrom(t))
        {
            Debug.LogError(t.Name + "并不是UIComponent类型！");
            return null;
        }
        UIComponent component = (UIComponent)System.Activator.CreateInstance(t);
        component.m_root = this;
        component.LoadLayer(0);//初始化
        if (parent != null)
        {
            component.SetLocation(parent, Vector2.zero);//设置父组件和坐标
        }
        component.Show();
        m_childComponents.Add(component);//容器储存，方便集体销毁
        return component;
    }

    protected UIComponent CreateUIComponent(Type t, GameObject parent)
    {
        RectTransform rect = parent.GetComponent<RectTransform>();
        if (rect == null)
        {
            Debug.LogError("UIComponent的根必须含有RectTransform控件！");
            return null;
        }

        return CreateUIComponent(t, parent.GetComponent<RectTransform>());
    }

    /// <summary>
    /// 销毁改UIWindow下的所有子控件
    /// </summary>
    public void DestoryAllChildComponent()
    {
        foreach (UIComponent component in m_childComponents)
        {
            component.DestroyImmediate();
        }
        m_childComponents.Clear();
    }

    public void DestoryChildComponent(UIComponent comp)
    {
        m_childComponents.Remove(comp);
        comp.DestroyImmediate();

    }
}
