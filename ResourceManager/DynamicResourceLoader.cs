using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class DynResource
{
    ResourceLoaderInterface loader;
    public string asset_path;
    public string asset_name;
    public DynResource(ResourceLoaderInterface loader, string path)
    {
        this.loader = loader;
        asset_path = path;

        string[] s = path.Split(new char[] { '\\', '/', '.' });
        asset_name = s[s.Length - 2];
    }

    object asset;
    public object LoadAsset(Type t)
    {
        if (asset == null)
        {
            asset = loader.LoadResource(asset_path, t);
        }
        if (asset == null)
        {
            asset_name = "__ignored__";
            return null;
        }
        if (asset.GetType() == t)
        {
            return asset;
        }
        else
        {
            Debug.LogError("无法将一个资源加载为两种类型！原加载类型:" + asset.GetType().Name + " 新加载类型:" + t.Name);
            return null;
        }
    }

    public object GetAsset()
    {
        return asset;
    }
}

public class DynamicResourceLoader
{
    string pack_name;
    List<DynResource> manifest = new List<DynResource>();

    /// <summary>
    /// 将一个基本资源加载器封装为动态加载器。
    /// 在动态加载器中，文件的路径仅被用于区分文件类型，而可以根据文件名一次性检索到多种资源。
    /// 
    /// 注：动态加载器依赖于基本加载器的Manifest函数。
    /// </summary>
    /// <param name="loader">基本资源加载器</param>
    /// <param name="load_rule">加载规则，key是路径目录，value是目录下资源的类型。</param>
    public DynamicResourceLoader(ResourceLoaderInterface loader, Dictionary<string, Type> load_rule)
    {
        if(loader is ResourcesLoader)
        {
            Debug.LogAssertion("ResourcesLoader缺少Manifest，无法进行动态加载！");
        }

        pack_name = loader.PackName();
        foreach (string s in loader.Manifest())
        {
            manifest.Add(new DynResource(loader, s));
        }

        if (load_rule != null)
        {
            foreach(string s in load_rule.Keys)
            {
                LoadByResourcePath(s, load_rule[s]);
            }
        }
    }
    public List<string> Manifest()
    {
        List<string> s = new List<string>();
        foreach(DynResource r in manifest)
        {
            s.Add(r.asset_path);
        }
        return s;
    }
    public string PackName()
    {
        return pack_name;
    }

    /// <summary>
    /// 加载指定路径下的所有资源为某一类型。
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="path">指定路径</param>
    public List<T> LoadByResourcePath<T>(string path)
    {
        List<T> ls = new List<T>();
        foreach (object o in LoadByResourcePath(path, typeof(T)))
        {
            ls.Add((T)o);
        }
        return ls;
    }

    /// <summary>
    /// 加载指定路径的所有资源为某一类型。注意，这里的路径对大小写不敏感。
    /// 注意：如果t是一个JSON可序列化的类，您必须加载文本类型资源，然后将文本反序列化。
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="path">指定路径</param>
    public List<object> LoadByResourcePath(string path, Type t)
    {
        List<object> assets = new List<object>();
        foreach (DynResource asset in manifest)
        {
            if (asset.asset_path.ToLower().StartsWith(path.ToLower()))
            {
                assets.Add(asset.LoadAsset(t));
            }
        }
        return assets;
    }

    /// <summary>
    /// 获得一个路径下指定名字的所有资源。
    /// 注意，名字和路径均对大小写不敏感。
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="packName">包名</param>
    /// <returns>所有资源</returns>
    private List<object> LoadByResourceName(string name, string path)
    {
        List<object> assets = new List<object>();
        foreach (DynResource asset in manifest)
        {
            if (asset.asset_name.ToLower().Equals(name.ToLower()) && asset.asset_path.ToLower().StartsWith(path.ToLower()))
            {
                if(asset.GetAsset()!=null)
                    assets.Add(asset.GetAsset());
            }
        }
        return assets;
    }



    /// <summary>
    /// 获得指定目录下指定名字指定类型的资源。
    /// 如果出现多个同包+同名+同类型资源，则只加载最后一个。
    /// 如果未加载成功，则返回false，否则返回true。
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="fileName">文件名</param>
    /// <param name="packName">包名</param>
    public bool GetAsset<T>(string fileName, out T resource1, string path = "")
    {
        resource1 = default(T);
        bool l1 = false;
        foreach (object asset in LoadByResourceName(fileName, path))
        {
            if (asset.GetType() == typeof(T))
            {
                resource1 = (T)asset;
                l1 = true;
            }
        }

        return l1;
    }

    /// <summary>
    /// 获得指定目录下指定名字指定类型的资源。
    /// 如果出现多个同包+同名+同类型资源，则只加载最后一个。
    /// 如果有任何一个资源未加载成功，则返回false，否则返回true。
    /// </summary>
    /// <typeparam name="T">资源1类型</typeparam>
    /// <typeparam name="K">资源2类型</typeparam>
    /// <param name="fileName">文件名</param>
    /// <param name="packName">包名</param>
    public bool GetAsset<T, K>(string fileName, out T resource1, out K resource2, string path = "")
    {
        resource1 = default(T);
        resource2 = default(K);
        bool l1 = false, l2 = false;
        foreach (object asset in LoadByResourceName(fileName, path))
        {
            if (asset.GetType() == typeof(T))
            {
                resource1 = (T)asset;
                l1 = true;
            }
            if (asset.GetType() == typeof(K))
            {
                resource2 = (K)asset;
                l2 = true;
            }
        }

        return l1 & l2;
    }

    /// <summary>
    /// 获得指定目录下指定名字指定类型的资源。
    /// 如果出现多个同包+同名+同类型资源，则只加载最后一个。
    /// 如果有任何一个资源未加载成功，则返回false，否则返回true。
    /// </summary>
    /// <typeparam name="T">资源1类型</typeparam>
    /// <typeparam name="K">资源2类型</typeparam>
    /// <typeparam name="M">资源3类型</typeparam>
    /// <param name="fileName">文件名</param>
    /// <param name="packName">包名</param>
    public bool GetAsset<T, K, M>(string fileName, out T resource1, out K resource2, out M resource3, string path = "")
    {
        resource1 = default(T);
        resource2 = default(K);
        resource3 = default(M);
        bool l1 = false, l2 = false, l3 = false;

        foreach (object asset in LoadByResourceName(fileName, path))
        {
            if (asset.GetType() == typeof(T))
            {
                resource1 = (T)asset;
                l1 = true;
            }
            if (asset.GetType() == typeof(K))
            {
                resource2 = (K)asset;
                l2 = true;
            }
            if (asset.GetType() == typeof(M))
            {
                resource3 = (M)asset;
                l3 = true;
            }
        }

        return l1 & l2 & l3;
    }

}


/// <summary>
/// 使用该类管理包时，应注意: 该工具暂不支持类别分包,仅支持场景分包！
/// 即：如果将三类资源分在三个包内,用该类管理会寄！
/// </summary>
public class DynamicLoaderGroup
{
    static Dictionary<string, Type> load_rule;

    List<DynamicResourceLoader> bdlist = new List<DynamicResourceLoader>();

    /// <summary>
    /// 设定资源加载规则字典。
    /// 字典的Key是目录路径，字典的Value表示该路径下资源的类型。
    /// </summary>
    /// <param name="load_rule"></param>
    public DynamicLoaderGroup(Dictionary<string,Type> load_rule)
    {
        DynamicLoaderGroup.load_rule = load_rule;
        foreach(DynamicResourceLoader bd in bdlist)
        {
            foreach(string rule in load_rule.Keys)
            {
                bd.LoadByResourcePath(rule, load_rule[rule]);
            }
        }
    }

    /// <summary>
    /// 在组中添加一个加载器。
    /// 该加载器会自动升级为动态加载器。
    /// </summary>
    /// <param name="loader"></param>
    public void AddLoader(ResourceLoaderInterface loader)
    {
        DynamicResourceLoader dyn_loader = new DynamicResourceLoader(loader, load_rule);
        bdlist.Add(dyn_loader);
        foreach (string rule in load_rule.Keys)
        {
            dyn_loader.LoadByResourcePath(rule, load_rule[rule]);
        }
    }

    /// <summary>
    /// 获得组内指定名字指定类型的资源(无论路径)。
    /// 如果出现多个同包+同名+同类型资源，则只加载最后一个。
    /// 注意：
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="fileName">文件名</param>
    /// <param name="packName">包名</param>
    public bool GetAsset<T>(string fileName, out T resource1)
    {
        resource1 = default(T);

        foreach(DynamicResourceLoader loader in bdlist)
        {
            if (loader.GetAsset(fileName, out resource1)) return true;
        }
        
        return false;
    }

    /// <summary>
    /// 获得指定包下指定名字指定类型的资源(无论路径)。
    /// 如果出现多个同包+同名+同类型资源，则只加载最后一个。
    /// 如果有任何一个资源未加载成功，则返回false，否则返回true。
    /// </summary>
    /// <typeparam name="T">资源1类型</typeparam>
    /// <typeparam name="K">资源2类型</typeparam>
    /// <param name="fileName">文件名</param>
    /// <param name="packName">包名</param>
    public bool GetAsset<T, K>(string fileName, out T resource1, out K resource2)
    {
        resource1 = default(T);
        resource2 = default(K);
        foreach (DynamicResourceLoader loader in bdlist)
        {
            if (loader.GetAsset(fileName, out resource1, out resource2)) return true;
        }
        return false;
    }

    /// <summary>
    /// 获得指定包下指定名字指定类型的资源(无论路径)。
    /// 如果出现多个同包+同名+同类型资源，则只加载最后一个。
    /// 如果有任何一个资源未加载成功，则返回false，否则返回true。
    /// </summary>
    /// <typeparam name="T">资源1类型</typeparam>
    /// <typeparam name="K">资源2类型</typeparam>
    /// <typeparam name="M">资源3类型</typeparam>
    /// <param name="fileName">文件名</param>
    /// <param name="packName">包名</param>
    public bool GetAsset<T, K, M>(string fileName, out T resource1, out K resource2, out M resource3)
    {
        resource1 = default(T);
        resource2 = default(K);
        resource3 = default(M);
        foreach (DynamicResourceLoader loader in bdlist)
        {
            if (loader.GetAsset(fileName, out resource1, out resource2, out resource3)) return true;
        }
        return false;
    }

}