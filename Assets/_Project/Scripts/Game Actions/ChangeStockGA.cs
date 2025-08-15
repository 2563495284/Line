using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStockGA : GameAction
{
    public int Amount { get; set; }

    public ChangeStockGA(int amount)
    {
        Amount = amount;
    }
}