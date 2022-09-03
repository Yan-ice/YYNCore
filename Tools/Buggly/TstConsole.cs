using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

class TstConsole : UIWindow
{
    private static StreamWriter writer;
    public static string log_time = "log_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Day + "_" + System.DateTime.Now.Hour 
        + "_" + System.DateTime.Now.Minute + "_" + System.DateTime.Now.Second;

    private string per_logs = "";
    private string logs = "";
    private int line = 0;

    #region 脚本工具生成的代码
    private ScrollRect m_scrollView;
    private Text m_textContent;
    private InputField m_inputInput;
    private Button m_btnSend;
    private Button m_btnHide;
    /*该方法会在初始化前被UIWindow自动调用。*/
    protected override void ComponentInit()
    {
        m_scrollView = FindChildComponent<ScrollRect>("m_scrollView");
        m_textContent = FindChildComponent<Text>("m_scrollView/Viewport/m_textContent");
        m_inputInput = FindChildComponent<InputField>("input/m_inputInput");
        m_btnSend = FindChildComponent<Button>("input/m_btnSend");
        m_btnHide = FindChildComponent<Button>("input/m_btnHide");
        m_btnSend.onClick.AddListener(OnClickSendBtn);
        m_btnHide.onClick.AddListener(OnClickHideBtn);
    }
    #endregion

    private void LogCallback(string condition, string stackTrace, LogType type)
    {

        if(type==LogType.Error || type == LogType.Exception)
        {
            PushLog(condition+ "  "+stackTrace,false);
        }
        else if(type == LogType.Assert || type==LogType.Warning)
        {
            PushLog(condition,false);
        }
    }

    public void PushLog(string log,bool hidden)
    {
#if UNITY_ANDROID
        
#else
        writer.WriteLine(log);
        writer.Flush();
#endif

        per_logs+=log + "\n";
        if(!hidden)
        {
            logs += log + "\n";
        }
        line = logs.Split('\n').Length;
        if (line > 200)
        {
            logs = logs.Substring(logs.IndexOf("\n")+1);
            logs = logs.Substring(logs.IndexOf("\n"));
        }
        if (IsShowing())
        {
            m_textContent.text = logs;
        }
    }

    #region 事件
    private void OnClickSendBtn()
	{
        string c = m_inputInput.text;
        Log.Message("[Command] #" + c);
        m_inputInput.text = "";
        InputCommand(c);
	}

    private void OnClickHideBtn()
    {
        Hide();
    }
    #endregion

    protected override int SortOrder()
    {
		return 10081;
    }

    protected override void OnShow()
    {
        m_scrollView.enabled = true;
        m_textContent.text = logs;
        m_textContent.transform.localPosition = Vector2.zero;
    }

    protected override void OnHide()
    {
        m_inputInput.text = "";
        m_scrollView.enabled = false;
    }

    
    protected override void OnInit(GameObject m_go)
    {
#if UNITY_ANDROID
        
#else
       if (!Directory.Exists("GameLogs"))
        {
            Directory.CreateDirectory("GameLogs");
        }
        writer = new StreamWriter("GameLogs/" + log_time + ".txt", true);
#endif

        m_scrollView.enabled = false;

#if UNITY_5
                    Application.logMessageReceived += LogCallback;  
#else
        Application.RegisterLogCallback(LogCallback);  
#endif
    }


    protected override void OnDestroy()
    {
        
    }
    public bool InputCommand(string c)
    {
        if (c.Contains("$")){
            bool ch= false;
            foreach(string s in c.Split('$'))
            {
                if (InputCommand(s))
                {
                    ch = true;
                }
            }
            return ch;
        }
        else
        {
            List<string> args = new List<string>();
            string label = "";
            if (c.Contains(" "))
            {
                string[] ags = c.Split(' ');
                bool first = true;
                foreach (string ag in ags)
                {
                    if (first)
                    {
                        label = ag;
                        first = false;
                    }
                    else
                    {
                        args.Add(ag);
                    }
                }
            }
            else
            {
                label = c;
            }
            return AnalysisCommand(label, args);
        }

    }

    private bool AnalysisCommand(string label,List<string> args)
    {
        if (args.Count == 0)
        {
            if (label.Equals("help"))
            {
                Log.Message("====指令帮助====");
                Log.Message("coin <amount>            获得若干金币");
                Log.Message("card <cardName>          获得指定卡牌，如The_Yue");
                Log.Message("user <cardName>          获得指定核心技能，如The_Yue");
                Log.Message("level <levelName>        解锁指定关卡，如Level1_2");
                Log.Message("chapter <chapterName>    解锁指定章节，如Chapter1");
                Log.Message("treat gain               获得全套卡牌。");
            }
            else
            {
                Log.Message("请输入参数。");
            }
            
            return false;
        }
        switch (label)
        {
            case "card":
            default:
                Log.Message("未知的命令。");
                return false;
        }
        
    }

}
