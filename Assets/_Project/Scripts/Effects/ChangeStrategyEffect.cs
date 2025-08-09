using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStrategyEffect : Effect
{
    [SerializeField] private ECharacterStrategyType strategyType;

    public override GameAction GetGameAction()
    {
        ChangeStrategyGA changeCharacterStrategyGA = new(characterView, strategyType);
        return changeCharacterStrategyGA;
    }
}