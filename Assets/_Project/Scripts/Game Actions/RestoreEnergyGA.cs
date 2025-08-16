using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 恢复能量的GameAction
/// </summary>
public class RestoreEnergyGA : GameAction
{
    public int Amount { get; set; }

    public RestoreEnergyGA(int amount)
    {
        Amount = amount;
    }
}
