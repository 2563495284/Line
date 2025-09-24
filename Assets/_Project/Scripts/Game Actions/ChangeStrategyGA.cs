using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStrategyGA : GameAction
{
    public CharacterView CharacterView { get; set; }
    public EStrategyType StrategyType { get; set; }

    public ChangeStrategyGA(CharacterView characterView, EStrategyType strategyType)
    {
        CharacterView = characterView;
        StrategyType = strategyType;
    }
}