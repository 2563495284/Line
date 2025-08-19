using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeAllStockGA : GameAction
{
    public ETradeAllStockType TradeAllStockType { get; set; }

    public EStockType StockType { get; set; }

    public TradeAllStockGA(ETradeAllStockType tradeAllStockType, EStockType stockType)
    {
        TradeAllStockType = tradeAllStockType;
        StockType = stockType;
    }
}

public enum ETradeAllStockType
{
    Buy,  // 梭哈买入
    Sell, // 全部卖出
}