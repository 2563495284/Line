using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用能量的效果
/// </summary>
public class UseEnergyEffect : Effect
{
    [Header("能量消耗设置")]
    [SerializeField]
    private int energyCost = 1;

    [Header("消耗检查")]
    [SerializeField]
    private bool checkEnergyBeforeUse = true;

    public override GameAction GetGameAction()
    {
        // 检查能量是否足够
        if (checkEnergyBeforeUse && PlayerAttributeSystem.Instance != null)
        {
            if (!PlayerAttributeSystem.Instance.HasEnoughEnergy(energyCost))
            {
                Debug.LogWarning($"能量不足: 需要 {energyCost}，拥有 {PlayerAttributeSystem.Instance.GetCurrentEnergy()}");
                return null; // 返回null表示无法执行
            }
        }

        UseEnergyGA energyGA = new UseEnergyGA(energyCost);
        return energyGA;
    }

    /// <summary>
    /// 获取能量需求描述
    /// </summary>
    public string GetEnergyRequirementText()
    {
        return $"消耗: {energyCost} 能量";
    }

    /// <summary>
    /// 检查是否可以使用
    /// </summary>
    public bool CanUse()
    {
        if (PlayerAttributeSystem.Instance == null) return false;
        return PlayerAttributeSystem.Instance.HasEnoughEnergy(energyCost);
    }
}
