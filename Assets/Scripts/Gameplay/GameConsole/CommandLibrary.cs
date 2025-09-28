
// 命令参数结构
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

// 自定义特性：带字符串参数的协程标记
[AttributeUsage(AttributeTargets.Method)]
public class ConsoleCommandAttribute : Attribute
{
    // 用于层级构建的字符串标识
    public string Description { get; }

    // 构造函数，接受层名称参数
    public ConsoleCommandAttribute(string desc = "")
    {
        Description = desc;
    }
}

public class CommandArgument
{
    public string Name;
    public bool IsRequired;
}

public class CommandInfo : IIndexableElement<string>
{
    public string Command;
    public string Description;
    public List<CommandArgument> Arguments = new List<CommandArgument>();
    public MethodInfo methodInfo;
    public bool HasReturn;
    public string GetKey()
    {
        return Command;
    }
}

// 命令词库管理类
public class CommandLibrary
{
    private DList<string, CommandInfo> _commands = new();
    private List<MethodInfo> commandActionMethods;
    private CommandExecutor executor;
    public CommandLibrary(CommandLineCtrl ctrl)
    {
        executor = new(this, ctrl);
        List<CommandInfo> commandInfos = new List<CommandInfo>();
        Type executorType = typeof(CommandExecutor);
        MethodInfo[] methods = executorType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (MethodInfo method in methods)
        {
            // 跳过特殊方法（如构造函数、属性访问器等）
            if (method.IsSpecialName)
                continue;

            // 创建命令信息
            CommandInfo commandInfo = new CommandInfo
            {
                Command = method.Name,
                methodInfo = method,
                HasReturn = method.ReturnType != typeof(void),
                // 获取描述（从特性或默认值）
                Description = GetMethodDescription(method)
            };

            // 处理方法参数
            foreach (ParameterInfo param in method.GetParameters())
            {
                commandInfo.Arguments.Add(new CommandArgument
                {
                    Name = param.Name,
                    // 判断是否为必需参数（非可选且无默认值）
                    IsRequired = !param.IsOptional && !param.HasDefaultValue
                });
            }
            commandInfos.Add(commandInfo);
        }
        _commands = new(commandInfos);
    }
    private string GetMethodDescription(MethodInfo method)
    {
        // 检查方法是否有ConsoleCommandAttribute特性
        ConsoleCommandAttribute attribute = method.GetCustomAttribute<ConsoleCommandAttribute>();
        return attribute != null ? attribute.Description : "无描述";
    }
    public void ExecuteCommand(string cmd, string[] args)
    {
        try
        {
            CommandInfo info = GetCommandInfo(cmd);
            object ret = info.methodInfo.Invoke(executor, args);
            if (info.HasReturn)
                UIManager.Ins.console.AddLog($"return: {ret}", LogMsgType.Return);
        }
        catch (Exception e)
        {
            GameConsole.LogErrorSafeInUnity($"【执行命令{cmd}出错】{e}");
        }
    }

    // 获取所有命令
    public List<CommandInfo> GetAllCommands()
    {
        return _commands.ToList();
    }

    // 查找匹配的命令（用于补全）
    public List<string> FindMatchingCommands(string prefix)
    {
        return _commands.Keys.Where(cmd => cmd.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                            .OrderBy(cmd => cmd)
                            .ToList();
    }

    // 获取命令信息
    public CommandInfo GetCommandInfo(string command)
    {
        return _commands[command];
    }

    // 检查命令是否存在
    public bool CommandExists(string command)
    {
        return _commands.Has(command);
    }
}
