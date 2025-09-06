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
public class MarketEventSystem : Singleton<MarketEventSystem>
{
    [Header("触发回合配置（多股市）")]
    [SerializeField] private int roundInterval = 2; // 每隔 N 回合尝试触发

    [Header("价格影响区间（百分比）")]
    [SerializeField] private Vector2 bullImpactPercentRange = new Vector2(1.0f, 4.0f);
    [SerializeField] private Vector2 neutralImpactPercentRange = new Vector2(-0.1f, 0.1f);
    [SerializeField] private Vector2 bearImpactPercentRange = new Vector2(-4.0f, -1.0f);

    private int roundCounter = 0;

    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<NextRoundTurnGA>(OnNextRoundTurnPostReaction, ReactionTiming.POST);
        ActionSystem.SubscribeReaction<MadeInHeavenExecuteGA>(OnMadeInHeavenExecutePostReaction, ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<NextRoundTurnGA>(OnNextRoundTurnPostReaction, ReactionTiming.POST);
        ActionSystem.UnsubscribeReaction<MadeInHeavenExecuteGA>(OnMadeInHeavenExecutePostReaction, ReactionTiming.POST);
    }

    #region 天堂制造事件处理

    #endregion

    #region 市场事件执行逻辑

    /// <summary>
    /// NextRoundTurnGA触发的市场事件
    /// </summary>
    private void OnNextRoundTurnPostReaction(NextRoundTurnGA nextRound)
    {
        if (!MadeInHeavenSystem.Instance.IsMadeInHeavenActive)
        {
            ExecuteMarketEvents();
        }
    }

    /// <summary>
    /// MadeInHeavenExecuteGA触发的市场事件
    /// </summary>
    private void OnMadeInHeavenExecutePostReaction(MadeInHeavenExecuteGA action)
    {
        if (MadeInHeavenSystem.Instance.IsMadeInHeavenActive)
        {
            ExecuteMarketEvents();
        }
    }

    /// <summary>
    /// 执行市场事件的核心逻辑
    /// </summary>
    private void ExecuteMarketEvents()
    {
        roundCounter++;
        if (roundCounter % roundInterval != 0) return;
        TryTriggerMarketNewsForRandomStock(EStockType.Oil, 0.8f, 0.3f);
        TryTriggerMarketNewsForRandomStock(EStockType.Steel, 0.6f, 0.5f);
        TryTriggerMarketNewsForRandomStock(EStockType.Cotton, 0.3f, 0.7f);
    }

    #endregion

    private void TryTriggerMarketNewsForRandomStock(EStockType stockType, float eventChance, float effectFactor)
    {
        var market = MultiStockSystem.Instance.GetStockMarket(stockType);
        if (market == null) return;
        if (Random.value > eventChance) return; // 本次不触发

        // 根据相对初始价格的偏离选择事件方向
        float deviation = (market.currentPrice - market.initialPrice) / Mathf.Max(1e-5f, market.initialPrice);
        EEventCardType eventType = SelectEventTypeByDeviation(deviation);

        // 应用价格影响
        float impactPercent = GetImpactPercentByEvent(eventType) * effectFactor;
        ApplyPriceImpact(market.stockType, impactPercent);

        // 播报新闻
        NewsSystem.Instance?.BroadcastMarketEvent(market.stockType, eventType);
    }

    private EEventCardType SelectEventTypeByDeviation(float deviation)
    {
        float absDev = Mathf.Abs(deviation);
        float bullWeight = deviation < -0.02f ? 0.6f : (absDev < 0.02f ? 0.2f : 0.3f);
        float bearWeight = deviation > 0.02f ? 0.6f : (absDev < 0.02f ? 0.2f : 0.3f);
        float neutralWeight = 1f - Mathf.Max(bullWeight, bearWeight) + 0.2f;

        float total = bullWeight + bearWeight + neutralWeight;
        float r = Random.value * total;
        if (r < bullWeight) return EEventCardType.Bull;
        if (r < bullWeight + bearWeight) return EEventCardType.Bear;
        return EEventCardType.Neutral;
    }

    private float GetImpactPercentByEvent(EEventCardType type)
    {
        Vector2 range = type switch
        {
            EEventCardType.Bull => bullImpactPercentRange,
            EEventCardType.Bear => bearImpactPercentRange,
            _ => neutralImpactPercentRange
        };
        return Random.Range(range.x, range.y);
    }

    private void ApplyPriceImpact(EStockType stockType, float percent)
    {
        // 将百分比影响应用到三个策略键，MultiStockSystem 的 performer 会读取其中一个键
        var percentDict = new Dictionary<ECharacterStrategyType, float>
        {
            { ECharacterStrategyType.medium, percent },
            { ECharacterStrategyType.aggressive, percent },
            { ECharacterStrategyType.conservative, percent },
        };

        var cv = NPCSystem.Instance.GetRandomNPCView();
        if (cv == null)
        {
            Debug.LogWarning("PlayerView 未就绪，价格影响未应用");
            return;
        }

        var ga = new ChangeStockPriceGA(cv, stockType, null, percentDict);
        ActionSystem.Instance.Perform(ga);
    }
}
