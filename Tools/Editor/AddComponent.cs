using System;

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class AddComponent
{

    [MenuItem("GameObject/CustomUI/AnimationText")]
    public static void LoadObjectAni()
    {
        var root = Selection.activeTransform;
        GameObject comp = GameObject.Instantiate(ResourceLoader.LoadPrefab("UIPrefab/AnimationText"), root);
        comp.name = "AnimationText";
    }

    [MenuItem("GameObject/CustomUI/LongPressButton")]
    public static void LoadObjectLon()
    {
        var root = Selection.activeTransform;
        GameObject comp = GameObject.Instantiate(ResourceLoader.LoadPrefab("UIPrefab/LongPressButton"), root);
        comp.name = "LongPressButton";
    }

    [MenuItem("GameObject/CustomUI/AutoFitText")]
    public static void LoadObjectAutoF()
    {
        var root = Selection.activeTransform;
        GameObject comp = GameObject.Instantiate(ResourceLoader.LoadPrefab("UIPrefab/AutoFitText"), root);
        comp.name = "AutoFitText";
    }
}
#endif
