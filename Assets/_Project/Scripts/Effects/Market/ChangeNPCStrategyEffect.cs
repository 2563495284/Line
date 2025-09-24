using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeNPCStrategyEffect : Effect2
{
    [SerializeField] private EStrategyType strategyType;
    [SerializeField] private float changeProbability;


    public override GameAction GetGameAction()
    {
        ChangeNPCStrategyGA changeCharacterStrategyGA = new(strategyType, changeProbability);
        return changeCharacterStrategyGA;
    }
}