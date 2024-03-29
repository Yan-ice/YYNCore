﻿using System;

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class AddComponent
{

    [MenuItem("GameObject/CustomUI/AnimationText")]
    public static void LoadObjectAni()
    {
        var root = Selection.activeTransform;
        
        GameObject comp = GameObject.Instantiate(Resources.Load<GameObject>("UIPrefab/AnimationText"), root);
        comp.name = "AnimationText";
    }

    [MenuItem("GameObject/CustomUI/LongPressButton")]
    public static void LoadObjectLon()
    {
        var root = Selection.activeTransform;
        GameObject comp = GameObject.Instantiate(Resources.Load<GameObject>("UIPrefab/LongPressButton"), root);
        comp.name = "LongPressButton";
    }

    [MenuItem("GameObject/CustomUI/AutoFitText")]
    public static void LoadObjectAutoF()
    {
        var root = Selection.activeTransform;
        GameObject comp = GameObject.Instantiate(Resources.Load<GameObject>("UIPrefab/AutoFitText"), root);
        comp.name = "AutoFitText";
    }

    [MenuItem("GameObject/CustomUI/AutoFitBoarder")]
    public static void LoadObjectAutiFB()
    {
        var root = Selection.activeTransform;
        GameObject comp = GameObject.Instantiate(Resources.Load<GameObject>("UIPrefab/AutoFitBoarder"), root);
        comp.name = "AutoFitBoarder";
    }

    [MenuItem("GameObject/CustomUI/PageSelector")]
    public static void LoadObjectPageS()
    {
        var root = Selection.activeTransform;
        GameObject comp = GameObject.Instantiate(Resources.Load<GameObject>("UIPrefab/PageSelector"), root);
        comp.name = "PageSelector";
    }
}
#endif
