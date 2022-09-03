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
        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
                                        BuildAssetBundleOptions.ChunkBasedCompression,
                                        BuildTarget.StandaloneWindows);
    }
}
