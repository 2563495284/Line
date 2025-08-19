using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStockPriceGA : GameAction
{
    public CharacterView characterView;
    public Dictionary<ECharacterStrategyType, float> ChangePricePersentDictionary;
    public Dictionary<ECharacterStrategyType, float> ChangePriceDictionary;

    public EStockType stockType;
    public ChangeStockPriceGA(CharacterView characterView, EStockType stockType, Dictionary<ECharacterStrategyType, float> changePrice = null, Dictionary<ECharacterStrategyType, float> changePersentDictionary = null)
    {
        this.characterView = characterView;
        this.stockType = stockType;
        ChangePriceDictionary = changePrice ?? new();
        ChangePricePersentDictionary = changePersentDictionary ?? new();
    }
}