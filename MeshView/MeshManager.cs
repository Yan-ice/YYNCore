﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MeshManager : Singleton<MeshManager>
{

    public static string path = "MeshPrefab/";

    Dictionary<Type, Dictionary<object, MonoBehaviour>> hash = new Dictionary<Type, Dictionary<object, MonoBehaviour>>();

    /// <summary>
    /// 获得与脚本关联的Mono。脚本仅与Mono单向关联。
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public U GetLinking<U>(MonoScript<U> obj) where U : MonoBehaviour
    {
        if(!hash.ContainsKey(obj.GetType()))
        {
            hash.Add(obj.GetType(), new Dictionary<object, MonoBehaviour>());
        }
        Dictionary<object, MonoBehaviour> link = hash[obj.GetType()];

        if (link.ContainsKey(obj))
        {
            U linkd = (U)link[obj];
            if (linkd != null)
            {
                return (U)link[obj];
            }
            else
            {
                link.Remove(obj);
                return GetLinking(obj);
            }
        }
        else
        {
            GameObject go = LoadMesh(typeof(U).Name);
            if (go == null)
            {
                Debug.LogError("未找到mesh预制件" + typeof(U).Name + "!");
                return null;
            }
            if(go.GetComponent<U>() == null)
            {
                go.AddComponent<U>();
            }
            link.Add(obj,go.GetComponent<U>());
            return go.GetComponent<U>();
        }

    }

    /// <summary>
    /// 销毁脚本关联的Mono。
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <param name="obj"></param>
    public void Unlink<U>(MonoScript<U> obj) where U:MonoBehaviour
    {
        if (!hash.ContainsKey(obj.GetType()))
        {
            return;
        }
        Dictionary<object, MonoBehaviour> link = hash[obj.GetType()];

        if (link.ContainsKey(obj))
        {
            GameObject.DestroyImmediate((U)link[obj]);
            link.Remove(obj);
        }
    }

    private GameObject LoadMesh(string mesh)
    {
        GameObject go = (GameObject)Resources.Load(string.Format("{0}{1}", path, mesh));
        return GameObject.Instantiate(go);
    }
}