using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static Dictionary<string, Camera> cam = new Dictionary<string, Camera>();

    public static Camera getCamera(string key)
    {
        if (cam.ContainsKey(key))
        {
            return cam[key];
        }
        else
        {
            return null;
        }
    }

    private void Start()
    {
        cam.Add(gameObject.name, GetComponent<Camera>());
    }
    private void OnDestroy()
    {
        cam.Remove(gameObject.name);
    }
}