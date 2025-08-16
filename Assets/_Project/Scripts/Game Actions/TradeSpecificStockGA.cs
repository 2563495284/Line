using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 交易特定股票的GameAction
/// </summary>
public class TradeSpecificStockGA : GameAction
{
    public EStockType StockType { get; set; }
    public int Amount { get; set; } // 正数买入，负数卖出

    public TradeSpecificStockGA(EStockType stockType, int amount)
    {
        StockType = stockType;
        Amount = amount;
    }
}
