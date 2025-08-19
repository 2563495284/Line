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
    private int tradeAmount; // 正数买入，负数卖出

    public override GameAction GetGameAction()
    {
        return new TradeSpecificStockGA(targetLineView.StockType, tradeAmount);
    }
}