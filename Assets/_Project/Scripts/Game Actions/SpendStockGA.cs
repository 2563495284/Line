using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpendStockGA : GameAction
{
    public int Amount { get; set; }

    public SpendStockGA(int amount)
    {
        Amount = amount;
    }
}