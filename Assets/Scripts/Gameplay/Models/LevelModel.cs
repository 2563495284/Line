using System;
using System.Collections.Generic;
using System.Linq;
using GameConfig;
using UnityEngine;

public class LevelModel
{
    private LevelConfig cfg;
    public Dictionary<AttrType, float> attrVals = new();
    public int mana = 0;
    public int money = 0;
    //回合数
    public int turn = 0;
    public List<StockModel> stocks = new();
    public List<PredictionData> activePredictions = new();
    public List<NewsItemData> newsHistories = new();
    public List<CardModel> handCards = new();
    public Queue<CardModel> drawCards = new();
    public Queue<CardModel> discardCards = new();
    public DList<int, CardModel> allCards = new();

    public int CardNumPerTurn => cfg.baseCardsPerTurn + (int)GetAttrValue(AttrType.Social);
    public int ManaPerTurn => cfg.baseManaPerTurn + (int)GetAttrValue(AttrType.Wisdom);
    public float PlayerInfluenceToPrice => 1 + Mathf.Min(10, GetAttrValue(AttrType.Charisma)) * 10f / 100f;
    public float EnvInfluenceToPrice => Mathf.Max(0.2f, 1 + Mathf.Clamp(GetAttrValue(AttrType.Fanaticism) - GetAttrValue(AttrType.Calmness), -30, 30) / 50f);
    public LevelModel(LevelConfig cfg)
    {
        this.cfg = cfg;
        turn = 1;
        foreach (var e in Config.StockConfig.list)
            stocks.Add(new StockModel(e.Id));
        allCards = new(cfg.cardCfgIds.Select(e => new CardModel(e)));
        var newCards = new List<CardModel>(allCards);
        newCards.Shuffle();
        drawCards = new(newCards);


    }
    public float StockTradeMul => 1 + GetAttrValue(AttrType.Courage) * 10f / 100f;
    public StockModel GetStockModel(int stockId) => stocks.Find(e => e.stockId == stockId);
    public float GetAttrValue(AttrType type)
    {
        if (!attrVals.ContainsKey(type))
            attrVals.Add(type, 0);
        return attrVals[type];
    }
    public void ChangeAttrValue(AttrType type, float delta)
    {
        if (!attrVals.ContainsKey(type))
            attrVals.Add(type, 0);
        attrVals[type] = Math.Max(0, delta);
    }
    public CardModel GetCardModel(int cardId)
    {
        return allCards[cardId];
    }
    public void SetAttrValue(AttrType type, float val)
    {
        if (!attrVals.ContainsKey(type))
            attrVals.Add(type, 0);
        attrVals[type] = val;
    }
}