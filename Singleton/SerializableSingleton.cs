using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 在保证一个具体类只有唯一实例时，就可以继承此抽象类。
/// 继承时确保类具有无参数构造函数。
/// </summary>
/// <typeparam name="T">唯一实例的具体类</typeparam>
public abstract class SerializableSingleton<T> : JsonSerializable where T : SerializableSingleton<T>,new()
{
    private static T _instance;

   
    /// <summary>
    /// 获得唯一实例Instance。
    /// </summary>
    public static T Instance
    {
        get
        {
            if (null == _instance)
            {

                _instance = new T();

            }
            return _instance;
        }
    }

    public static bool hasInstance()
    {
        return _instance != null;
    }
    /// <summary>
    /// 手动构造这个对象。
    /// </summary>
    /// <returns>如果已经构造过，则返回false。</returns>
    public static bool Init()
    {
        if (null == _instance)
        {
            _instance = new T();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 从加载器指定路径反序列化这个对象。
    /// </summary>
    public static void Load(ResourceLoaderInterface loader)
    {
        if (null != _instance)
        {
            _instance.onReset();
        }

        _instance = loader.LoadResource<T>(typeof(T).Name);
    }

    /// <summary>
    /// 序列化这个对象，保存到加载器指定路径。
    /// 如果实例不存在，则保存null（删除文件）。
    /// </summary>
    /// <returns></returns>
    public static void Save(ResourceLoaderInterface saver)
    {
        if (null != _instance)
        {
            saver.SaveResource(typeof(T).Name, _instance);
        }
        else
        {
            saver.SaveResource(typeof(T).Name, null);
        }
    }

    /// <summary>
    /// 重置这个对象，原来的实例会丢失。
    /// </summary>
    public static void Reset()
    {
        if (_instance != null)
        {
            _instance.onReset();
        }
        _instance = null;
    }

    /// <summary>
    /// 当对象被重置时调用，
    /// 可以理解为单例对象的析构函数。
    /// </summary>
    protected virtual void onReset()
    {

    }
}
