using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeStockEffect : Effect
{
    [SerializeField] private int tradeStockAmount;

    public override GameAction GetGameAction()
    {
        TradeStockGA tradeStockGA = new(tradeStockAmount);
        return tradeStockGA;
    }
}