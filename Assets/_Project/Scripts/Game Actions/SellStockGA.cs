using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellStockGA : GameAction
{
    public int Amount { get; set; }

    public SellStockGA(int amount)
    {
        Amount = amount;
    }
}