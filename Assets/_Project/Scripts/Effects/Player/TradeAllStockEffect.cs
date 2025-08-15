using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TradeAllStockEffect : Effect
{
    [SerializeField] private ETradeAllStockType tradeAllStockType;
    public override GameAction GetGameAction()
    {
        TradeAllStockGA tradeAllStockGA = new(tradeAllStockType);
        return tradeAllStockGA;
    }
}