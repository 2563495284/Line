using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用能量的效果
/// </summary>
public class ChangeManaEffect : Effect
{
    [Header("能量消耗设置")]
    [SerializeField]
    private int manaAmount = 1;

    [Header("消耗检查")]
    [SerializeField]
    private bool checkEnergyBeforeUse = true;

    public override GameAction GetGameAction()
    {
        ChangeManaGA energyGA = new ChangeManaGA(manaAmount);
        return energyGA;
    }
}
