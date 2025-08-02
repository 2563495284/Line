using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddStockEffect : Effect
{
    [SerializeField] private int stockAmount;

    public override GameAction GetGameAction()
    {
        AddStockGA addStockGA = new(stockAmount);
        return addStockGA;
    }
}