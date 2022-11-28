using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class CoreInit : MonoBehaviour
{
    private static bool m_init = false;
    void Awake()
    {
        if (!m_init)
        {
            m_init = true;
            Log.Message("核心初始化");
            UIManager.Init();
            AudioSystem.Init();
            //用户需要初始化的调用
            OnInit();
        }
        Destroy(gameObject);
    }

    public abstract void OnInit();
}
