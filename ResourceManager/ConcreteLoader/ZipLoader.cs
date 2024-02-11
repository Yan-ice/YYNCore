using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Drawing;

/// <summary>
/// Mods加载对资源加载器接口的实现。
/// 暂时还没做。
/// </summary>
public class ZipLoader : ResourceLoaderInterface
{
    string pack_name;

    Dictionary<string, byte[]> resources = new Dictionary<string, byte[]>();
    public ZipLoader(string zipPath)
    {
        //TODO: 名字待定
        pack_name = zipPath;

        ZipEntry entry;
        ZipInputStream zipInStream = new ZipInputStream(File.OpenRead(zipPath));
        //循环读取Zip目录下的所有文件
        while ((entry = zipInStream.GetNextEntry()) != null)
        {
            if (entry.IsFile)
            {
                byte[] data = new byte[entry.Size];
                zipInStream.Read(data, 0, data.Length);
                resources.Add(entry.Name, data);
            }
            
        }
        zipInStream.Close();
    }

   
    public Sprite GetSprite(byte[] bytes)
    {
        //MemoryStream ms = new MemoryStream(bytes);
        //Image image = System.Drawing.Image.FromStream(ms);

        //先创建一个Texture2D对象，用于把流数据转成Texture2D
        //Texture2D texture = new Texture2D(image.Width, image.Height);
        //texture.LoadImage(bytes);//流数据转换成Texture2D

        //创建一个Sprite,以Texture2D对象为基础
        //Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        //return sp;
        return null;
    }

    //这里需要手动实现每一种文件的反序列化。还挺烦的。
    public object LoadResource(string path, Type t)
    {
        byte[] b = resources[path];
        if (b != null)
        {
            if (t == typeof(Sprite))
            {
                return GetSprite(b);
            }
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

    public IEnumerable<string> Manifest()
    {
        return resources.Keys;
    }

    public string ResourcePackName()
    {
        return pack_name;
    }

    public void SaveResource(string path, object resource)
    {
        throw new NotImplementedException();
    }
}
