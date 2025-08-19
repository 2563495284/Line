using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeStockGA : GameAction
{
    public int TradeAmount { get; set; }

    public EStockType StockType { get; set; }

    public TradeStockGA(int tradeAmount, EStockType stockType)
    {
        TradeAmount = tradeAmount;
        StockType = stockType;
    }
}