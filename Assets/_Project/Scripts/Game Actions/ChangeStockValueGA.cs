using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStockValueGA : GameAction
{
    public float Value { get; private set; }

    public ChangeStockValueGA(float value)
    {
        Value = value;
    }
}