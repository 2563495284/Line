using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeNPCStrategyGA : GameAction
{
    public EStrategyType StrategyType { get; set; }

    public float ChangeProbability { get; set; }

    public ChangeNPCStrategyGA(EStrategyType strategyType, float changeProbability)
    {
        StrategyType = strategyType;
        ChangeProbability = changeProbability;
    }
}