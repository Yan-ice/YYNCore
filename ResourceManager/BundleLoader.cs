using Newtonsoft.Json;
using RoslynCSharp;
using RoslynCSharp.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class BundleLoader : Singleton<BundleLoader>
{
    class BundleAsset
    {
        public BundleAsset(AssetBundle b, string f)
        {
            from_bundle = b;
            file_path = f;
        }
        AssetBundle from_bundle;
        public string file_path { get; private set; }
        public string file_name
        {
            get
            {
                string[] s = file_path.Split(new char[] { '\\', '/','.'});
                return s[s.Length - 2];
            }
        }
        public T GetAsset<T>() where T: UnityEngine.Object
        {
            T asset = from_bundle.LoadAsset<T>(file_path);
            if(asset == null)
            {
                Debug.LogError("找不到类型为"+typeof(T).Name+"的AB包资源" + file_path + "!");
            }
            return asset;
        }
    }

    private static Dictionary<string,List<BundleAsset>> tag_map = new Dictionary<string,List<BundleAsset>>();
    
    public static void initBundleLoader(string[] valid_tag)
    {
        tag_map.Clear();
        foreach(string s in valid_tag)
        {
            tag_map.Add(s, new List<BundleAsset>());
        }
    }

    private static ScriptDomain domain;

    /// <summary>
    /// 注意，static_resources是会被忽略的。
    /// </summary>
    /// <param name="bundle"></param>
    public static void AddBundle(AssetBundle bundle)
    {
        foreach (string file in bundle.GetAllAssetNames())
        {

            if(file.Split(new char[] { '\\', '/' }).Contains("static_resources"))
            {
                continue;
            }
            BundleAsset asset = new BundleAsset(bundle, file);
            Debug.Log("load resources: " + file);
            //遍历所有key tag，并将有效的放进去
            foreach (string key in file.Split(new char[] { '\\', '/' }))
            {
                
                if (tag_map.ContainsKey(key))
                {
                    tag_map[key].Add(asset);
                }
            }
        }
    }

    public static T loadAsset<T>(string tag, string filter, string file_name) where T : UnityEngine.Object
    {
        tag = tag.ToLower();
        filter = filter.ToLower();
        file_name = file_name.ToLower();
        if(!tag_map.ContainsKey(tag) || !tag_map.ContainsKey(filter))
        {
            Debug.Log(tag + "或" + filter + "不是一个有效标签！");
        }
        foreach (BundleAsset file in tag_map[tag])
        {
            if (filter == null || tag_map[filter].Contains(file))
            {
                if (file.file_name.Contains(file_name))
                {
                    return file.GetAsset<T>();
                }
            }
        }
        Debug.LogError("在AssetBundle中找不到资源"+file_name+"!");
        return null;
    }
    public static Dictionary<string, T> loadAssets<T>(string tag, string filter = null) where T : UnityEngine.Object
    {
        tag = tag.ToLower();
        filter = filter.ToLower();
        if (!tag_map.ContainsKey(tag) || !tag_map.ContainsKey(filter))
        {
            Debug.Log(tag + "或" + filter + "不是一个有效标签！");
        }
        Dictionary<string, T> asset_list = new Dictionary<string, T>();
        foreach (BundleAsset file in tag_map[tag])
        {
            if (filter==null || tag_map[filter].Contains(file))
            {
                asset_list.Add(file.file_name, file.GetAsset<T>());
            }
        }
        if (asset_list.Count == 0)
        {
            Debug.LogError("在AssetBundle中找不到资源集" + tag + "("+filter+")" + "!");
        }
        return asset_list;
    }

    public static Dictionary<string, string> loadTexts(string tag, string filter = null)
    {
        Dictionary<string, string> asset_list = new Dictionary<string, string>();
        foreach (var file in loadAssets<TextAsset>(tag, filter))
        {
            asset_list.Add(file.Key, file.Value.text);
        }
        return asset_list;
    }

    public static Dictionary<string, T> loadJsonObjects<T>(string type, string filter = "json")
    {
        Dictionary<string, T> asset_list = new Dictionary<string, T>();
        foreach (var file in loadTexts(type, filter))
        {

            asset_list.Add(file.Key, JsonConvert.DeserializeObject<T>(file.Value));
        }
        return asset_list;
    }

    /// <summary>
    /// 为了防止多次加载资源时的重复编译。
    /// </summary>
    private static Dictionary<string, ScriptType> _script_cache = new Dictionary<string, ScriptType>();

    public static Dictionary<string, ScriptType> loadScripts(string type, string filter = "scripts")
    {
        if (domain == null)
        {
            domain = ScriptDomain.CreateDomain("DynScriptDomain", true);
            domain.RegisterAssembly(typeof(CardData).Assembly, ScriptSecurityMode.EnsureLoad);
            IMetadataReferenceProvider reference = AssemblyReference.FromAssembly(typeof(BundleLoader).Assembly);
            domain.RoslynCompilerService.ReferenceAssemblies.Add(reference);
        }
        Dictionary<string, ScriptType> scriptx = new Dictionary<string, ScriptType>();
        foreach(var text in loadTexts(type, filter))
        {
            if (_script_cache.ContainsKey(text.Value))
            {
                scriptx.Add(text.Key, _script_cache[text.Value]);
            }
            else
            {
                scriptx.Add(text.Key, domain.CompileAndLoadMainSource(text.Value, ScriptSecurityMode.EnsureLoad));
            }
        }
        return scriptx;
    }

}

