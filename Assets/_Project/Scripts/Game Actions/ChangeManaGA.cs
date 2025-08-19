using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用能量的GameAction
/// </summary>
public class ChangeManaGA : GameAction
{
    public int Amount { get; set; }

    public ChangeManaGA(int amount)
    {
        Amount = amount;
    }
}
