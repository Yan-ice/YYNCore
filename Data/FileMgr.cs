﻿using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using SQLite4Unity3d;

using UnityEngine;
public class FileMgr
{
    public static bool IsFileInUse(string fileName)
    {
        fileName = convertToAbsolute(fileName);
        bool inUse = true;

        FileStream fs = null;
        try
        {
            fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            inUse = false;
        }
        catch
        { }
        finally
        {
            if (fs != null)
                fs.Close();
        }
        return inUse;
    }

    public static bool CopyOutFromAsset(string fileName, bool force)
    {
        string src = fileName;
        string des = Application.persistentDataPath + "/" + fileName;

        //des = Application.persistentDataPath + "/" + fileName;
        if (!File.Exists(Application.streamingAssetsPath + "/"+ src))
        {
            return false;
        }

        if (File.Exists(des))
        {
            if (force)
            {
                File.Delete(des);
            }
            else
            {
                return false;
            }

        }
        FileStream fsDes = File.Create(des);
        Debug.Log("extracting source from " + src);
        byte[] data = BetterStreamingAssets.ReadAllBytes(src);
        fsDes.Write(data, 0, data.Length);
        fsDes.Flush();
        fsDes.Close();
        return true;
    }
    /// <summary>
    /// 相对路径转绝对路径。
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string convertToAbsolute(string path)
    {
        if (path.StartsWith(Application.persistentDataPath))
        {
            return path;
        }
        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }
        return Application.persistentDataPath + "/" + path;
    }

    /// <summary>
    /// 创建文件夹。
    /// </summary>
    /// <param name="path"></param>
    public static void mkdir(string path)
    {
        path = convertToAbsolute(path);
        Debug.Log("creating dir " + path);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// 列出文件夹下的所有文件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string[] listFiles(string path)
    {
        path = convertToAbsolute(path);
        return Directory.GetFiles(path);
    }

    /// <summary>
    /// 列出文件夹下所有文件夹
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string[] listDirectories(string path)
    {
        path = convertToAbsolute(path);
        return Directory.GetDirectories(path);
    }
    /// <summary>
    /// 创建文件并获得流。如果创建失败，返回null。
    /// 如果replace为true，则文件存在会先删除；为false则文件存在时创建失败。
    /// </summary>
    /// <param name="path"></param>
    /// <param name="replace"></param>
    /// <returns></returns>
    public static FileStream openFile(string path, bool replace)
    {
        path = convertToAbsolute(path);
        if (File.Exists(path) && replace)
        {
            File.Delete(path);
        }
        return File.Open(path,FileMode.OpenOrCreate);
    }

  
    public static bool Exists(string path)
    {
        path = convertToAbsolute(path);
        return Directory.Exists(path) || File.Exists(path);
    }
    public static void saveJsonObjToFile<T>(string path, T obj)
    {
        string data = JsonConvert.SerializeObject(obj);
        byte[] binary = Encoding.UTF8.GetBytes(data);
        saveBinaryToFile(path, binary);
    }

    public static T readJsonObjectFromFile<T>(string path)
    {
        string data = Encoding.UTF8.GetString(readBinaryFromFile(path));
        if (data.Length > 1)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
        return default(T);
    }

    public static void saveBinaryToFile(string path, byte[] binary)
    {
        path = convertToAbsolute(path);
        FileStream s = openFile(path, true);
        s.Write(binary, 0, binary.Length);
        s.Close();
    }

    public static byte[] readBinaryFromFile(string path)
    {
        path = convertToAbsolute(path);
        FileStream f = openFile(path, false);
        f.Close();
        return File.ReadAllBytes(path);
    }

}
