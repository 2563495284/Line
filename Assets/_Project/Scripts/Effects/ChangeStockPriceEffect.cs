using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStockPriceEffect : Effect2
{
    [SerializeField] private SerializableDictionary<EStrategyType, float> changePriceDictionary;
    [SerializeField] private SerializableDictionary<EStrategyType, float> changePersentDictionary;
    [SerializeField] private bool markPoint = false;
    public override GameAction GetGameAction()
    {
        ChangeStockPriceGA changeStockPriceGA = new(characterView, targetLineView.StockType, changePriceDictionary.ToDictionary(), changePersentDictionary.ToDictionary(), markPoint);
        return changeStockPriceGA;
    }
}