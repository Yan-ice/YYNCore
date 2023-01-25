using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MouseAction
{
    left_click, right_click, middle_click, scroll_up, scroll_down
}

public class InputMapping : MonoSingleton<InputMapping>
{
    Dictionary<string, InputOperation> oper_list = new Dictionary<string, InputOperation>();
    
    Dictionary<KeyCode, InputOperation> key_mapping = new Dictionary<KeyCode, InputOperation>();
    Dictionary<MouseAction, InputOperation> mouse_mapping = new Dictionary<MouseAction, InputOperation>();
    
    Dictionary<InputOperation, Event> event_mapping = new Dictionary<InputOperation, Event>();

    /// <summary>
    /// 注册一个新的操作（例如跳、前进、后退等。)
    /// 该函数仅由InputOperation内部调用。
    /// </summary>
    /// <param name="oper_name"></param>
    public void RegisterOperation(InputOperation op)
    {
        oper_list.Add(op.code, op);
    }

    public void bindKeyOperation(KeyCode code, InputOperation e)
    {
        if (key_mapping.ContainsKey(code))
        {
            key_mapping[code] = e;
        }
        else
        {
            key_mapping.Add(code, e);
        }
    }
    public void bindMouseOperation(MouseAction code, InputOperation e)
    {
        if (mouse_mapping.ContainsKey(code))
        {
            mouse_mapping[code] = e;
        }
        else
        {
            mouse_mapping.Add(code, e);
        }
    }
    public void linkEvent(InputOperation op, Event evt)
    {
        if (event_mapping.ContainsKey(op))
        {
            event_mapping[op] = evt;
        }
        else
        {
            event_mapping.Add(op, evt);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //扫描键盘按键
        foreach(KeyCode code in key_mapping.Keys)
        {
            if (Input.GetKeyDown(code))
            {
                EventCenter.Instance.trigger(event_mapping[key_mapping[code]]);
            }
        }

        //扫描鼠标按键
        for(int a = 0; a < 3; a++)
        {
            if (Input.GetMouseButtonDown(a))
            {
                EventCenter.Instance.trigger(event_mapping[mouse_mapping[(MouseAction)a]]);
            }
        }
        if (Input.mouseScrollDelta.y == 1)
        {
            EventCenter.Instance.trigger(event_mapping[mouse_mapping[MouseAction.scroll_up]]);
        }
        if (Input.mouseScrollDelta.y == -1)
        {
            EventCenter.Instance.trigger(event_mapping[mouse_mapping[MouseAction.scroll_down]]);
        }


    }
}
