using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TradeAllStockEffect : Effect2
{
    [SerializeField] private ETradeAllStockType tradeAllStockType;
    public override GameAction GetGameAction()
    {
        TradeAllStockGA tradeAllStockGA = new(tradeAllStockType, (EStockType)targetLineView.StockType);
        return tradeAllStockGA;
    }
}