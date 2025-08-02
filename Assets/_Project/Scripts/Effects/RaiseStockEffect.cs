using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseStockEffect : Effect
{
    [SerializeField] private float raisePrice;
    [SerializeField] private float raisePersent;

    public override GameAction GetGameAction()
    {
        RaiseStockPriceGA raiseStockPriceGA = new(raisePrice, raisePersent);
        return raiseStockPriceGA;
    }
}