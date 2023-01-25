using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// AB包加载对资源加载器接口的实现。
/// 在构造函数中提供AB包路径。
/// </summary>
public class BundlePackLoader : ResourceLoaderInterface
{
    AssetBundle bundle;

    List<string> manifest = new List<string>();
    string prefix = null;

    public BundlePackLoader(string bundle_path)
    {
        bundle = AssetBundle.LoadFromFile(bundle_path);
        foreach(string ass in bundle.GetAllAssetNames())
        {
            if (prefix == null)
            {
                prefix = "";
                bool find = false;
                string[] s = ass.Split(new char[] { '\\', '/', '.' });
                foreach(string sim in s)
                {
                    prefix += sim + "/";
                    if (sim == bundle.name)
                    {
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    prefix = "";
                }
                Debug.Log("prefix: " + prefix);
            }
            
            manifest.Add(ass.Replace(prefix, ""));
        }
    }

    public object LoadResource(string path, Type t)
    {
        path = prefix+path.ToLower(); //AB包只支持小写，真狗

        object o = null;
        if (typeof(UnityEngine.Object).IsAssignableFrom(t))
        {
            o = bundle.LoadAsset(path, t);
        }
        else if (typeof(JsonSerializable).IsAssignableFrom(t))
        {
            TextAsset txt = bundle.LoadAsset<TextAsset>(path);
            o = JsonConvert.DeserializeObject(txt.text, t);
        }

        return o;
    }

    public T LoadResource<T>(string path)
    {
        return (T)LoadResource(path, typeof(T));
    }

    public IEnumerable<string> Manifest()
    {
        return manifest;
    }

    public string PackName()
    {
        return bundle.name;
    }

    public void SaveResource(string path, object resource)
    {
        throw new NotImplementedException();
    }
}
