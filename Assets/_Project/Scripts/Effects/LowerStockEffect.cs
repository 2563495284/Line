using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerStockEffect : Effect
{
    [SerializeField] private float lowerPrice;
    [SerializeField] private float lowerPersent;

    public override GameAction GetGameAction()
    {
        LowerStockPriceGA lowerStockPriceGA = new(lowerPrice, lowerPersent);
        return lowerStockPriceGA;
    }
}