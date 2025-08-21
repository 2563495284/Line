using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;

/// <summary>
/// 多股市管理系统
/// </summary>
public class MultiStockSystem : Singleton<MultiStockSystem>
{
    [Header("股市配置")]
    [SerializeField] private List<SingleStockMarketData> stockMarkets = new List<SingleStockMarketData>();

    [Header("UI引用")]
    [SerializeField] private TripleKLineDisplay tripleKLineDisplay;
    // 玩家资金
    [SerializeField] public float currentMoney = 100000f;
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ChangeStockPriceGA>(ChangeStockPricePerformer);
        ActionSystem.AttachPerformer<TradeSpecificStockGA>(TradeSpecificStockPerformer);
        ActionSystem.AttachPerformer<TradeAllStockGA>(TradeAllStockPerformer);
        ActionSystem.AttachPerformer<NextRoundTurnGA>(NextRoundTurnPostReaction);

        //改变金币
        ActionSystem.AttachPerformer<ChangeMoneyGA>(ChangeMoneyPerformer);
        //改变股票数量
        ActionSystem.AttachPerformer<ChangeStockGA>(ChangeStockPerformer);

    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ChangeStockPriceGA>();
        ActionSystem.DetachPerformer<TradeSpecificStockGA>();
        ActionSystem.DetachPerformer<TradeAllStockGA>();
        ActionSystem.DetachPerformer<NextRoundTurnGA>();

        //改变金币
        ActionSystem.DetachPerformer<ChangeMoneyGA>();
        //改变股票数量
        ActionSystem.DetachPerformer<ChangeStockGA>();
    }

    #region Initialization

    /// <summary>
    /// 初始化股市
    /// </summary>
    public void InitializeStockMarkets()
    {
        if (stockMarkets.Count == 0)
        {
            // 创建三个股市
            foreach (EStockType stockType in Enum.GetValues(typeof(EStockType)))
            {
                stockMarkets.Add(new SingleStockMarketData(stockType));
            }
        }

        // 确保所有股市都正确初始化
        foreach (var market in stockMarkets)
        {
            if (market.priceHistory.Count == 0)
            {
                market.priceHistory.Add(market.currentPrice);
            }
        }
        UpdateKLineDisplays();
    }

    #endregion

    #region Price Management

    /// <summary>
    /// 刷新所有股票价格
    /// </summary>
    private void RefreshAllStockPrices()
    {
        foreach (var market in stockMarkets)
        {
            market.MarkPrice();
        }

        UpdateKLineDisplays();
    }

    #endregion

    #region GameAction Performers

    /// <summary>
    /// 处理股票价格上涨
    /// </summary>
    private IEnumerator ChangeStockPricePerformer(ChangeStockPriceGA action)
    {
        if (action.characterView.CharacterType == ECharacterType.Player)
        {
            // 使用ToList()避免在遍历时修改集合的异常
            foreach (var item in action.ChangePriceDictionary.ToList())
            {
                action.ChangePriceDictionary[item.Key] = item.Value * PlayerAttributeSystem.Instance.GetStockInfluenceBonus();
            }
            // 使用ToList()避免在遍历时修改集合的异常
            foreach (var item in action.ChangePricePersentDictionary.ToList())
            {
                action.ChangePricePersentDictionary[item.Key] = item.Value * PlayerAttributeSystem.Instance.GetStockInfluenceBonus();
            }
        }
        else if (action.characterView.CharacterType == ECharacterType.NPC)
        {
            // 使用ToList()避免在遍历时修改集合的异常
            foreach (var item in action.ChangePriceDictionary.ToList())
            {
                action.ChangePriceDictionary[item.Key] = item.Value * PlayerAttributeSystem.Instance.GetStockEnvironmentBonus();
            }
            // 使用ToList()避免在遍历时修改集合的异常
            foreach (var item in action.ChangePricePersentDictionary.ToList())
            {
                action.ChangePricePersentDictionary[item.Key] = item.Value * PlayerAttributeSystem.Instance.GetStockEnvironmentBonus();
            }
        }
        var market = GetStockMarket(action.stockType);
        if (market == null)
        {
            Debug.LogError($"未找到股票类型: {action.stockType}");
            yield break;
        }

        float tempPrice = market.tempPrice;
        ECharacterStrategyType characterStrategyType = action.characterView.StrategyType;
        int index = (int)characterStrategyType;
        float changeStockPrice = 0;
        float changeStockPersentPrice = 0;
        action.ChangePriceDictionary.TryGetValue(characterStrategyType, out changeStockPrice);
        action.ChangePricePersentDictionary.TryGetValue(characterStrategyType, out changeStockPersentPrice);
        // 计算新的价格
        float priceIncrease = changeStockPrice;
        priceIncrease += Mathf.Round(tempPrice * changeStockPersentPrice) / 100f;

        tempPrice = Mathf.Round((tempPrice + priceIncrease) * 100f) / 100f;

        // 限制价格范围并保留两位小数
        tempPrice = Mathf.Round(Mathf.Clamp(tempPrice, market.minPrice, market.maxPrice) * 100f) / 100f;

        // 更新价格历史
        market.UpdatePrice(tempPrice);
        Debug.Log($"NPC {action.characterView.name} 投资策略: {characterStrategyType} 价格: {tempPrice:F2} -> {tempPrice:F2} ({priceIncrease:F2})");
        yield return null;
    }
    /// <summary>
    /// 处理特定股票交易
    /// </summary>
    private IEnumerator TradeSpecificStockPerformer(TradeSpecificStockGA action)
    {
        var market = GetStockMarket(action.StockType);
        if (market == null)
        {
            Debug.LogError($"未找到股票类型: {action.StockType}");
            yield break;
        }

        if (!CanTradeStock(action.StockType, action.Amount))
        {
            Utils.ShakeCamera();
            yield break;
        }
        ChangeStockGA changeStockGA = new ChangeStockGA(action.Amount, action.StockType);
        ActionSystem.Instance.Perform(changeStockGA);
        ChangeMoneyGA changeMoneyGA = new ChangeMoneyGA(-action.Amount * market.currentPrice);
        ActionSystem.Instance.Perform(changeMoneyGA);
        yield return null;
    }
    private IEnumerator TradeAllStockPerformer(TradeAllStockGA action)
    {
        var market = GetStockMarket(action.StockType);
        if (market == null)
        {
            Debug.LogError($"未找到股票类型: {action.StockType}");
            yield break;
        }
        if (action.TradeAllStockType == ETradeAllStockType.Buy)
        {
            int buyStockCount = (int)Math.Floor(currentMoney / market.currentPrice);
            ChangeStockGA changeStockGA = new ChangeStockGA(buyStockCount, action.StockType);
            ActionSystem.Instance.Perform(changeStockGA);
            ChangeMoneyGA changeMoneyGA = new ChangeMoneyGA(-buyStockCount * market.currentPrice);
            ActionSystem.Instance.Perform(changeMoneyGA);
        }
        else
        {
            ChangeStockGA changeStockGA = new ChangeStockGA(-market.playerHoldings, action.StockType);
            ActionSystem.Instance.Perform(changeStockGA);
            ChangeMoneyGA changeMoneyGA = new ChangeMoneyGA(market.playerHoldings * market.currentPrice);
            ActionSystem.Instance.Perform(changeMoneyGA);
        }
        yield return null;
    }
    private IEnumerator NextRoundTurnPostReaction(NextRoundTurnGA action)
    {
        RefreshAllStockPrices();
        yield return null;
    }
    private IEnumerator ChangeMoneyPerformer(ChangeMoneyGA action)
    {
        currentMoney += action.Amount;
        yield return null;
    }

    private IEnumerator ChangeStockPerformer(ChangeStockGA action)
    {
        var market = GetStockMarket(action.StockType);
        if (market == null)
        {
            Debug.LogError($"未找到股票类型: {action.StockType}");
            yield break;
        }
        market.playerHoldings = Mathf.Max(0, market.playerHoldings + action.Amount);
        yield return null;
    }

    #endregion

    #region Public Interface

    /// <summary>
    /// 获取指定股市数据
    /// </summary>
    public SingleStockMarketData GetStockMarket(EStockType stockType)
    {
        return stockMarkets.Find(market => market.stockType == stockType);
    }

    /// <summary>
    /// 获取所有股市数据
    /// </summary>
    public List<SingleStockMarketData> GetAllStockMarkets()
    {
        return new List<SingleStockMarketData>(stockMarkets);
    }

    /// <summary>
    /// 获取当前金币
    /// </summary>
    public float GetCurrentMoney()
    {
        return currentMoney;
    }

    /// <summary>
    /// 获取总资产价值
    /// </summary>
    public float GetTotalAssetValue()
    {
        float totalValue = currentMoney;
        foreach (var market in stockMarkets)
        {
            totalValue += market.totalValue;
        }
        return totalValue;
    }

    /// <summary>
    /// 获取指定股票的持有量
    /// </summary>
    public int GetStockHoldings(EStockType stockType)
    {
        var market = GetStockMarket(stockType);
        return market?.playerHoldings ?? 0;
    }

    public bool CanTradeStock(EStockType stockType, int amount)
    {
        amount = (int)math.floor(amount * PlayerAttributeSystem.Instance.GetStockCourageBonus());
        var market = GetStockMarket(stockType);

        if (market == null)
        {
            return false;
        }
        if (amount <= 0)
        {
            return market.playerHoldings >= -amount;
        }


        return market.currentPrice * amount <= currentMoney;
    }
    public LineView GetLineView(EStockType eStockType)
    {
        return tripleKLineDisplay.GetLineView(eStockType);
    }
    public LineView GetLineViewRandom()
    {
        return tripleKLineDisplay.GetLineViewRandom();
    }
    #endregion

    #region UI Updates

    /// <summary>
    /// 更新K线显示
    /// </summary>
    private void UpdateKLineDisplays()
    {
        if (tripleKLineDisplay != null)
        {
            tripleKLineDisplay.UpdateAllKLines();
        }
    }

    #endregion
}
