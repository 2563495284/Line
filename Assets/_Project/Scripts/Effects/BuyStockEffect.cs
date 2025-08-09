using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyStockEffect : Effect
{
    [SerializeField] private int stockAmount;

    public override GameAction GetGameAction()
    {
        BuyStockGA buyStockGA = new(stockAmount);
        return buyStockGA;
    }
}