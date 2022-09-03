using System;
using UnityEngine;
using UnityEngine.UI;

public class ResourceLoader
{
    public static Sprite LoadImage(string imgName)
    {
        Sprite sp = Resources.Load<Sprite>("Image/" + imgName);
        if (sp == null)
        {
            Log.Assertion("未找到图片资源" + "Image/" + imgName);
        }
        return sp;
    }
    public static AudioClip LoadMusic(string musicName)
    {
        return Resources.Load<AudioClip>("Music/" + musicName);
    }
    public static AudioClip LoadSound(string musicName)
    {
        return Resources.Load<AudioClip>("Sound/" + musicName);
    }
    public static GameObject LoadPrefab(string prefabName)
    {
        return Resources.Load<GameObject>(prefabName);
    }

    public static Sprite LoadImage(string path,string imgName)
    {
        Sprite sp = Resources.Load<Sprite>("Image/"+path+"/" + imgName);
        if (sp == null)
        {
            Log.Assertion("未找到图片资源" + "Image/" + path + "/" + imgName);
        }
        return sp;
    }
    public static GameObject LoadPrefab(string path, string prefabName)
    {
        return Resources.Load<GameObject>(path+"/"+prefabName);
    }
}
