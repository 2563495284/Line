using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeNPCStrategyEffect : Effect
{
    [SerializeField] private ECharacterStrategyType strategyType;
    [SerializeField] private float changeProbability;


    public override GameAction GetGameAction()
    {
        ChangeNPCStrategyGA changeCharacterStrategyGA = new(strategyType, changeProbability);
        return changeCharacterStrategyGA;
    }
}