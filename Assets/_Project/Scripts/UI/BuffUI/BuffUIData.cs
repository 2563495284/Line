using System;
using UnityEngine;

[System.Serializable]
public class BuffUIData
{
    [Header("基础信息")]
    public string buffName;
    public string description;
    public Sprite icon;
    public Color backgroundColor = Color.white;
    public Color textColor = Color.black;

    [Header("数值信息")]
    public int stackCount;
    public float multiplier;
    public int remainingRounds;
    public float progress; // 0-1, 用于进度条

    [Header("状态")]
    public bool isActive;
    public bool isExpiring; // 即将过期
    public bool isNew; // 新获得的Buff

    [Header("动画")]
    public bool shouldPulse; // 是否应该脉冲动画
    public bool shouldShake; // 是否应该震动动画

    public BuffUIData()
    {
        buffName = "未知Buff";
        description = "";
        backgroundColor = Color.white;
        textColor = Color.black;
        isActive = false;
    }

    public BuffUIData(string name, string desc, int stacks, float mult, int rounds)
    {
        buffName = name;
        description = desc;
        stackCount = stacks;
        multiplier = mult;
        remainingRounds = rounds;
        isActive = true;
        progress = rounds > 0 ? 1f : 0f;
    }

    /// <summary>
    /// 从杠杆Buff数据创建UI数据
    /// </summary>
    public static BuffUIData FromLeverageBuff(LeverageBuffData leverageBuff)
    {
        if (leverageBuff == null || leverageBuff.IsExpired())
        {
            return new BuffUIData
            {
                buffName = "杠杆",
                description = "无活跃杠杆",
                isActive = false
            };
        }

        return new BuffUIData
        {
            buffName = "杠杆",
            description = leverageBuff.GetDescription(),
            stackCount = leverageBuff.stackCount,
            multiplier = leverageBuff.GetTotalMultiplier(),
            remainingRounds = leverageBuff.remainingRounds,
            progress = leverageBuff.remainingRounds / 10f, // 假设最大10回合
            isActive = true,
            isExpiring = leverageBuff.remainingRounds <= 1,
            backgroundColor = leverageBuff.remainingRounds <= 1 ? Color.red : Color.green,
            textColor = Color.white,
            shouldPulse = leverageBuff.remainingRounds <= 1
        };
    }

    /// <summary>
    /// 更新进度
    /// </summary>
    public void UpdateProgress(int maxRounds)
    {
        if (maxRounds > 0)
        {
            progress = (float)remainingRounds / maxRounds;
        }
        else
        {
            progress = 0f;
        }
    }

    /// <summary>
    /// 检查是否需要警告动画
    /// </summary>
    public bool ShouldShowWarning()
    {
        return isActive && (remainingRounds <= 1 || progress <= 0.2f);
    }

    /// <summary>
    /// 获取显示文本
    /// </summary>
    public string GetDisplayText()
    {
        if (!isActive)
            return buffName;

        if (stackCount > 1)
        {
            return $"{buffName} x{multiplier:F1} ({stackCount}层)";
        }
        else
        {
            return $"{buffName} x{multiplier:F1}";
        }
    }

    /// <summary>
    /// 获取回合显示文本
    /// </summary>
    public string GetRoundsText()
    {
        if (!isActive || remainingRounds <= 0)
            return "";

        return $"{remainingRounds}回合";
    }
}
