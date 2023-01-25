using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;

    private static readonly object m_locker = new object();

    private static bool m_appQuitting;

    public static void Init()
    {
        GameObject g = Instance.gameObject;
    }

    public static T Instance
    {
        get
        {
            if (m_appQuitting)
            {
                _instance = null;
                return _instance;
            }

            lock (m_locker)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (FindObjectsOfType<T>().Length > 1)
                    {
                        Log.Error("不应该存在多个单例！");
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        var singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton)" + typeof(T);
                        singleton.hideFlags = HideFlags.None;
                        DontDestroyOnLoad(singleton);
                    }
                    else
                        DontDestroyOnLoad(_instance.gameObject);
                }
                _instance.hideFlags = HideFlags.None;
                return _instance;
            }
        }
    }
    public static bool hasInstance()
    {
        return _instance != null;
    }

    protected virtual void onInit()
    {

    }

    private void Awake()
    {
        m_appQuitting = false;onInit();
    }

    private void OnDestroy()
    {
        m_appQuitting = true;
    }
}