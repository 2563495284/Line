using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Buff系统 - 管理所有Buff效果，包括杠杆Buff
/// </summary>
public class BuffSystem : Singleton<BuffSystem>
{
    [Header("杠杆Buff管理")]
    [SerializeField] private LeverageBuffData leverageBuff;

    [Header("Buff配置")]
    [SerializeField] private int maxLeverageStacks = 10;
    [SerializeField] private float baseLeverageMultiplier = 2f;
    [SerializeField] private int defaultLeverageDuration = 3;

    [Header("调试信息")]
    [SerializeField] private bool showDebugInfo = true;

    public LeverageBuffData LeverageBuff => leverageBuff;
    public bool HasLeverageBuff => leverageBuff != null && !leverageBuff.IsExpired();
    public float CurrentLeverageMultiplier => HasLeverageBuff ? leverageBuff.GetTotalMultiplier() : 1f;

    protected override void Awake()
    {
        base.Awake();
        InitializeBuff();
    }

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<AddLeverageBuffGA>(AddLeverageBuffPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<AddLeverageBuffGA>();
    }

    #region Initialization

    /// <summary>
    /// 初始化Buff系统
    /// </summary>
    private void InitializeBuff()
    {
        if (leverageBuff == null)
        {
            leverageBuff = new LeverageBuffData(
                0, // 初始无层数
                baseLeverageMultiplier,
                0, // 初始无持续时间
                maxLeverageStacks,
                true
            );
        }
    }

    #endregion

    #region Performers

    /// <summary>
    /// 处理添加杠杆Buff
    /// </summary>
    private IEnumerator AddLeverageBuffPerformer(AddLeverageBuffGA leverageBuffGA)
    {
        if (leverageBuff == null)
        {
            InitializeBuff();
        }

        int oldStacks = leverageBuff.stackCount;
        float oldMultiplier = leverageBuff.GetTotalMultiplier();

        // 添加层数和刷新持续时间
        leverageBuff.AddStacks(leverageBuffGA.StackCount, leverageBuffGA.Duration);

        float newMultiplier = leverageBuff.GetTotalMultiplier();

        if (showDebugInfo)
        {
            Debug.Log($"杠杆Buff更新: {oldStacks}层 -> {leverageBuff.stackCount}层 | " +
                     $"倍数: {oldMultiplier:F1}x -> {newMultiplier:F1}x | " +
                     $"持续: {leverageBuff.remainingRounds}回合");
        }

        // 播放Buff获得动画
        if (oldStacks == 0)
        {
            // BuffUIManager.Instance?.PlayBuffGainedAnimation("杠杆");
        }

        // 通知UI更新
        NotifyBuffChanged();

        yield return null;
    }

    #endregion

    #region Buff Management

    /// <summary>
    /// 更新所有Buff的回合计数（每回合调用）
    /// </summary>
    public void UpdateBuffRounds()
    {
        if (leverageBuff != null && !leverageBuff.IsExpired())
        {
            int oldRounds = leverageBuff.remainingRounds;
            leverageBuff.ReduceDuration(1);

            if (showDebugInfo)
            {
                if (leverageBuff.IsExpired())
                {
                    Debug.Log($"杠杆Buff已过期: {oldRounds} -> 0回合");
                }
                else
                {
                    Debug.Log($"杠杆Buff回合更新: {oldRounds} -> {leverageBuff.remainingRounds}回合 " +
                             $"({leverageBuff.stackCount}层, {leverageBuff.GetTotalMultiplier():F1}x倍数)");
                }
            }

            // 播放Buff过期动画
            if (leverageBuff.IsExpired())
            {
                // BuffUIManager.Instance?.PlayBuffExpiredAnimation("杠杆");
            }

            NotifyBuffChanged();
        }
    }

    /// <summary>
    /// 应用杠杆倍数到股价变化
    /// </summary>
    public float ApplyLeverageToStockChange(float originalChange)
    {
        if (!HasLeverageBuff)
            return originalChange;

        float multiplier = CurrentLeverageMultiplier;
        float leveragedChange = originalChange * multiplier;

        if (showDebugInfo && originalChange != 0)
        {
            Debug.Log($"杠杆效果应用: 原始变化 {originalChange:F2} -> 杠杆后 {leveragedChange:F2} " +
                     $"(倍数: {multiplier:F1}x, {leverageBuff.stackCount}层)");
        }

        return leveragedChange;
    }

    /// <summary>
    /// 清除所有Buff
    /// </summary>
    [ContextMenu("清除所有Buff")]
    public void ClearAllBuffs()
    {
        if (leverageBuff != null)
        {
            leverageBuff.Reset();
            NotifyBuffChanged();

            if (showDebugInfo)
            {
                Debug.Log("所有Buff已清除");
            }
        }
    }

    /// <summary>
    /// 手动添加杠杆Buff（测试用）
    /// </summary>
    [ContextMenu("添加杠杆Buff")]
    public void AddTestLeverageBuff()
    {
        AddLeverageBuffGA testGA = new AddLeverageBuffGA(2, baseLeverageMultiplier, defaultLeverageDuration);
        ActionSystem.Instance.Perform(testGA);
    }

    #endregion

    #region UI and Events

    /// <summary>
    /// 通知Buff状态改变
    /// </summary>
    private void NotifyBuffChanged()
    {
        // 触发UI更新
        // BuffUIManager.Instance?.RefreshDisplay();

        if (showDebugInfo)
        {
            Debug.Log("BuffSystem: 已通知UI更新Buff显示");
        }
    }

    /// <summary>
    /// 获取所有活跃Buff的描述
    /// </summary>
    public List<string> GetActiveBuffDescriptions()
    {
        List<string> descriptions = new List<string>();

        if (HasLeverageBuff)
        {
            descriptions.Add(leverageBuff.GetDescription());
        }

        return descriptions;
    }

    /// <summary>
    /// 获取Buff统计信息
    /// </summary>
    public (int leverageStacks, float leverageMultiplier, int leverageRounds) GetBuffStats()
    {
        if (HasLeverageBuff)
        {
            return (leverageBuff.stackCount, leverageBuff.GetTotalMultiplier(), leverageBuff.remainingRounds);
        }

        return (0, 1f, 0);
    }

    #endregion

    #region Debug Methods

    /// <summary>
    /// 打印当前Buff状态
    /// </summary>
    [ContextMenu("打印Buff状态")]
    public void PrintBuffStatus()
    {
        if (leverageBuff == null || leverageBuff.IsExpired())
        {
            Debug.Log("当前没有活跃的杠杆Buff");
            return;
        }

        Debug.Log($"=== 杠杆Buff状态 ===");
        Debug.Log($"层数: {leverageBuff.stackCount}/{leverageBuff.maxStacks}");
        Debug.Log($"倍数: {leverageBuff.GetTotalMultiplier():F1}x");
        Debug.Log($"剩余回合: {leverageBuff.remainingRounds}");
        Debug.Log($"创建时间: {leverageBuff.createdTime:HH:mm:ss}");
        Debug.Log($"最后更新: {leverageBuff.lastUpdateTime:HH:mm:ss}");
    }

    /// <summary>
    /// 模拟回合结束
    /// </summary>
    [ContextMenu("模拟回合结束")]
    public void SimulateRoundEnd()
    {
        UpdateBuffRounds();
        Debug.Log("已模拟回合结束，Buff状态已更新");
    }

    #endregion
}
