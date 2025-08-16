using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用能量的GameAction
/// </summary>
public class UseEnergyGA : GameAction
{
    public int Amount { get; set; }

    public UseEnergyGA(int amount)
    {
        Amount = amount;
    }
}
