using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

class TextReplaceControl : MonoBehaviour
{
    public void setLanguageText(LanguageText lang)
    {
        Text[] t = gameObject.GetComponentsInChildren<Text>();
        foreach (Text comp in t)
        {
            if (comp.text.StartsWith("$"))
            {
                TextReplacer tr = comp.gameObject.AddComponent<TextReplacer>();
                tr.setLanguageText(lang);
                tr.updateText();
            }
        }
        foreach(PageSelector sel in gameObject.GetComponentsInChildren<PageSelector>())
        {
            TextReplacer tr = sel.gameObject.AddComponent<TextReplacer>();
            tr.setLanguageText(lang);
            tr.updateText();
        }
    }

}

class TextReplacer : MonoBehaviour
{
    public List<string> keys = new List<string>();

    private LanguageText lang;
    public void setLanguageText(LanguageText lang)
    {
        this.lang = lang;
    }

    private static List<TextReplacer> rpc = new List<TextReplacer>();
    public static void UpdateLanguage()
    {
        foreach (TextReplacer r in rpc)
        {
            r.updateText();
        }
    }
    
    public void Start()
    {
        Text t = gameObject.GetComponent<Text>();
        if (t != null)
        {
            if (t.text.StartsWith("$"))
            {
                keys.Add(t.text.Substring(1));
            }
        }

        PageSelector sel = gameObject.GetComponent<PageSelector>();
        if (sel != null)
        {
            for(int a = 0; a < sel.candidate.Length; a++)
            {
                if (sel.candidate[a].StartsWith("$"))
                {
                    keys.Add(sel.candidate[a].Substring(1));
                }
            }
        }
        if(keys.Count == 0)
        {
            DestroyImmediate(this);
            return;
        }
        updateText();
        rpc.Add(this);
    }

    public void updateText()
    {

        Text t = gameObject.GetComponent<Text>();
        if (t != null)
        {
            if (t.text.StartsWith("$"))
            {
                keys.Add(t.text.Substring(1));
            }
            t.text = lang.getValue(keys[0]);
        }

        PageSelector sel = gameObject.GetComponent<PageSelector>();
        if (sel != null)
        {
            for (int a = 0; a < keys.Count; a++)
            {
                sel.candidate[a] = lang.getValue(keys[a]);
            }
        }

    }
    public void OnDestroy()
    {
        rpc.Remove(this);
    }

}