using UnityEngine;
using System.Collections.Generic;
using System;
[Serializable]
public class StockConfig
{
    public bool isCoverStockAttr = false;
    public int maxRoundInStrategy = 4;
    public int positiveStrategyWeight = 5;
    public int neutralStrategyWeight = 3;
    public int negativeStrategyWeight = 2;
    public float factor1_wei = 3;
    public float factor2_wei = 5;
    public float factor3_wei = 2;
    public float originPrice = 100;

}
[CreateAssetMenu(fileName = "UniverseConfig", menuName = "ConfigData/LevelConfig", order = 0)]
public class LevelConfig : ScriptableObject
{
    public List<int> routeList = new();
    public List<int> initialActionCards = new();
    public List<int> initialTradeCards = new();
    public List<StockConfig> stockCfgs = new() { new(), new() };

}