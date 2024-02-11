using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// AB包加载对资源加载器接口的实现。
/// 至少需要在构造函数中提供AB包路径。
/// 
/// 在此基础上，你可以额外指定AB包内部的一个路径，
/// 来进一步构造一个仅关注AB包中某个子目录的Loader。
/// </summary>
public class BundlePackLoader : ResourceLoaderInterface
{
    AssetBundle bundle;
    string sub_path;

    List<string> manifest = new List<string>();

    //特殊作用。
    List<string> prefixs = new List<string>();


    /// <summary>
    /// 根据AB包路径新建一个AB包资源加载器。
    /// </summary>
    /// <param name="bundle_path"></param>
    /// <param name="sub_path">若该参数存在，则加载器仅关注AB包内部的某个子目录。</param>
    public BundlePackLoader(string bundle_path, string sub_path = "")
    {
        this.sub_path = sub_path;
        bundle = AssetBundle.LoadFromFile(bundle_path);
        updateManifest();
    }

    private BundlePackLoader(){}

    private void updateManifest()
    {
        manifest.Clear();
        foreach (string ass in bundle.GetAllAssetNames())
        {
            bool contain_prefix = false;
            string prefix = "";
            foreach(string s in prefixs)
            {
                if (ass.StartsWith(s))
                {
                    contain_prefix = true;
                    prefix = s;
                }
            }
            if (!contain_prefix)
            {
                bool find = false;
                string[] s = ass.Split(new char[] { '\\', '/', '.' });
                foreach (string sim in s)
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
                Debug.Log("new prefix: " + prefix);
                prefixs.Add(prefix);
            }

            string asset_path = ass.Replace(prefix, "");
            if (asset_path.StartsWith(sub_path))
            {
                if (sub_path != "")
                {
                    asset_path = asset_path.Replace(sub_path, "");
                }
                manifest.Add(asset_path);
            }

        }
    }
    /// <summary>
    /// 指定该Loader中的一个子目录路径，
    /// 返回仅关注该子目录的更小范围的BundlePackLoader。
    /// </summary>
    /// <param name="sub_path"></param>
    /// <returns></returns>
    public BundlePackLoader forkLoader(string sub_path)
    {
        BundlePackLoader sub_loader =  new BundlePackLoader();

        //基本属性直接继承
        sub_loader.prefixs = prefixs;
        sub_loader.bundle = bundle;

        sub_loader.sub_path = this.sub_path + sub_path.TrimEnd('/','\\')+"/";

        sub_loader.updateManifest();
        return sub_loader;
    }
    public object LoadResource(string path, Type t)
    {
        object o = null;
        foreach (string prefix in prefixs)
        {
            string full_path = prefix + sub_path + path.ToLower(); //AB包只支持小写，真狗
            if (typeof(UnityEngine.Object).IsAssignableFrom(t))
            {
                o = bundle.LoadAsset(full_path, t);
            }
            else if (typeof(JsonSerializable).IsAssignableFrom(t))
            {
                TextAsset txt = bundle.LoadAsset<TextAsset>(full_path);
                o = JsonConvert.DeserializeObject(txt.text, t);
            }
            if (o != null)
            {
                return o;
            }
            
        }
        return null;
    }

    public T LoadResource<T>(string path)
    {
        return (T)LoadResource(path, typeof(T));
    }

    public IEnumerable<string> Manifest()
    {
        return manifest;
    }

    public string ResourcePackName()
    {
        return bundle.name+" ("+sub_path+")";
    }

    public void SaveResource(string path, object resource)
    {
        throw new NotImplementedException();
    }
}
