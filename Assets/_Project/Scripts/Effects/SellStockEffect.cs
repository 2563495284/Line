using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellStockEffect : Effect
{
    [SerializeField] private int stockAmount;

    public override GameAction GetGameAction()
    {
        SellStockGA sellStockGA = new(stockAmount);
        return sellStockGA;
    }
}