using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.IO;

public class AssetBundleCompile
{
    static string source_root = "Assets/AssetBundle/";
    static string target_root = "Assets/AssetBundle/_compile/";

    public static void compileAB(string packname)
    {
        copyDir(packname, packname);
        AssetImporter assetImporter = AssetImporter.GetAtPath(target_root+packname);
        assetImporter.assetBundleName = packname;
    }

    static void copyDir(string dir, string packname)
    {
        string r_path = source_root + dir;
        string t_path = target_root + dir;

        if (!Directory.Exists(t_path))
        {
            Directory.CreateDirectory(t_path);
        }
        foreach (string d in Directory.GetDirectories(r_path))
        {
            string[] ss = d.Split(new char[] { '/', '\\' });
            string name = ss[ss.Length - 1];
            copyDir(dir+"/"+name, packname);
        }

        foreach (string d in Directory.GetFiles(r_path))
        {
            string[] ss = d.Split(new char[] { '/', '\\' });
            string name = ss[ss.Length - 1];
            if (name.EndsWith(".cs"))
            {
                copyFile(dir + "/" + name);
            }
            else if(!name.EndsWith(".meta"))
            {
                AssetImporter assetImporter = AssetImporter.GetAtPath(r_path+"/"+name);
                assetImporter.assetBundleName = packname;
            }
            
        }
    }

    static void copyFile(string file)
    {
        string r_path = source_root + file;
        string t_path = target_root + file;
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
    [MenuItem("Assets/Build AssetBundlePack")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "AssetBundlePack";
        if (Directory.Exists(assetBundleDirectory))
        {
            Directory.Delete(assetBundleDirectory, true);
        }
        Directory.CreateDirectory(assetBundleDirectory);

        AssetBundleCompile.compileAB("defaultpack");

        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
                                        BuildAssetBundleOptions.ChunkBasedCompression,
                                        BuildTarget.StandaloneWindows);
    }
}
