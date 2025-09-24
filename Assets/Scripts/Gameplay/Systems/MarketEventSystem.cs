using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EEventCardType
{
    Bull,
    Bear,
    Neutral
}

/// <summary>
/// 市场事件系统（多股市）：每隔 N 回合有概率对随机股票播报新闻并影响价格
/// </summary>
public class MarketEvent_System : LevelSystem
{
    private int roundCounter = 0;

    public MarketEvent_System(LevelController ctrl) : base(ctrl)
    {
    }
    public override void EnableSystem()
    {
        Ctrl.AddRection<NextRoundTurnCMD>(OnNextRoundTurnPostReaction, ReactionTiming.POST);
    }
    public override void DisableSystem()
    {
        Ctrl.RemoveRection<NextRoundTurnCMD>(OnNextRoundTurnPostReaction, ReactionTiming.POST);
    }


    #region 市场事件执行逻辑

    /// <summary>
    /// NextRoundTurnCMD触发的市场事件
    /// </summary>
    private void OnNextRoundTurnPostReaction(NextRoundTurnCMD nextRound)
    {
        ExecuteMarketEvents();
    }


    /// <summary>
    /// 执行市场事件的核心逻辑
    /// </summary>
    private void ExecuteMarketEvents()
    {
        roundCounter++;
        if (roundCounter % Cfg.roundInterval != 0) return;
        TryTriggerMarketNewsForRandomStock(EStockType.Oil, 0.8f, 0.3f);
        TryTriggerMarketNewsForRandomStock(EStockType.Steel, 0.6f, 0.5f);
        TryTriggerMarketNewsForRandomStock(EStockType.Cotton, 0.3f, 0.7f);
    }

    #endregion

    private void TryTriggerMarketNewsForRandomStock(EStockType stockType, float eventChance, float effectFactor)
    {
        var stock = Data.GetStockModel(stockType);
        if (stock == null) return;
        if (UnityEngine.Random.Range(0, 1) > eventChance) return; // 本次不触发

        // 根据相对初始价格的偏离选择事件方向
        float deviation = (stock.price - stock.Cfg.initialPrice) / Mathf.Max(1e-5f, stock.Cfg.initialPrice);
        EEventCardType eventType = SelectEventTypeByDeviation(deviation);

        // 应用价格影响
        float impactPercent = GetImpactPercentByEvent(eventType) * effectFactor;
        ApplyPriceImpact(stock.type, impactPercent);


        // 标题包含品类名
        string stockName = MultiStockSystem.Ins?.GetStockMarket(stockType)?.stockName ?? stockType.ToString();
        string title = $"新闻 - {stockName}";

        // 内容依据品类与事件类型
        string content = OilMarketMessages.GetRandomMessage(stockType, eventType);
        Data.newsHistories.Add(new NewsItemData()
        {
            id = Guid.NewGuid().ToString(),
            title = title,
            content = content,
            newsType = NewsType.MarketEvent,
            timestamp = Time.time,
            isMerged = false
        });
        Ctrl.Notify(NotifyConst.PopupNews, new PopupNewsArgs()
        {
            title = title,
            content = content,
            newsType = NewsType.MarketEvent
        });
        Ctrl.Notify(NotifyConst.UpdateNewsHistory);

    }

    private EEventCardType SelectEventTypeByDeviation(float deviation)
    {
        float absDev = Mathf.Abs(deviation);
        float bullWeight = deviation < -0.02f ? 0.6f : (absDev < 0.02f ? 0.2f : 0.3f);
        float bearWeight = deviation > 0.02f ? 0.6f : (absDev < 0.02f ? 0.2f : 0.3f);
        float neutralWeight = 1f - Mathf.Max(bullWeight, bearWeight) + 0.2f;

        float total = bullWeight + bearWeight + neutralWeight;
        float r = MUtils.RandF() * total;
        if (r < bullWeight) return EEventCardType.Bull;
        if (r < bullWeight + bearWeight) return EEventCardType.Bear;
        return EEventCardType.Neutral;
    }

    private float GetImpactPercentByEvent(EEventCardType type)
    {
        Vector2 range = type switch
        {
            EEventCardType.Bull => Cfg.bullImpactPercentRange,
            EEventCardType.Bear => Cfg.bearImpactPercentRange,
            _ => Cfg.neutralImpactPercentRange
        };
        return MUtils.RandF(range.x, range.y);
    }

    private void ApplyPriceImpact(EStockType stockType, float percent)
    {
        // 将百分比影响应用到三个策略键，MultiStockSystem 的 performer 会读取其中一个键
        var percentDict = new Dictionary<EStrategyType, float>
        {
            { EStrategyType.medium, percent },
            { EStrategyType.aggressive, percent },
            { EStrategyType.conservative, percent },
        };

        var cv = NPCSystem.Ins.GetRandomNPCView();
        if (cv == null)
        {
            Debug.LogWarning("PlayerView 未就绪，价格影响未应用");
            return;
        }

        var CMD = new ChangeStockPriceCMD(cv, stockType, null, percentDict);
        Ctrl.AddCMD(CMD);
    }

}