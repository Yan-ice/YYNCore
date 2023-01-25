using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Resources加载对资源加载器接口的实现。
/// 在构造函数中提供一级目录名。
/// 但是注意，Mainfest不可用。
/// </summary>
public class ResourcesLoader : ResourceLoaderInterface
{
    string catalog_name;

    public ResourcesLoader(string name)
    {
        catalog_name = name;
    }

    public object LoadResource(string path, Type t)
    {
        object o = null;
        if (typeof(UnityEngine.Object).IsAssignableFrom(t))
        {
            o = Resources.Load(catalog_name + "/" + path, t);
        }
        else if (typeof(JsonSerializable).IsAssignableFrom(t))
        {
            TextAsset txt = Resources.Load<TextAsset>(catalog_name + "/" + path);
            o = JsonConvert.DeserializeObject(txt.text, t);
        }

        return o;
    }

    public T LoadResource<T>(string path)
    {
        return (T)LoadResource(path, typeof(T));
    }

    public void SaveResource(string path, object resource)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 由于Resources的局限性，无法得到其清单。
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Manifest()
    {
        return new string[0];
    }

    public string PackName()
    {
        return catalog_name;
    }
}
