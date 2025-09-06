using System;
using System.Collections;
using System.Collections.Generic;

public class ActionSystem : Singleton<ActionSystem>
{

    private static Dictionary<Type, List<Action<GameAction>>> preSubs = new();
    private static Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();
    private static Dictionary<Type, List<Action<GameAction>>> postSubs = new();

    private List<GameAction> reactions = null;

    // 动作队列相关
    private Queue<ActionRequest> actionQueue = new();
    private bool isProcessingQueue = false;

    private struct ActionRequest
    {
        public GameAction action;
        public Action onFinished;
    }

    public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
    {
        Type type = typeof(T);
        IEnumerator wrappedPerformer(GameAction action) => performer((T)action);

        if
            (performers.ContainsKey(type)) performers[type] = wrappedPerformer;
        else
            performers.Add(type, wrappedPerformer);
    }

    public static void DetachPerformer<T>() where T : GameAction
    {
        Type type = typeof(T);

        if (performers.ContainsKey(type))
            performers.Remove(type);
    }

    public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;

        void wrappedReaction(GameAction action) => reaction((T)action);

        if (subs.ContainsKey(typeof(T)))
            subs[typeof(T)].Add(wrappedReaction);
        else
        {
            subs.Add(typeof(T), new());
            subs[typeof(T)].Add(wrappedReaction);
        }
    }

    public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;

        if (subs.ContainsKey(typeof(T)))
        {
            void wrappedReaction(GameAction action) => reaction((T)action);
            subs[typeof(T)].Remove(wrappedReaction);
        }
    }

    public void Perform(GameAction action, Action OnPerformFinished = null)
    {
        // 将动作加入队列
        ActionRequest request = new ActionRequest
        {
            action = action,
            onFinished = OnPerformFinished
        };

        actionQueue.Enqueue(request);

        // 如果队列未在处理中，开始处理
        if (!isProcessingQueue)
        {
            StartCoroutine(ProcessActionQueue());
        }
    }

    public void AddReaction(GameAction gameAction)
    {
        reactions?.Add(gameAction);
    }

    /// <summary>
    /// 处理动作队列，确保动作按顺序执行
    /// </summary>
    private IEnumerator ProcessActionQueue()
    {
        isProcessingQueue = true;

        while (actionQueue.Count > 0)
        {
            ActionRequest request = actionQueue.Dequeue();
            yield return Flow(request.action, request.onFinished);
        }

        isProcessingQueue = false;
    }

    private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
    {
        reactions = action.PreReactions;
        PerformSubscribers(action, preSubs);
        yield return PerformReactions();

        reactions = action.PerformReactions;
        yield return PerformPerformer(action);
        yield return PerformReactions();

        reactions = action.PostReactions;
        PerformSubscribers(action, postSubs);
        yield return PerformReactions();

        OnFlowFinished?.Invoke();
    }

    private IEnumerator PerformPerformer(GameAction action)
    {
        Type type = action.GetType();

        if (performers.ContainsKey(type))
            yield return performers[type](action);
    }

    private IEnumerator PerformReactions()
    {
        foreach (GameAction reaction in reactions)
        {
            yield return Flow(reaction);
        }
    }

    private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs)
    {
        Type type = action.GetType();

        if (subs.ContainsKey(type))
        {
            foreach (Action<GameAction> sub in subs[type])
            {
                sub(action);
            }
        }
    }
}