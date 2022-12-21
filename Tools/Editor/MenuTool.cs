using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class MenuTool
{

    [MenuItem("Help/区域选取工具", priority = 49)]
    public static void OpenWindow()
    {
        if (Application.isPlaying)
        {
            UIManager.Instance.ShowWindow<RectSelecter>();
        }
        else
        {
            Debug.Log("在游戏模式下才可以使用本工具！");
        }
    }

}
#endif

