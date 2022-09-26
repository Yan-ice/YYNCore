using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.IO;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundlePack")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/AssetBundlePack";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        compFile("Assets/AssetBundle");
        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
                                        BuildAssetBundleOptions.ChunkBasedCompression,
                                        BuildTarget.StandaloneWindows);
        decompFile("Assets/AssetBundle");
    }

    static void compFile(string dir)
    {
        foreach(string d in Directory.GetDirectories(dir))
        {
            compFile(d);
        }
        foreach(string d in Directory.GetFiles(dir))
        {
            Debug.Log("file found: " + d);
            if (d.EndsWith(".cs"))
            {
                File.Move(d, d.Replace(".cs", ".txt"));
            }
        }
    }

    static void decompFile(string dir)    
    {
        foreach(string d in Directory.GetDirectories(dir))
        {
            decompFile(d);
        }
        foreach(string d in Directory.GetFiles(dir))
        {
            if (d.EndsWith(".txt"))
            {
                File.Move(d, d.Replace(".txt", ".cs"));
            }
        }
 
    }
}
