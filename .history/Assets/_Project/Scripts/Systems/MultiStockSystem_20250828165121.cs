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

    [Header("交易设置")]
    [SerializeField] private bool enableMaximizeTrading = true; // 启用尽可能多地买卖模式
    [SerializeField] private bool showTradeDebugLogs = true; // 显示交易调试日志

    [Header("UI引用")]
    [SerializeField] private TripleKLineDisplay tripleKLineDisplay;

    [Header("资金配置")]
    [SerializeField] private FinancialData financialData;

    // 当前资金状态
    private float currentMoney;

    protected override void Awake()
    {
        base.Awake();
        InitializeFinancialSystem();
    }

    private void InitializeFinancialSystem()
    {
        if (financialData != null)
        {
            currentMoney = financialData.InitialMoney;
        }
        else
        {
            Debug.LogWarning("FinancialData 未设置，使用默认值 200000");
            currentMoney = 200000f;
        }
    }

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ChangeStockPriceGA>(ChangeStockPricePerformer);
        ActionSystem.AttachPerformer<TradeSpecificStockGA>(TradeSpecificStockPerformer);
        ActionSystem.AttachPerformer<TradeAllStockGA>(TradeAllStockPerformer);

        //改变金币
        ActionSystem.AttachPerformer<ChangeMoneyGA>(ChangeMoneyPerformer);
        //改变股票数量
        ActionSystem.AttachPerformer<ChangeStockGA>(ChangeStockPerformer);

        ActionSystem.SubscribeReaction<NextRoundTurnGA>(NextRoundTurnPostReaction, ReactionTiming.POST);

        ActionSystem.SubscribeReaction<MadeInHeavenExecuteGA>(StartMadeInHeavenPostReaction, ReactionTiming.PRE);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ChangeStockPriceGA>();
        ActionSystem.DetachPerformer<TradeSpecificStockGA>();
        ActionSystem.DetachPerformer<TradeAllStockGA>();

        //改变金币
        ActionSystem.DetachPerformer<ChangeMoneyGA>();
        //改变股票数量
        ActionSystem.DetachPerformer<ChangeStockGA>();

        ActionSystem.UnsubscribeReaction<NextRoundTurnGA>(NextRoundTurnPostReaction, ReactionTiming.POST);
        ActionSystem.UnsubscribeReaction<MadeInHeavenExecuteGA>(StartMadeInHeavenPostReaction, ReactionTiming.PRE);
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
    /// 处理特定股票交易（支持尽可能多地买卖模式）
    /// </summary>
    private IEnumerator TradeSpecificStockPerformer(TradeSpecificStockGA action)
    {
        var market = GetStockMarket(action.StockType);
        if (market == null)
        {
            Debug.LogError($"未找到股票类型: {action.StockType}");
            yield break;
        }

        LineView lineView = GetLineView(action.StockType);
        int actualTradeAmount = 0;

        if (enableMaximizeTrading)
        {
            // 尽可能多地买卖模式
            if (action.Amount > 0) // 买入意向
            {
                // 计算能买入的最大数量
                int maxBuyAmount = (int)Math.Floor(currentMoney / market.currentPrice);
                actualTradeAmount = Math.Min(action.Amount, maxBuyAmount);

                if (actualTradeAmount <= 0)
                {
                    if (showTradeDebugLogs)
                        Debug.LogWarning($"[MultiStockSystem] 资金不足，无法买入 {action.StockType} 股票。当前资金: {currentMoney:F2}，股价: {market.currentPrice:F2}");
                    Utils.ShakeCamera();
                    yield break;
                }

                if (showTradeDebugLogs)
                    Debug.Log($"[MultiStockSystem] 买入 {action.StockType}: 请求 {action.Amount} 股，实际买入 {actualTradeAmount} 股 (最大化模式)");
            }
            else // 卖出意向
            {
                // 计算能卖出的最大数量
                int maxSellAmount = market.playerHoldings;
                actualTradeAmount = Math.Max(action.Amount, -maxSellAmount); // action.Amount是负数

                if (actualTradeAmount >= 0)
                {
                    if (showTradeDebugLogs)
                        Debug.LogWarning($"[MultiStockSystem] 持有量不足，无法卖出 {action.StockType} 股票。当前持有: {market.playerHoldings} 股");
                    Utils.ShakeCamera();
                    yield break;
                }

                if (showTradeDebugLogs)
                    Debug.Log($"[MultiStockSystem] 卖出 {action.StockType}: 请求卖出 {-action.Amount} 股，实际卖出 {-actualTradeAmount} 股 (最大化模式)");
            }
        }
        else
        {
            // 传统固定数量交易模式
            if (!CanTradeStock(action.StockType, action.Amount))
            {
                if (showTradeDebugLogs)
                    Debug.LogWarning($"[MultiStockSystem] 无法交易 {action.Amount} 股 {action.StockType}");
                Utils.ShakeCamera();
                yield break;
            }

            actualTradeAmount = action.Amount;
            if (showTradeDebugLogs)
                Debug.Log($"[MultiStockSystem] 交易 {action.StockType}: {actualTradeAmount} 股 (固定数量模式)");
        }

        // 执行交易
        ChangeStockGA changeStockGA = new ChangeStockGA(actualTradeAmount, action.StockType);
        ActionSystem.Instance.Perform(changeStockGA);

        ChangeMoneyGA changeMoneyGA = new ChangeMoneyGA(-actualTradeAmount * market.currentPrice);
        ActionSystem.Instance.Perform(changeMoneyGA);

        // 设置视觉反馈
        if (actualTradeAmount > 0)
        {
            lineView.SetPointState(PointState.Buy);
        }
        else
        {
            lineView.SetPointState(PointState.Sell);
        }

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
        LineView lineView = GetLineView(action.StockType);
        if (action.TradeAllStockType == ETradeAllStockType.Buy)
        {
            int buyStockCount = (int)Math.Floor(currentMoney / market.currentPrice);
            ChangeStockGA changeStockGA = new ChangeStockGA(buyStockCount, action.StockType);
            ActionSystem.Instance.Perform(changeStockGA);
            ChangeMoneyGA changeMoneyGA = new ChangeMoneyGA(-buyStockCount * market.currentPrice);
            ActionSystem.Instance.Perform(changeMoneyGA);
            lineView.SetPointState(PointState.Buy);
        }
        else
        {
            ChangeStockGA changeStockGA = new ChangeStockGA(-market.playerHoldings, action.StockType);
            ActionSystem.Instance.Perform(changeStockGA);
            ChangeMoneyGA changeMoneyGA = new ChangeMoneyGA(market.playerHoldings * market.currentPrice);
            ActionSystem.Instance.Perform(changeMoneyGA);
            lineView.SetPointState(PointState.Sell);
        }
        yield return null;
    }
    private void UpdateStockPrice()
    {
        RefreshAllStockPrices();
        tripleKLineDisplay.UpdateAllKLines();
    }
    private void StartMadeInHeavenPostReaction(MadeInHeavenExecuteGA action)
    {
        if (MadeInHeavenSystem.Instance.IsMadeInHeavenActive)
        {
            UpdateStockPrice();
        }
    }

    private void NextRoundTurnPostReaction(NextRoundTurnGA action)
    {
        if (!MadeInHeavenSystem.Instance.IsMadeInHeavenActive)
        {
            UpdateStockPrice();
        }
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
        ChangeStockPriceGA changeStockPriceGA;
        if (action.Amount > 0)
        {
            changeStockPriceGA = new ChangeStockPriceGA(PlayerAttributeSystem.Instance.playerView, action.StockType, PlayerAttributeSystem.Instance.ChangePriceDictionaryWhenBuy, PlayerAttributeSystem.Instance.ChangePricePersentDictionaryWhenBuy);
        }
        else
        {
            changeStockPriceGA = new ChangeStockPriceGA(PlayerAttributeSystem.Instance.playerView, action.StockType, PlayerAttributeSystem.Instance.ChangePriceDictionaryWhenSell, PlayerAttributeSystem.Instance.ChangePricePersentDictionaryWhenSell);
        }
        ActionSystem.Instance.Perform(changeStockPriceGA);
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
    /// 获取资金配置数据
    /// </summary>
    // public FinancialData GetFinancialData()
    // {
    //     return financialData;
    // }

    /// <summary>
    /// 设置当前金币（使用配置的限制）
    /// </summary>
    public void SetCurrentMoney(float amount)
    {
        if (financialData != null)
        {
            currentMoney = financialData.ClampMoney(amount);
        }
        else
        {
            currentMoney = Mathf.Max(0, amount);
        }
    }

    /// <summary>
    /// 检查是否有足够的资金
    /// </summary>
    public bool HasEnoughMoney(float requiredAmount)
    {
        return currentMoney >= requiredAmount;
    }

    /// <summary>
    /// 格式化金钱显示
    /// </summary>
    public string FormatMoney(float amount)
    {
        if (financialData != null)
        {
            return financialData.FormatMoney(amount);
        }
        return amount.ToString("N0");
    }

    /// <summary>
    /// 格式化当前金钱显示
    /// </summary>
    public string FormatCurrentMoney()
    {
        return FormatMoney(currentMoney);
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

    /// <summary>
    /// 计算最大可交易数量
    /// </summary>
    /// <param name="stockType">股票类型</param>
    /// <param name="isBuying">是否为买入操作</param>
    /// <returns>最大可交易数量（买入为正数，卖出为负数）</returns>
    public int GetMaxTradeAmount(EStockType stockType, bool isBuying)
    {
        var market = GetStockMarket(stockType);
        if (market == null) return 0;

        if (isBuying)
        {
            // 买入：计算用所有资金能买多少股
            return (int)Math.Floor(currentMoney / market.currentPrice);
        }
        else
        {
            // 卖出：返回所有持有量（负数）
            return -market.playerHoldings;
        }
    }

    /// <summary>
    /// 设置是否启用最大化交易模式
    /// </summary>
    public void SetMaximizeTradingMode(bool enabled)
    {
        enableMaximizeTrading = enabled;
        if (showTradeDebugLogs)
        {
            Debug.Log($"[MultiStockSystem] 最大化交易模式已{(enabled ? "启用" : "禁用")}");
        }
    }
    public LineView GetLineView(EStockType eStockType)
    {
        return tripleKLineDisplay.GetLineView(eStockType);
    }
    public LineView GetLineViewRandom()
    {
        return tripleKLineDisplay.GetLineViewRandom();
    }

    /// <summary>
    /// 重置股市系统到初始状态
    /// </summary>
    public void ResetSystem()
    {
        // 重置玩家资金到初始值
        InitializeFinancialSystem();

        // 重置所有股市数据
        foreach (var market in stockMarkets)
        {
            market.currentPrice = market.initialPrice;
            market.tempPrice = market.initialPrice;
            market.playerHoldings = 0;
            market.currentVolatility = market.baseVolatility;
            market.priceHistory.Clear();
            market.priceHistory.Add(market.currentPrice);
        }

        // 更新K线显示
        UpdateKLineDisplays();
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
