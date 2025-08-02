using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMoneyGA : GameAction
{
    public float Amount { get; set; }

    public AddMoneyGA(float amount)
    {
        Amount = amount;
    }
}