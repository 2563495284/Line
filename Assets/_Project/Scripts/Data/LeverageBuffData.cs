using System;
using UnityEngine;

[System.Serializable]
public class LeverageBuffData
{
    [Header("Buff信息")]
    public int stackCount;
    public float multiplier;
    public int remainingRounds;
    public DateTime createdTime;
    public DateTime lastUpdateTime;

    [Header("配置")]
    public int maxStacks;
    public bool canRefresh;

    public LeverageBuffData(
        int initialStacks = 1,
        float leverageMultiplier = 2f,
        int duration = 3,
        int maxStackCount = 10,
        bool allowRefresh = true
    )
    {
        stackCount = initialStacks;
        multiplier = leverageMultiplier;
        remainingRounds = duration;
        maxStacks = maxStackCount;
        canRefresh = allowRefresh;
        createdTime = DateTime.Now;
        lastUpdateTime = DateTime.Now;
    }

    /// <summary>
    /// 添加层数
    /// </summary>
    public void AddStacks(int amount, int duration)
    {
        stackCount = Mathf.Min(stackCount + amount, maxStacks);

        if (canRefresh)
        {
            remainingRounds = Mathf.Max(remainingRounds, duration);
        }

        lastUpdateTime = DateTime.Now;
    }

    /// <summary>
    /// 减少层数
    /// </summary>
    public void ReduceStacks(int amount = 1)
    {
        stackCount = Mathf.Max(0, stackCount - amount);
        lastUpdateTime = DateTime.Now;
    }

    /// <summary>
    /// 减少持续时间
    /// </summary>
    public void ReduceDuration(int rounds = 1)
    {
        remainingRounds = Mathf.Max(0, remainingRounds - rounds);
        lastUpdateTime = DateTime.Now;
    }

    /// <summary>
    /// 获取当前总倍数
    /// </summary>
    public float GetTotalMultiplier()
    {
        if (stackCount <= 0 || remainingRounds <= 0)
            return 1f;

        return 1f + (multiplier - 1f) * stackCount;
    }

    /// <summary>
    /// 检查Buff是否已过期
    /// </summary>
    public bool IsExpired()
    {
        return stackCount <= 0 || remainingRounds <= 0;
    }

    /// <summary>
    /// 获取Buff描述
    /// </summary>
    public string GetDescription()
    {
        if (IsExpired())
            return "杠杆已失效";

        float totalMultiplier = GetTotalMultiplier();
        return $"杠杆 x{totalMultiplier:F1} ({stackCount}层, {remainingRounds}回合)";
    }

    /// <summary>
    /// 重置Buff
    /// </summary>
    public void Reset()
    {
        stackCount = 0;
        remainingRounds = 0;
        lastUpdateTime = DateTime.Now;
    }
}
