using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InputOperation
{
    public static InputOperation DefineOperation(string code)
    {
        InputOperation oper = new InputOperation(code);
        InputMapping.Instance.RegisterOperation(oper);
        return oper;
    }

    public string code;
    private InputOperation(string code)
    {
        this.code = code;
    }

    /// <summary>
    /// 从language的input组获得该操作的显示名。
    /// 如果在language未定义，则直接返回code。
    /// </summary>
    /// <returns></returns>
    public string getDisplay()
    {
        //TODO: 尚未解决。
        //string display = LanguageText.getValue("input", code);
        //return display == null ? code : display;
        return "";
    }
}