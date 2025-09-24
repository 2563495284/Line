using System;
using System.Collections.Generic;

public class LevelModel
{
    private LevelConfig cfg;
    public Dictionary<EAttrType, float> attrVals = new();
    public Dictionary<EStrategyType, float> ChangePricePersentDictionaryWhenBuy;
    public Dictionary<EStrategyType, float> ChangePriceDictionaryWhenBuy;
    public Dictionary<EStrategyType, float> ChangePricePersentDictionaryWhenSell;
    public Dictionary<EStrategyType, float> ChangePriceDictionaryWhenSell;
    public int mana = 0;
    public int money = 0;
    public List<StockModel> stocks = new();
    public List<PredictionData> activePredictions = new();
    public List<NewsItemData> newsHistories = new();
    public List<CardModel> handCards = new();
    public Queue<CardModel> drawCards = new();
    public Queue<CardModel> discardCards = new();

    public int CardNumPerTurn => cfg.baseCardsPerTurn + (int)GetAttrValue(EAttrType.Social);
    public int ManaPerTurn => cfg.baseManaPerTurn + (int)GetAttrValue(EAttrType.Wisdom);
    public LevelModel(LevelConfig cfg)
    {
        this.cfg = cfg;
        ChangePricePersentDictionaryWhenBuy = cfg.playerData.changePricePersentDictionaryWhenBuy.ToDictionary();
        ChangePriceDictionaryWhenBuy = cfg.playerData.changePriceDictionaryWhenBuy.ToDictionary();
        ChangePricePersentDictionaryWhenSell = cfg.playerData.changePricePersentDictionaryWhenSell.ToDictionary();
        ChangePriceDictionaryWhenSell = cfg.playerData.changePriceDictionaryWhenSell.ToDictionary();
        Global.Ins.stockCfg.ForEach(e => stocks.Add(new StockModel(cfg, e.stockType)));
    }
    public float StockTradeMul => 1 + GetAttrValue(EAttrType.Courage) * 10f / 100f;
    public StockModel GetStockModel(EStockType type) => stocks.Find(e => e.type == type);
    public float GetAttrValue(EAttrType type)
    {
        if (!attrVals.ContainsKey(type))
            attrVals.Add(type, 0);
        return attrVals[type];
    }
    public void ChangeAttrValue(EAttrType type, float delta)
    {
        if (!attrVals.ContainsKey(type))
            attrVals.Add(type, 0);
        attrVals[type] = Math.Max(0, delta);
    }
    public CardModel GetCardModel(int cardId)
    {
        return handCards.Find(e => e.cardId == cardId);
    }
    public void SetAttrValue(EAttrType type, float val)
    {
        if (!attrVals.ContainsKey(type))
            attrVals.Add(type, 0);
        attrVals[type] = val;
    }
}