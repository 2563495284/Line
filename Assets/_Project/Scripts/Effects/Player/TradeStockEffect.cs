using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 交易特定股票的效果
/// </summary>
public class TradeStockEffect : Effect
{
    [Header("交易设置")]
    [SerializeField]
    private EStockType stockType;

    [SerializeField]
    private int tradeAmount; // 正数买入，负数卖出

    [Header("交易检查")]
    [SerializeField]
    private bool checkResourcesBeforeUse = true;

    public override GameAction GetGameAction()
    {
        // 检查资源是否足够
        if (checkResourcesBeforeUse && MultiStockSystem.Instance != null)
        {
            if (tradeAmount > 0) // 买入检查
            {
                if (!MultiStockSystem.Instance.CanAffordStock(stockType, tradeAmount))
                {
                    Debug.LogWarning($"金币不足，无法买入 {stockType} x{tradeAmount}");
                    return null;
                }
            }
            else if (tradeAmount < 0) // 卖出检查
            {
                int sellAmount = -tradeAmount;
                if (!MultiStockSystem.Instance.HasEnoughMaterial(stockType, sellAmount))
                {
                    Debug.LogWarning($"股票不足，无法卖出 {stockType} x{sellAmount}");
                    return null;
                }
            }
        }

        TradeSpecificStockGA tradeGA = new TradeSpecificStockGA(stockType, tradeAmount);
        return tradeGA;
    }

    /// <summary>
    /// 获取交易描述
    /// </summary>
    public string GetTradeDescription()
    {
        string materialName = GetStockName(stockType);
        string operation = tradeAmount > 0 ? "买入" : "卖出";
        int amount = Mathf.Abs(tradeAmount);
        return $"{operation}: {materialName} x{amount}";
    }

    /// <summary>
    /// 检查是否可以交易
    /// </summary>
    public bool CanTrade()
    {
        if (MultiStockSystem.Instance == null) return false;

        if (tradeAmount > 0)
        {
            return MultiStockSystem.Instance.CanAffordStock(stockType, tradeAmount);
        }
        else if (tradeAmount < 0)
        {
            return MultiStockSystem.Instance.HasEnoughMaterial(stockType, -tradeAmount);
        }

        return false;
    }

    /// <summary>
    /// 获取股票名称
    /// </summary>
    private string GetStockName(EStockType stockType)
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