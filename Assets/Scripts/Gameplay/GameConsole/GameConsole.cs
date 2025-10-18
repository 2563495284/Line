using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum LogMsgType
{
    Info,
    Warn,
    Error,
    Echo,//命令回显
    Return,//命令返回值
    Start,//某段进程的开始
    End,//某段进程的结束
    Test,//测试
    Spec,//自定义颜色
}
public class GameConsole : MonoBehaviour
{
    public ConsoleLogList list;
    public List<LogData> Datas => list.Logs;
    public CommandLineCtrl cmd;
    public Image modal;
    public bool isTrackCoroutine = true;
    public bool isLogInUnityConsole = false;
    public void Init()
    {
        list.Init();
    }
    public void BlockRaycast()
    {
        modal.raycastTarget = true;
    }
    public void UnblockRaycast()
    {
        modal.raycastTarget = false;
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void ClearLog()
    {
        list.ClearLogs();
    }
    public static void LogErrorSafeInUnity(string msg)
    {
        Debug.LogWarning($"<color=red>{msg}</color>");
    }
    public void AddLog(string logMsg, LogMsgType type, Color? specColor = null)
    {

        list.AddLog(new LogData
        {
            message = logMsg,
            logType = type,
            timestamp = TimestampTool.Now()
        }, specColor);
        if (isLogInUnityConsole)
        {
            string msg = "【Command Output】" + logMsg;
            switch (type)
            {
                case LogMsgType.Error:
                    LogErrorSafeInUnity(msg);
                    break;
                case LogMsgType.Warn:
                    Debug.LogWarning(msg);
                    break;
                default:
                    Debug.Log(msg);
                    break;
            }
        }
    }
}
