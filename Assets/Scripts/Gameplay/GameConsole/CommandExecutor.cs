using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
public static class ConsoleArgsEnum
{
    public const string
        NotifyKey = "notifyKey",
        LevelCmd = "levelCmd",
        LevelModel = "levelModel";
}
//参数标准
//NotifyConst枚举：notifyKey
//LevelCommand枚举：levelCmd

//添加新命令规范：
//函数名必须和Command指令名一样,首字母必须大写
//函数必须为public
//参数类型必须为string，但数量可变
public class CommandExecutor
{
    private GameConsole Console => UIManager.Ins.console;
    private CommandLibrary CmdLibrary { get; }
    private CommandLineCtrl Ctrl { get; }
    public CommandExecutor(CommandLibrary lib, CommandLineCtrl ctrl)
    {
        CmdLibrary = lib;
        Ctrl = ctrl;
    }
    [ConsoleCommand("帮助，查看所有命令及使用方法")]
    public void Help()
    {

        var commands = CmdLibrary.GetAllCommands().OrderBy(c => c.Command);
        string str = "";
        int cnt = 0;
        foreach (var cmd in commands)
        {
            string argsDesc = cmd.Arguments.Count > 0
                ? " " + string.Join(" ", cmd.Arguments.Select(a => $"<{(a.IsRequired ? "" : "optional ")}param {a.Name}>"))
                : "";
            str += $"\n{cmd.Command}{argsDesc} - {cmd.Description}";
            cnt++;
        }
        Console.AddLog($"共{cnt}条指令：{str}", LogMsgType.Return);

    }
    [ConsoleCommand("清除控制台")]
    public void Clear()
    {
        Console.ClearLog();
    }
    [ConsoleCommand("取消控制台遮挡，可穿透日志窗口进行游戏操作")]
    public void Unblock()
    {
        Console.UnblockRaycast();
    }
    [ConsoleCommand("开启控制台遮挡，开启日志窗口时将屏蔽游戏点击操作")]
    public void Block()
    {
        Console.BlockRaycast();
    }
    [ConsoleCommand("查看已执行的命令历史")]
    public void History()
    {
        for (int i = 0; i < Ctrl.CmdHistory.Count; i++)
        {
            Console.AddLog($"{i + 1}. {Ctrl.CmdHistory[i]}", LogMsgType.Return);
        }
    }

    [ConsoleCommand("通知UI发送事件")]
    public void Notify(string notifyKey)
    {
        if (GM.Ins.Level == null)
        {
            Console.AddLog("没有对局信息", LogMsgType.Error);
            return;
        }
        GM.Ins.Level.Notify(notifyKey);
    }
    [ConsoleCommand("发送游戏指令")]
    public void Execute(string levelCmd)
    {
        LevelCommand cmd = Activator.CreateInstance(LoadManager.Ins.GetDynamicClass(EDynamicSerial.LevelCommand, levelCmd)) as LevelCommand;
        GM.Ins.Level.ExeCMD(cmd);
    }
    [ConsoleCommand("打印局内数据")]
    public void Print(string levelModel)
    {
        if (GM.Ins.Level == null)
        {
            Console.AddLog("没有对局信息", LogMsgType.Error);
            return;
        }
        Type modelType = typeof(LevelModel);
        FieldInfo res = modelType.GetField(levelModel);
        if (res != null)
        {
            object obj = res.IsStatic ? res.GetValue(null) : res.GetValue(GM.Ins.Level.model);
            Console.AddLog(GetObjectStr(obj), LogMsgType.Return);
            return;
        }
        PropertyInfo info = modelType.GetProperty(levelModel);
        if (info != null)
        {
            object obj = info.GetValue(GM.Ins.Level.model);
            Console.AddLog(GetObjectStr(obj), LogMsgType.Return);
            return;
        }

        Console.AddLog("没有对应数据字段", LogMsgType.Error);

    }
    private string GetObjectStr(object target, int level = 0)
    {
        string prefix = "";
        string typeName = target.GetType().Name;
        for (int i = 0; i < level; i++)
            prefix += " ";
        string res = prefix + $"> {typeName}: ";
        if (target is IEnumerable enumerable)
        {
            string listStr = "";
            int i = 0;
            bool isExceed = false;
            foreach (object e in enumerable)
            {
                i++;
                listStr += $"\n{prefix}[{i}]{GetObjectStr(e, level + 1)}";
                if (i >= 100)
                {
                    listStr += $"\n ...... ......";
                    isExceed = true;
                    break;
                }
            }
            res += $"(Count={(isExceed ? ">" : "")}{i}){listStr}";
        }
        else
        {
            res += target.ToString();
        }
        return res;
    }
}