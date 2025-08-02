using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpendMoneyGA : GameAction
{
    public float Amount { get; set; }

    public SpendMoneyGA(float amount)
    {
        Amount = amount;
    }
}