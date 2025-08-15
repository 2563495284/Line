using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMoneyGA : GameAction
{
    public float Amount { get; set; }

    public ChangeMoneyGA(float amount)
    {
        Amount = amount;
    }
}