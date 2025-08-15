using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStockPriceEffect : Effect
{
    [SerializeField] private SerializableDictionary<ECharacterStrategyType, float> changePriceDictionary;
    [SerializeField] private SerializableDictionary<ECharacterStrategyType, float> changePersentDictionary;

    public override GameAction GetGameAction()
    {
        ChangeStockPriceGA changeStockPriceGA = new(characterView, changePriceDictionary.ToDictionary(), changePersentDictionary.ToDictionary());
        return changeStockPriceGA;
    }
}