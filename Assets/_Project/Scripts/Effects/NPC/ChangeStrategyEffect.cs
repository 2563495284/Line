using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStrategyEffect : Effect2
{
    [SerializeField] private EStrategyType strategyType;

    public override GameAction GetGameAction()
    {
        ChangeStrategyGA changeCharacterStrategyGA = new(characterView, strategyType);
        return changeCharacterStrategyGA;
    }
}