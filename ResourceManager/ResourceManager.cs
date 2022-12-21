
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
    public List<T> m_list = new List<T>();
}

public class ResourceManager : Singleton<ResourceManager>
{

    public Sprite LoadImage(string space, string name)
    {
        string path = "image/" + space + "/" + name;
        Sprite txt = Resources.Load<Sprite>(path);
        if (txt == null)
        {
            Debug.LogAssertion("Could not find image assets " + path);
            return null;
        }
        return txt;
    }

    public GameObject LoadPrefab(string space, string name)
    {
        string path = "prefab/" + space + "/" + name;
        GameObject txt = Resources.Load<GameObject>(path);
        if (txt == null)
        {
            Debug.LogAssertion("Could not find prefab assets " + path);
            return null;
        }
        return txt;
    }
    public AudioClip LoadAudio(string space, string name)
    {
        string path = "Audio/" + space + "/" + name;
        AudioClip txt = Resources.Load<AudioClip>(path);
        if (txt == null)
        {
            Debug.LogAssertion("Could not find audio assets " + path);
            return null;
        }
        return txt;
    }

}
