using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerStockPriceGA : GameAction
{
    public float LowerPrice = 0;
    public float LowerPersent = 0;
    public LowerStockPriceGA(float lowerPrice = 0, float lowerPersent = 0)
    {
        LowerPrice = lowerPrice;
        LowerPersent = lowerPersent / 100;
    }
}