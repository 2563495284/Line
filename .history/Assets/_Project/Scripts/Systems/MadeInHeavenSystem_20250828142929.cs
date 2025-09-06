using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自动系统管理器 - 管理天堂制造等自动执行功能
/// </summary>
public class MadeInHeavenSystem : Singleton<MadeInHeavenSystem>
{
    [Header("天堂制造设置")]
    [SerializeField] private float defaultAutoInterval = 3.0f; // 默认自动执行间隔
    [SerializeField] private int maxExecuteCount = 10; // 最大执行次数，达到后自动停止
    [SerializeField] private bool showDebugLogs = true; // 是否显示调试日志

    public List<CardData> startMadeInHeavenCards;
    public List<CardData> endMadeInHeavenCards;

    private bool isMadeInHeavenActive = false; // 天堂制造是否激活
    private float currentAutoInterval; // 当前自动执行间隔
    private Coroutine autoExecuteCoroutine; // 自动执行协程
    private int currentExecuteCount = 0; // 当前执行次数计数器


    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<StartMadeInHeavenGA>(OnStartMadeInHeavenReaction, ReactionTiming.POST);
        ActionSystem.SubscribeReaction<EndMadeInHeavenGA>(OnEndMadeInHeavenReaction, ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<StartMadeInHeavenGA>(OnStartMadeInHeavenReaction, ReactionTiming.POST);
        ActionSystem.UnsubscribeReaction<EndMadeInHeavenGA>(OnEndMadeInHeavenReaction, ReactionTiming.POST);
    }

    protected void OnDestroy()
    {
    }

    /// <summary>
    /// 响应启动天堂制造动作
    /// </summary>
    private void OnStartMadeInHeavenReaction(StartMadeInHeavenGA action)
    {
        // 这里只处理来自卡牌效果的启动请求
        // 如果已经激活，则忽略
        if (isMadeInHeavenActive) return;

        StartMadeInHeavenInternal(action.AutoInterval);

        foreach (var cardData in startMadeInHeavenCards)
        {
            Card card = new Card(cardData);

            PlayCardGA playCardGA = new(card, PlayerAttributeSystem.Instance.playerView);
            PlayerAttributeSystem.Instance.playerView.DoManualTargetEffect(playCardGA);
            PlayerAttributeSystem.Instance.playerView.DoAutoTargetEffect(playCardGA);
        }
    }

    private void OnEndMadeInHeavenReaction(EndMadeInHeavenGA action)
    {
        foreach (var cardData in endMadeInHeavenCards)
        {
            Card card = new Card(cardData);

            PlayCardGA playCardGA = new(card, PlayerAttributeSystem.Instance.playerView);
            PlayerAttributeSystem.Instance.playerView.DoManualTargetEffect(playCardGA);
            PlayerAttributeSystem.Instance.playerView.DoAutoTargetEffect(playCardGA);
        }
    }


    /// <summary>
    /// 启动天堂制造自动系统（外部调用接口）
    /// </summary>
    /// <param name="interval">自动执行间隔时间</param>
    public void StartMadeInHeaven(float interval = -1)
    {
        if (interval < 0) interval = defaultAutoInterval;

        // 如果已经在运行，先停止
        if (isMadeInHeavenActive)
        {
            StopMadeInHeaven();
        }

        currentAutoInterval = Mathf.Max(0.1f, interval); // 最小间隔0.1秒

        // 发送启动天堂制造GA - 这会让系统切换订阅
        StartMadeInHeavenGA startGA = new StartMadeInHeavenGA(currentAutoInterval);
        ActionSystem.Instance.Perform(startGA);

        StartMadeInHeavenInternal(interval);
    }

    /// <summary>
    /// 启动天堂制造自动系统（内部实现）
    /// </summary>
    /// <param name="interval">自动执行间隔时间</param>
    private void StartMadeInHeavenInternal(float interval = -1)
    {
        if (interval < 0) interval = defaultAutoInterval;

        // 如果已经在运行，先停止
        if (isMadeInHeavenActive)
        {
            StopMadeInHeavenInternal();
        }

        currentAutoInterval = Mathf.Max(0.1f, interval); // 最小间隔0.1秒
        currentExecuteCount = 0; // 重置执行计数器
        isMadeInHeavenActive = true;

        // 启动自动执行协程
        autoExecuteCoroutine = StartCoroutine(AutoExecuteCoroutine());

        if (showDebugLogs)
        {
            Debug.Log($"[天堂制造] 已启动！自动执行间隔: {currentAutoInterval}秒，最大执行次数: {maxExecuteCount}");
        }
    }

    /// <summary>
    /// 停止天堂制造自动系统（外部调用接口）
    /// </summary>
    public void StopMadeInHeaven()
    {
        if (!isMadeInHeavenActive) return;
        if (ActionSystem.Instance == null) return;

        // 发送结束天堂制造GA - 这会让系统恢复订阅
        EndMadeInHeavenGA endGA = new EndMadeInHeavenGA();
        ActionSystem.Instance.Perform(endGA);

        StopMadeInHeavenInternal();
    }

    /// <summary>
    /// 停止天堂制造自动系统（内部实现）
    /// </summary>
    private void StopMadeInHeavenInternal()
    {
        if (!isMadeInHeavenActive) return;

        isMadeInHeavenActive = false;

        // 停止协程
        if (autoExecuteCoroutine != null)
        {
            StopCoroutine(autoExecuteCoroutine);
            autoExecuteCoroutine = null;
        }

        if (showDebugLogs)
        {
            Debug.Log($"[天堂制造] 已停止！(共执行了 {currentExecuteCount} 次)");
        }

        // 重置执行计数器
        currentExecuteCount = 0;

        // 触发状态改变事件
        OnMadeInHeavenStateChanged?.Invoke(false);
    }

    /// <summary>
    /// 切换天堂制造状态
    /// </summary>
    public void ToggleMadeInHeaven()
    {
        if (isMadeInHeavenActive)
        {
            StopMadeInHeaven();
        }
        else
        {
            StartMadeInHeaven();
        }
    }

    /// <summary>
    /// 自动执行协程 - 每隔指定时间自动触发NextRoundTurnGA
    /// </summary>
    private IEnumerator AutoExecuteCoroutine()
    {
        while (isMadeInHeavenActive)
        {
            // 等待指定时间
            yield return new WaitForSeconds(currentAutoInterval);

            // 检查是否还在运行状态
            if (!isMadeInHeavenActive) break;

            // 自动执行天堂制造逻辑
            ExecuteAutoMadeInHeaven();
        }
    }



    /// <summary>
    /// 执行自动天堂制造逻辑
    /// </summary>
    private void ExecuteAutoMadeInHeaven()
    {
        try
        {
            // 增加执行计数
            currentExecuteCount++;

            // 创建并执行天堂制造GA
            MadeInHeavenExecuteGA executeGA = new MadeInHeavenExecuteGA();
            ActionSystem.Instance.Perform(executeGA);

            if (showDebugLogs)
            {
                Debug.Log($"[天堂制造] 自动执行天堂制造逻辑 ({currentExecuteCount}/{maxExecuteCount})");
                Debug.Log("[天堂制造] 同时触发NextRoundTurnGA以确保玩家系统正常工作");
            }

            // 检查是否达到最大执行次数
            if (currentExecuteCount >= maxExecuteCount)
            {
                if (showDebugLogs)
                {
                    Debug.Log($"[天堂制造] 已达到最大执行次数 ({maxExecuteCount})，自动停止");
                }
                StopMadeInHeaven();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[天堂制造] 自动执行出错: {e.Message}");
            // 发生错误时停止自动执行
            StopMadeInHeaven();
        }
    }

    /// <summary>
    /// 设置自动执行间隔
    /// </summary>
    /// <param name="interval">间隔时间（秒）</param>
    public void SetAutoInterval(float interval)
    {
        float newInterval = Mathf.Max(0.1f, interval);

        if (Mathf.Approximately(currentAutoInterval, newInterval)) return;

        currentAutoInterval = newInterval;

        // 如果正在运行，重新启动以应用新间隔
        if (isMadeInHeavenActive)
        {
            bool wasActive = isMadeInHeavenActive;
            StopMadeInHeaven();
            if (wasActive)
            {
                StartMadeInHeaven(newInterval);
            }
        }

        if (showDebugLogs)
        {
            Debug.Log($"[天堂制造] 间隔时间已设置为: {currentAutoInterval}秒");
        }
    }

    #region 公共属性和状态查询

    /// <summary>
    /// 获取天堂制造是否激活
    /// </summary>
    public bool IsMadeInHeavenActive => isMadeInHeavenActive;

    /// <summary>
    /// 获取当前自动执行间隔
    /// </summary>
    public float CurrentAutoInterval => currentAutoInterval;

    /// <summary>
    /// 获取当前执行次数
    /// </summary>
    public int CurrentExecuteCount => currentExecuteCount;

    /// <summary>
    /// 获取最大执行次数
    /// </summary>
    public int MaxExecuteCount => maxExecuteCount;

    /// <summary>
    /// 获取剩余执行次数
    /// </summary>
    public int RemainingExecuteCount => Mathf.Max(0, maxExecuteCount - currentExecuteCount);

    /// <summary>
    /// 设置最大执行次数
    /// </summary>
    /// <param name="maxCount">最大执行次数（必须大于0）</param>
    public void SetMaxExecuteCount(int maxCount)
    {
        maxExecuteCount = Mathf.Max(1, maxCount);

        if (showDebugLogs)
        {
            Debug.Log($"[天堂制造] 最大执行次数已设置为: {maxExecuteCount}");
        }

        // 如果当前正在运行且已经达到新的最大值，则停止
        if (isMadeInHeavenActive && currentExecuteCount >= maxExecuteCount)
        {
            StopMadeInHeaven();
        }
    }

    /// <summary>
    /// 获取系统状态信息
    /// </summary>
    public string GetStatusInfo()
    {
        if (isMadeInHeavenActive)
        {
            return $"天堂制造: 激活中 (间隔: {currentAutoInterval}s, 进度: {currentExecuteCount}/{maxExecuteCount})";
        }
        else
        {
            return $"天堂制造: 未激活 (最大执行次数: {maxExecuteCount})";
        }
    }

    #endregion
}
