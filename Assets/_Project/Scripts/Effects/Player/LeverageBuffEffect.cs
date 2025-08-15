using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverageBuffEffect : Effect
{
    [Header("杠杆Buff设置")]
    [SerializeField]
    private int stackCount = 1;

    [SerializeField]
    private float multiplier = 2f;

    [SerializeField]
    private int duration = 3;

    [SerializeField]
    private bool refreshDuration = true;

    public override GameAction GetGameAction()
    {
        AddLeverageBuffGA leverageBuffGA = new AddLeverageBuffGA(
            stackCount,
            multiplier,
            duration,
            refreshDuration
        );
        return leverageBuffGA;
    }
}
