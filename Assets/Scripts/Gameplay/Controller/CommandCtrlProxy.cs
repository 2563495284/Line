using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class CommandCtrlProxy
{
    readonly LevelController ctrl;
    public CommandCtrlProxy(LevelController ctrl)
    {
        this.ctrl = ctrl;
    }
    private Dictionary<Type, List<CMDReaction>> preReactions = new();
    private Dictionary<Type, CMDPerformer> performers = new();
    private Dictionary<Type, List<CMDReaction>> postReactions = new();
    public event Action enterPerform = () => { };
    public event Action exitPerform = () => { };

    // 动作队列相关
    private Queue<LevelCommand> cmdQueue = new();
    public bool IsProcessingQueue { get; private set; } = false;



    public void AttachPerformer<T>(CMDPerformer<T> performer) where T : LevelCommand
    {
        Type type = typeof(T);
        IEnumerator wrappedPerformer(LevelCommand action) => performer((T)action);

        if (performers.ContainsKey(type))
            performers[type] = wrappedPerformer;
        else
            performers.Add(type, wrappedPerformer);
    }
    public void DetachPerformer<T>() where T : LevelCommand
    {
        Type type = typeof(T);

        if (performers.ContainsKey(type))
            performers.Remove(type);
    }

    public void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : LevelCommand
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

    public void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : LevelCommand
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
    public void AddCMD(LevelCommand cmd)
    {
        // 将动作加入队列

        cmdQueue.Enqueue(cmd);

        // 如果队列未在处理中，开始处理
        if (!IsProcessingQueue)
        {
            enterPerform.Invoke();
            ctrl.Root.StartCoroutine(ProcessCommands());
        }
    }
    /// <summary>
    /// 立即执行命令
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    public IEnumerator ExeCMD(LevelCommand cmd)
    {
        yield return CommandPerformFlow(cmd);
    }


    /// <summary>
    /// 按顺序执行命令
    /// </summary>
    private IEnumerator ProcessCommands()
    {
        IsProcessingQueue = true;

        while (cmdQueue.Count > 0)
        {
            LevelCommand cmd = cmdQueue.Dequeue();
            yield return CommandPerformFlow(cmd);
        }

        IsProcessingQueue = false;
        exitPerform.Invoke();
    }

    /// <summary>
    /// 对于一个LevelCommand的完整的perform流程定义
    /// </summary>
    private IEnumerator CommandPerformFlow(LevelCommand cmd)
    {
        PerformReactions(cmd, preReactions);
        List<LevelCommand> preCmds = cmd.PickGen();
        yield return PerformCommands(preCmds);

        yield return PerformPerformer(cmd);
        List<LevelCommand> subCmds = cmd.PickGen();
        yield return PerformCommands(subCmds);

        PerformReactions(cmd, postReactions);
        List<LevelCommand> postCmds = cmd.PickGen();
        yield return PerformCommands(postCmds);
    }

    /// <summary>
    /// 执行由System为Command附加的performer流程
    /// </summary>
    private IEnumerator PerformPerformer(LevelCommand cmd)
    {
        Type type = cmd.GetType();

        if (performers.ContainsKey(type))
            yield return performers[type](cmd);
    }

    /// <summary>
    /// 为所有LevelCommand执行完整的PerformFlow流程
    /// </summary>
    private IEnumerator PerformCommands(List<LevelCommand> cmds)
    {
        foreach (LevelCommand cmd in cmds)
        {
            yield return CommandPerformFlow(cmd);
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