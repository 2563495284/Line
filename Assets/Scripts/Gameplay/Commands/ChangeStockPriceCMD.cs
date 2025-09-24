using System.Collections.Generic;
public class ChangeStockPriceCMD : LevelCommand
{
    public CharacterView characterView;
    public Dictionary<EStrategyType, float> ChangePricePersentDictionary;
    public Dictionary<EStrategyType, float> ChangePriceDictionary;
    public bool MarkPoint { get; set; }

    public EStockType stockType;
    public ChangeStockPriceCMD(CharacterView characterView, EStockType stockType, Dictionary<EStrategyType, float> changePrice = null, Dictionary<EStrategyType, float> changePersentDictionary = null, bool markPoint = false)
    {
        this.characterView = characterView;
        this.stockType = stockType;
        ChangePriceDictionary = changePrice ?? new();
        ChangePricePersentDictionary = changePersentDictionary ?? new();
        MarkPoint = markPoint;
    }
}