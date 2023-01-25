using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class LanguageText : JsonSerializable
{

    //当前语言
    public static string current_lang = null;

    //该文本组的字典
    public Dictionary<string, string> en_US;
    public Dictionary<string, string> zh_CN;

    /// <summary>
    /// 调用该函数获得字典的文本。
    /// 如果未找到文本，返回值为null。
    /// </summary>
    /// <param name="group"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public string getValue(string key)
    {
        Dictionary<string, string> cur_dictionary = null;
        switch (current_lang)
        {
            case "en_US":
                cur_dictionary = en_US;
                break;
            case "zh_CN":
                cur_dictionary = zh_CN;
                break;
        }
        if (cur_dictionary == null)
        {
            Debug.LogError("文本"+ current_lang + "尚不支持语言"+key+"!");
            return null;
        }

        if (cur_dictionary.ContainsKey(key))
        {
            return cur_dictionary[key];
        }
        return null;
    }

    public static void LoadLanguage(string key)
    {
        current_lang = key;
    }
}

