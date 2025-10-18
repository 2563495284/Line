using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
public enum ReactionTiming
{
    PRE,
    POST
}
// 自定义特性：带字符串参数的协程标记
[AttributeUsage(AttributeTargets.Method)]
public class TagEnumeratorAttribute : Attribute
{
    // 用于层级构建的字符串标识
    public string Tag { get; }

    // 构造函数，接受层名称参数
    public TagEnumeratorAttribute(string tag = "")
    {
        Tag = tag;
    }
}

public class CommandCtrlProxy
{
    readonly LevelController ctrl;
    private Dictionary<Type, List<CMDReaction>> preReactions = new();
    private Dictionary<Type, CMDProcessor> processors = new();
    private Dictionary<Type, List<CMDReaction>> postReactions = new();
    public event Action Event_EnterPerform = () => { };
    public event Action Event_ExitPerform = () => { };

    // 动作队列相关
    private Queue<LevelCommand> cmdQueue = new();
    public Coroutine QueueCoroutine { get; private set; } = null;

    public CommandCtrlProxy(LevelController ctrl)
    {
        this.ctrl = ctrl;
        AttachProcessor<CommandGroup>(CommandGroupProcessor);
    }
    [TagEnumerator("Group")]
    private IEnumerator CommandGroupProcessor(CommandGroup grp)
    {
        yield return RunInParallel(grp.Cmds.Select(e => CommandProcessFlow(e)).ToArray());
    }
    public void AttachProcessor<T>(CMDPerformer<T> performer) where T : LevelCommand
    {
        Type type = typeof(T);
        IEnumerator wrappedPerformer(LevelCommand action)
        {
            var attr = performer.Method.GetCustomAttribute<TagEnumeratorAttribute>();
            if (attr != null)
                return performer((T)action).WithData(attr.Tag, "CMD");
            else
                return performer((T)action);
        }
        if (processors.ContainsKey(type))
            processors[type] = wrappedPerformer;
        else
            processors.Add(type, wrappedPerformer);
    }
    public void DetachProcessor<T>() where T : LevelCommand
    {
        Type type = typeof(T);

        if (processors.ContainsKey(type))
            processors.Remove(type);
    }

    public void SubscribeReaction<T>(CMDReaction<T> reaction, ReactionTiming timing) where T : LevelCommand
    {
        Dictionary<Type, List<CMDReaction>> subs = timing == ReactionTiming.PRE ? preReactions : postReactions;

        void wrappedReaction(LevelCommand action) => reaction((T)action);

        if (subs.ContainsKey(typeof(T)))
            subs[typeof(T)].Add(wrappedReaction);
        else
        {
            subs.Add(typeof(T), new());
            subs[typeof(T)].Add(wrappedReaction);
        }
    }

    public void UnsubscribeReaction<T>(CMDReaction<T> reaction, ReactionTiming timing) where T : LevelCommand
    {
        Dictionary<Type, List<CMDReaction>> subs = timing == ReactionTiming.PRE ? preReactions : postReactions;

        if (subs.ContainsKey(typeof(T)))
        {
            void wrappedReaction(LevelCommand action) => reaction((T)action);
            subs[typeof(T)].Remove(wrappedReaction);
        }
    }

    /// <summary>
    /// 发送LevelCommand，LevelCommand的执行开端
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="OnPerformFinished"></param>
    public Coroutine AddCMD(LevelCommand cmd)
    {
        // 将动作加入队列

        cmdQueue.Enqueue(cmd);

        // 如果队列未在处理中，开始处理
        if (QueueCoroutine == null)
        {
            Event_EnterPerform.Invoke();
            if (GM.Ins.IsTrackCoroutine)
                return ctrl.Root.StartTrackedCoroutine(ProcessCommands(), "Queue");
            else
                return ctrl.Root.StartCoroutine(ExeCMD(cmd));
        }
        else
            return QueueCoroutine;
    }
    /// <summary>
    /// 立即执行命令
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    public IEnumerator ExeCMD(LevelCommand cmd)
    {
        yield return CommandProcessFlow(cmd);
    }


    /// <summary>
    /// 按顺序执行命令
    /// </summary>
    private IEnumerator ProcessCommands()
    {

        while (cmdQueue.Count > 0)
        {
            LevelCommand cmd = cmdQueue.Dequeue();
            yield return CommandProcessFlow(cmd);
        }

        QueueCoroutine = null;
        Event_ExitPerform.Invoke();
    }

    private IEnumerator RunInParallel(params IEnumerator[] coroutines)
    {
        if (coroutines == null || coroutines.Length == 0)
            yield break;

        // 存储所有活跃的协程迭代器
        var activeCoroutines = new List<IEnumerator>(coroutines);

        while (activeCoroutines.Count > 0)
        {
            // 遍历所有活跃协程并推进其执行
            for (int i = activeCoroutines.Count - 1; i >= 0; i--)
            {
                var coroutine = activeCoroutines[i];

                // 推进当前协程的迭代器
                bool isCompleted;
                try
                {
                    isCompleted = !coroutine.MoveNext();
                }
                catch (Exception e)
                {
                    Debug.LogError($"协程执行出错: {e.Message}");
                    isCompleted = true;
                }

                // 如果协程已完成，从活跃列表中移除
                if (isCompleted)
                {
                    activeCoroutines.RemoveAt(i);
                }
            }

            // 等待一帧，让所有协程有机会推进
            yield return null;
        }
    }
    /// <summary>
    /// 对于一个LevelCommand的完整的perform流程定义
    /// </summary>
    private IEnumerator CommandProcessFlow(LevelCommand cmd)
    {
        PerformReactions(cmd, preReactions);
        List<LevelCommand> preCmds = cmd.PickSub();
        yield return PerformCommands(preCmds);

        yield return PerformPerformer(cmd);
        List<LevelCommand> subCmds = cmd.PickSub();
        yield return PerformCommands(subCmds);

        PerformReactions(cmd, postReactions);
        List<LevelCommand> postCmds = cmd.PickSub();
        yield return PerformCommands(postCmds);
    }

    /// <summary>
    /// 执行由System为Command附加的performer流程
    /// </summary>
    private IEnumerator PerformPerformer(LevelCommand cmd)
    {
        Type type = cmd.GetType();

        if (processors.ContainsKey(type))
        {
            yield return processors[type](cmd);
        }
    }

    /// <summary>
    /// 为所有LevelCommand执行完整的PerformFlow流程
    /// </summary>
    private IEnumerator PerformCommands(List<LevelCommand> cmds)
    {
        foreach (LevelCommand cmd in cmds)
        {
            yield return CommandProcessFlow(cmd);
        }
    }

    /// <summary>
    /// 为cmd执行所有相关订阅的Reaction
    /// </summary>
    private void PerformReactions(LevelCommand cmd, Dictionary<Type, List<CMDReaction>> subs)
    {
        Type type = cmd.GetType();

        if (subs.ContainsKey(type))
        {
            foreach (CMDReaction sub in subs[type])
            {
                sub(cmd);
            }
        }
    }
}