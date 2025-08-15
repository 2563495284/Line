using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeNPCStrategyGA : GameAction
{
    public ECharacterStrategyType StrategyType { get; set; }

    public float ChangeProbability { get; set; }

    public ChangeNPCStrategyGA(ECharacterStrategyType strategyType, float changeProbability)
    {
        StrategyType = strategyType;
        ChangeProbability = changeProbability;
    }
}