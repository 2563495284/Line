using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyStockGA : GameAction
{
    public int Amount { get; set; }

    public BuyStockGA(int amount)
    {
        Amount = amount;
    }
}