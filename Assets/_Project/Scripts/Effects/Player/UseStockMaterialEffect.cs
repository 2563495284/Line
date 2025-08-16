using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用股票材料的效果
/// </summary>
public class UseStockMaterialEffect : Effect
{
    [Header("材料消耗设置")]
    [SerializeField]
    private EStockType materialType;

    [SerializeField]
    private int materialAmount = 1;

    [Header("消耗检查")]
    [SerializeField]
    private bool checkMaterialBeforeUse = true;

    public override GameAction GetGameAction()
    {
        // 检查材料是否足够
        if (checkMaterialBeforeUse && MultiStockSystem.Instance != null)
        {
            if (!MultiStockSystem.Instance.HasEnoughMaterial(materialType, materialAmount))
            {
                Debug.LogWarning($"材料不足: 需要 {materialType} x{materialAmount}");
                return null; // 返回null表示无法执行
            }
        }

        UseStockMaterialGA materialGA = new UseStockMaterialGA(materialType, materialAmount);
        return materialGA;
    }

    /// <summary>
    /// 获取材料需求描述
    /// </summary>
    public string GetMaterialRequirementText()
    {
        string materialName = GetMaterialName(materialType);
        return $"消耗: {materialName} x{materialAmount}";
    }

    /// <summary>
    /// 检查是否可以使用
    /// </summary>
    public bool CanUse()
    {
        if (MultiStockSystem.Instance == null) return false;
        return MultiStockSystem.Instance.HasEnoughMaterial(materialType, materialAmount);
    }

    /// <summary>
    /// 获取材料名称
    /// </summary>
    private string GetMaterialName(EStockType stockType)
    {
        switch (stockType)
        {
            case EStockType.Oil: return "石油";
            case EStockType.Steel: return "钢铁";
            case EStockType.Cotton: return "棉花";
            default: return stockType.ToString();
        }
    }
}
