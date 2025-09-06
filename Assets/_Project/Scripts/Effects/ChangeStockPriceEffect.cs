using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStockPriceEffect : Effect
{
    [SerializeField] private SerializableDictionary<ECharacterStrategyType, float> changePriceDictionary;
    [SerializeField] private SerializableDictionary<ECharacterStrategyType, float> changePersentDictionary;
    [SerializeField] private bool markPoint = false;
    public override GameAction GetGameAction()
    {
        ChangeStockPriceGA changeStockPriceGA = new(characterView, targetLineView.StockType, changePriceDictionary.ToDictionary(), changePersentDictionary.ToDictionary(), markPoint);
        return changeStockPriceGA;
    }
}