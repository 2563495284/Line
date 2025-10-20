using System;
using System.Collections.Generic;
using System.Linq;
using GameConfig;
public class StockRoundInfo : IIndexableElement<int>
{

    public int stockId;
    public int round;
    public int strategyId = 0;
    public float factor_strength = 1;//公司实力
    public float factor_senity = 1;//市场情绪敏感度
    public float factor_industry = 1;//行业冲击受力
    public float performance = 0;//业绩指数
    public float price = 100;//股价
    public float holding = 0;//持股数量
    public StockRoundInfo Clone()
    {
        return new()
        {
            round = round,
            strategyId = strategyId,
            factor_senity = factor_senity,
            factor_industry = factor_industry,
            factor_strength = factor_strength,
            performance = performance,
            price = price,
            holding = holding
        };
    }

    public int GetKey()
    {
        return round;
    }
}
public class CalcFactorInfo
{
    public float performanceAdd = 0;
    public float performanceMul = 1;
    public void Reset()
    {
        performanceAdd = 0;
        performanceMul = 1;
    }
}
public class StockModel : IIndexableElement<int>
{
    private static int CNT = 1;
    public StockConfig cfg;
    public int stockId;
    public DList<int, StockRoundInfo> roundInfoHistory = new();
    public List<StockAttrData> attrs = new();
    public int remainRound = 0;//决策链剩余回合数
    public int CurStrategyId => roundInfoHistory.Peek().strategyId;//决策链id
    public StockRoundInfo Info => roundInfoHistory.Peek();
    public CalcFactorInfo calcFactor = new();
    public StockModel(StockConfig cfg)
    {
        this.cfg = cfg;
        this.stockId = CNT++;
        roundInfoHistory = new()
        {
            new(){
                stockId = stockId,
                round = 0,
                factor_strength = cfg.factor1_wei,
                factor_senity = cfg.factor2_wei,
                factor_industry = cfg.factor3_wei,
                performance = 0,
                price = cfg.originPrice,
                holding = 0
            }
        };
        remainRound = 0;
        calcFactor = new();
        calcFactor.Reset();
    }
    public void EnterRound()
    {
        StockRoundInfo newInfo = roundInfoHistory.Peek().Clone();
        newInfo.round++;
        newInfo.performance = 0;
        calcFactor.Reset();
    }
    public void NextStrategy(int strategyId)
    {
        Info.strategyId = strategyId;
        this.remainRound = Config.StockStrategyConfig.Get(strategyId).Round;
        List<StockAttrData> extraAttrs = attrs.Where(e => e.isExtra).ToList();
        List<StockAttrData> newAttrs = ParseAttrStr(Config.StockStrategyConfig.Get(strategyId).Attrs);
        attrs = new List<StockAttrData>().Concat(newAttrs).Concat(extraAttrs).ToList();
    }
    public void ExitRound()
    {
        remainRound--;
        List<StockAttrData> dels = new();
        for (int i = 0; i < attrs.Count; i++)
        {
            attrs[i].ExitRound();
            if (attrs[i].round == 0)
                dels.Add(attrs[i]);
        }
        dels.ForEach(e => attrs.Remove(e));
    }
    private List<StockAttrData> ParseAttrStr(IReadOnlyList<string> args)
    {
        List<StockAttrData> result = new();
        for (int i = 0; i < args.Count; i++)
        {
            string[] subStr = args[i].Split('*');
            int delay = 0;
            if (subStr.Length > 1)
                delay = int.Parse(subStr[1]);
            subStr = subStr[0].Split("/");
            int round = -1;
            if (subStr.Length > 1)
                round = int.Parse(subStr[1]);
            StockAttrType attrType = (StockAttrType)int.Parse(subStr[0]);
            result.Add(new(stockId, attrType, round, delay, false));
        }
        return result;
    }
    public int GetKey()
    {
        return stockId;
    }
}