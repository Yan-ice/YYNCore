using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
[AddComponentMenu("UI/AnimationText")]
#endif
public class AnimationText : MonoBehaviour
{
    public string text
    {
        get
        {
            return str;
        }
        set
        {
            changeTextDirectly(value);
        }
    }

    private Text txt_comp;
    private Animator ani;
    // Start is called before the first frame update
    void Awake()
    {
        ani = this.GetComponent<Animator>();
        txt_comp = this.GetComponentInChildren<Text>();
        if (str != null)
        {
            txt_comp.text = str;
        }
    }

    string str;
    Color col = Color.white;
    public void changeText(string txt)
    {

        if (txt_comp.text == "x")
        {
            changeTextDirectly(txt);
            return;
        }
        if (str != txt)
        {
            str = txt;
            col = Color.white;
            ani.SetTrigger("update");
        }
    }
    public void changeText(string txt,Color color)
    {
        
        if(txt_comp.text == "x")
        {
            changeTextDirectly(txt);
            return;
        }
        if (str != txt)
        {
            str = txt;
            col = color;
            ani.SetTrigger("update");
        }
    }
    public void changeTextDirectly(string txt)
    {
        str = txt;
        if (txt_comp != null)
        {
            col = Color.white;
            txt_comp.text = txt;
        }
    }
    public void changeTextDirectly(string txt,Color color)
    {
        str = txt;
        if (txt_comp != null)
        {
            txt_comp.text = txt;
            col = color;
        }
    }
    protected void aniChange()
    {
        txt_comp.text = str;
        txt_comp.color = col;
    }
}
