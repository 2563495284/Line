using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeStockGA : GameAction
{
    public int Amount { get; set; }

    public TradeStockGA(int amount)
    {
        Amount = amount;
    }
}