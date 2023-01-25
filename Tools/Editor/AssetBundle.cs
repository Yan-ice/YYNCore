using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.IO;
using System;

public class AssetBundleCompile
{
    static string target_root = "Assets/AssetBundle/_compile";

    public static void compileAB(string root, string packname)
    {
        Debug.Log("compiling " + packname);
        copyDir(packname, root+"/"+packname, target_root+"/"+ packname);
        try
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(target_root + "/" + packname);
            assetImporter.assetBundleName = packname;
        }
        catch (Exception) {
            Debug.LogAssertion("由于Unity无法立刻加载生成的文件，您需要Unity重载一次asset，然后再打包一次。");
        }
        
    }

    static void copyDir(string packname, string r_path, string t_path)
    {
        if (!Directory.Exists(t_path))
        {
            Directory.CreateDirectory(t_path);
        }
        foreach (string d in Directory.GetDirectories(r_path))
        {
            string[] ss = d.Split(new char[] { '/', '\\' });
            string name = ss[ss.Length - 1];
            copyDir(packname, d, t_path+"/"+name);
        }

        foreach (string d in Directory.GetFiles(r_path))
        {
            string[] ss = d.Split(new char[] { '/', '\\' });
            string name = ss[ss.Length - 1];
            if (name.EndsWith(".cs"))
            {
                copyFile(d, t_path + "/" + name);
            }
            else if(!name.EndsWith(".meta"))
            {
                AssetImporter assetImporter = AssetImporter.GetAtPath(r_path+"/"+name);
                assetImporter.assetBundleName = packname;
            }
            
        }
    }

    static void copyFile(string r_path, string t_path)
    {
        if (t_path.EndsWith(".meta"))
        {
            return;
        }
        if (t_path.EndsWith(".cs"))
        {
            t_path = t_path.Replace(".cs", ".txt");
        }

        if (File.Exists(t_path))
        {
            File.Delete(t_path);
        }
        File.Copy(r_path, t_path);
    }
}

public class CreateAssetBundles
{
    public static string resource_root_name = "BundleData";

    /// <summary>
    /// 获得Assets内的所有资源根目录。
    /// </summary>
    /// <param name="root">Assets</param>
    /// <param name="result">(out)用于存放目录路径列表</param>
    public static void findResourceRoot(string root, List<string> result)
    {
        foreach(string pth in Directory.GetDirectories(root))
        {
            if (pth.EndsWith(resource_root_name))
            {
                result.Add(pth);
                continue;
            }
            findResourceRoot(pth,result);
        }
    }

    /// <summary>
    /// 标记所有资源包名，并返回资源包名列表。
    /// </summary>
    /// <param name="pack_namelist"></param>
    public static void MarkResources(List<string> pack_namelist)
    {
        List<string> roots = new List<string>();
        findResourceRoot("Assets",roots);

        foreach(string root in roots)
        {
            foreach(string pack in Directory.GetDirectories(root))
            {
                string pack_name = pack.Replace(root, "").Substring(1);
                AssetBundleCompile.compileAB(root, pack_name);

                AssetImporter assetImporter = AssetImporter.GetAtPath(pack);
                assetImporter.assetBundleName = pack_name;
                if (!pack_namelist.Contains(pack_name))
                {
                    pack_namelist.Add(pack_name);
                }
            }
        }
    }

    [MenuItem("Assets/Build Bundle Pack")]
    static void BuildPack()
    {
        List<string> packs = new List<string>();
        MarkResources(packs);

        string bundleOutDirectory = "AssetBundlePack";
        if (Directory.Exists(bundleOutDirectory))
        {
            Directory.Delete(bundleOutDirectory, true);
        }
        Directory.CreateDirectory(bundleOutDirectory);

        foreach (string pack in packs)
        {
            
            BuildPipeline.BuildAssetBundles(bundleOutDirectory,
                                            BuildAssetBundleOptions.ChunkBasedCompression,
                                            BuildTarget.StandaloneWindows);
        }

    }

}
