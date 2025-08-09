using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStrategyGA : GameAction
{
    public CharacterView CharacterView { get; set; }
    public ECharacterStrategyType StrategyType { get; set; }

    public ChangeStrategyGA(CharacterView characterView, ECharacterStrategyType strategyType)
    {
        CharacterView = characterView;
        StrategyType = strategyType;
    }
}