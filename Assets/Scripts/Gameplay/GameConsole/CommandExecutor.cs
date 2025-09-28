using System;
using System.Linq;
using UnityEngine;


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
        foreach (var cmd in commands)
        {
            string argsDesc = cmd.Arguments.Count > 0
                ? " " + string.Join(" ", cmd.Arguments.Select(a => $"<{(a.IsRequired ? "" : "optional ")}param {a.Name}>"))
                : "";
            Console.AddLog($"{cmd.Command}{argsDesc} - {cmd.Description}", LogMsgType.Return);
        }
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
}