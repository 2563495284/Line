using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStockEffect : Effect
{
    [SerializeField] private int changeStockAmount;

    public override GameAction GetGameAction()
    {
        ChangeStockGA changeStockGA = new(changeStockAmount);
        return changeStockGA;
    }
}