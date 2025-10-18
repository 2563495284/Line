
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
                Debug.Log(ret);

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
        return FindMatching(prefix, _commands.Keys);
    }
    private List<string> FindMatching(string prefix, IEnumerable<string> list)
    {
        if (list == null || prefix == null)
            return new List<string>();

        // 命令为空时返回空列表（无匹配基准）
        if (string.IsNullOrEmpty(prefix))
            return new List<string>();

        var matchResults = new List<Tuple<string, float>>();

        foreach (var target in list)
        {
            // 跳过空目标字符串
            if (string.IsNullOrEmpty(target))
                continue;

            // 检查是否符合正向匹配规则
            if (IsValidForwardMatch(target, prefix))
            {
                // 计算匹配率（命令长度 / 目标字符串长度）
                float matchRate = (float)prefix.Length / target.Length;
                matchResults.Add(Tuple.Create(target, matchRate));
            }
        }

        // 按匹配率从低到高排序，相同匹配率按原字符串长度排序
        var sortedResults = matchResults
            .OrderBy(t => t.Item2)
            .ThenBy(t => t.Item1.Length)
            .Select(t => t.Item1)
            .ToList();

        return sortedResults;
    }
    private static bool IsValidForwardMatch(string target, string cmd)
    {
        int cmdIndex = 0; // 当前匹配的命令字符索引
        int targetIndex = 0; // 当前搜索的目标字符索引

        while (cmdIndex < cmd.Length && targetIndex < target.Length)
        {
            // 找到匹配的字符
            if (char.ToLower(target[targetIndex]) == char.ToLower(cmd[cmdIndex]))
            {
                cmdIndex++; // 移动到下一个命令字符
            }
            targetIndex++; // 继续搜索下一个目标字符
        }

        // 命令所有字符都按顺序匹配完成才算有效正向匹配
        return cmdIndex == cmd.Length;
    }
    public (List<string> res, bool onlyTips) FindMatchingArgs(string argName, string prefix, List<string> premise = null)
    {
        List<string> list = new();
        BindingFlags allFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        bool onlyTips = false;
        switch (argName)
        {
            case ConsoleArgsEnum.LevelCmd:
                list = LoadManager.Ins.GetAllSerialClass(EDynamicSerial.LevelCommand).Select(e => e.Name).ToList();
                break;
            case ConsoleArgsEnum.NotifyKey:
                list = typeof(EventConst).GetFields(BindingFlags.Public | BindingFlags.Static).Where(field => field.FieldType == typeof(string) && field.IsInitOnly).Select(e => (string)e.GetValue(null)).ToList();
                break;
            case ConsoleArgsEnum.LevelModel:
                Type levelModelType = typeof(LevelModel);
                list = levelModelType.GetFields(allFlag).Select(e => e.Name).Concat(
                    levelModelType.GetProperties(allFlag).Select(e => e.Name)
                ).ToList();
                break;
        }
        return (FindMatching(prefix, list), onlyTips);
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
