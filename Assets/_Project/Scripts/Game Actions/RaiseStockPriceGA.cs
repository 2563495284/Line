using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseStockPriceGA : GameAction
{
    public float RaisePrice = 0;
    public float RaisePersent = 0;
    public RaiseStockPriceGA(float raisePrice = 0, float raisePersent = 0)
    {
        RaisePrice = raisePrice;
        RaisePersent = raisePersent / 100;
    }
}