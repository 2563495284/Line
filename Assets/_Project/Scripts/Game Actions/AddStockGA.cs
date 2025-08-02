using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddStockGA : GameAction
{
    public int Amount { get; set; }

    public AddStockGA(int amount)
    {
        Amount = amount;
    }
}