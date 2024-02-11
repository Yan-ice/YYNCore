using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Drawing;

/// <summary>
/// Json加载对资源加载器接口的实现。
/// 暂时还没做。
/// </summary>
public class RawJsonFileLoader : ResourceLoaderInterface
{
    string pack_name;

    string pack_path;
    public RawJsonFileLoader(string directoryPath)
    {
        if (!directoryPath.EndsWith("/"))
        {
            directoryPath += "/";
        }
        pack_path = directoryPath;
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        //TODO: 名字待定
        pack_name = directoryPath;
    }

    //这里需要手动实现每一种文件的反序列化。还挺烦的。
    public object LoadResource(string path, Type t)
    {
        path = pack_path + path;
        if (File.Exists(path))
        {
            FileStream f = File.OpenRead(path);
            byte[] b = new byte[f.Length];
            f.Read(b, 0, b.Length);
            f.Close();

            if (typeof(JsonSerializable).IsAssignableFrom(t))
            {
                string s = System.Text.Encoding.UTF8.GetString(b);
                return JsonConvert.DeserializeObject(s, t);
            }
        }
        
        return null;
    }

    public T LoadResource<T>(string path)
    {
        return (T)LoadResource(path, typeof(T));
    }

    public void SaveResource(string path, object resource)
    {
        path = pack_path + path;
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        if (resource != null)
        {
            FileStream f = File.Create(path);

            if (resource is JsonSerializable)
            {
                string s = JsonConvert.SerializeObject(resource);
                byte[] b = System.Text.Encoding.UTF8.GetBytes(s);
                f.Write(b, 0, b.Length);

            }
            f.Close();
        }
        
    }

    private void recurseFile(string dir, List<string> manifest)
    {
        foreach (string f in Directory.GetFiles(dir))
        {
            manifest.Add(f);
        }
        foreach (string f in Directory.GetDirectories(dir))
        {
            recurseFile(f, manifest);
        }
    }

    public IEnumerable<string> Manifest()
    {
        List<string> manifest = new List<string>();
        recurseFile(pack_path, manifest);
        return manifest;
    }

    public string ResourcePackName()
    {
        return pack_name;
    }

    
}
