using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStockPriceGA : GameAction
{
    public CharacterView characterView;
    public Dictionary<EStrategyType, float> ChangePricePersentDictionary;
    public Dictionary<EStrategyType, float> ChangePriceDictionary;
    public bool MarkPoint { get; set; }

    public int stockType;
    public ChangeStockPriceGA(CharacterView characterView, int stockType, Dictionary<EStrategyType, float> changePrice = null, Dictionary<EStrategyType, float> changePersentDictionary = null, bool markPoint = false)
    {
        this.characterView = characterView;
        this.stockType = stockType;
        ChangePriceDictionary = changePrice ?? new();
        ChangePricePersentDictionary = changePersentDictionary ?? new();
        MarkPoint = markPoint;
    }
}