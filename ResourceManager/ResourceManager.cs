﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



[Serializable]
public class JsonSet<T>
{
    public string m_tag;
    public List<T> m_list;
}

public class ResourceManager : Singleton<ResourceManager>
{

    /// <summary>
    /// 加载一个json对象资源
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="space">资源空间（文件夹）名</param>
    /// <param name="name">资源文件名</param>
    /// <returns>对象实例</returns>
    public T LoadJsonObject<T>(string space, string name){
        string path = "jsonObject/" + space + "/" + name;
        TextAsset txt = Resources.Load<TextAsset>(path);
        if (txt == null)
        {
            Debug.LogError("Could not find assets " + path);
            return default(T);
        }
        string t = txt.text;
        T myObject = JsonConvert.DeserializeObject<T>(t);
        //T myObject = JsonUtility.FromJson<T>(t);
        return myObject;
    }
    

    public JsonSet<T> LoadJsonObjects<T>(string space, string name)
    {
        string path = "jsonObject/" + space + "/" + name;
        TextAsset txt = Resources.Load<TextAsset>(path);
        if (txt == null)
        {
            Debug.LogError("Could not find assets " + path);
            return null;
        }
        string t = txt.text;
        JsonSet<T> myObject = JsonConvert.DeserializeObject<JsonSet<T>>(t);
        return myObject;
    }


    /// <summary>
    /// 打印一个json对象的字符串
    /// （这个函数不应该出现在这里 不过 暂时测试用 无所谓了）
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="obj">对象</param>
    public void PrintJsonObject<T>(T obj)
    {
        //string t = JsonUtility.ToJson(obj, true);
        string t = JsonConvert.SerializeObject(obj);
        //File.WriteAllText("Assets/Resources/jsonObject/Technology/Tech.json", t);
        Debug.Log(t);
    }
}