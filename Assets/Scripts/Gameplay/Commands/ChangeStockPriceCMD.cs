using System.Collections.Generic;
public class ChangeStockPriceByEnemyCMD : LevelCommand
{
    public Dictionary<EStrategyType, float> ChangePricePersentDictionary;
    public Dictionary<EStrategyType, float> ChangePriceDictionary;
    public bool MarkPoint { get; set; }

    public int stockId;
    public ChangeStockPriceByEnemyCMD(int stockId, Dictionary<EStrategyType, float> changePrice = null, Dictionary<EStrategyType, float> changePersentDictionary = null, bool markPoint = false)
    {
        this.stockId = stockId;
        ChangePriceDictionary = changePrice ?? new();
        ChangePricePersentDictionary = changePersentDictionary ?? new();
        MarkPoint = markPoint;
    }
}