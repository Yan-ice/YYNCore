using System.IO;
using UnityEngine;
using System;

/// <summary>
/// 该类用于输出信息到控制台。
/// </summary>
public class Log
{
    public static string m_logTime = "log_" + System.DateTime.Now.Month + "_" + System.DateTime.Now.Day + "_" + System.DateTime.Now.ToLongTimeString();
    public static bool m_useCmd = true;
    public static bool m_useFile = false;
    private static TstConsole m_console;

    /// <summary>
    /// 发送标准错误信息。
    /// </summary>
    /// <param name="str">信息</param>
    public static void Error(string str)
    {
        Loom.QueueOnMainThreadIfAsync(() =>
        {
            string mes = "[ERROR " + System.DateTime.Now.ToLongTimeString() + "] " + str;
            if (m_useCmd)
            {
                Debug.LogError(str);
            }
            if (m_console == null)
            {
                m_console = UIManager.Instance.GetWindow<TstConsole>();
            }
            m_console.PushLog(str, false);
        });
    }

    /// <summary>
    /// 发送标准警告信息。
    /// </summary>
    /// <param name="str">信息</param>
    public static void Assertion(string str)
    {
        Loom.QueueOnMainThreadIfAsync(() =>
        {
            string mes = "[WARN " + System.DateTime.Now.ToLongTimeString() + "] " + str;
            if (m_useCmd)
            {
                Debug.Log(str);
            }
            if (m_console == null)
            {
                m_console = UIManager.Instance.GetWindow<TstConsole>();
            }
            m_console.PushLog(str, false);
        });

    }

    /// <summary>
    /// 发送标准信息。
    /// </summary>
    /// <param name="str">信息</param>
    public static void Message(string str)
    {
        Loom.QueueOnMainThreadIfAsync(() =>
        {
            string mes = "[" + System.DateTime.Now.ToLongTimeString() + "] " + str;
            if (m_useCmd)
            {
                Debug.Log(str);
            }
            if (m_console == null)
            {
                m_console = UIManager.Instance.GetWindow<TstConsole>();
            }
            m_console.PushLog(str, false);
        });

    }
}
