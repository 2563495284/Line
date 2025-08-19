using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private MoneyUI moneyUI;
    [SerializeField] private TripleKLineDisplay tripleKLineDisplay;

    [Header("调试")]
    [SerializeField] private bool showDebugInfo = true;

    // 玩家资金
    private float currentMoney = 10000f;

    // 事件
    public event Action<EStockType, int, bool> OnStockTraded; // 股票类型，数量，是否买入
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ChangeStockPriceGA>(ChangeStockPricePerformer);
        ActionSystem.AttachPerformer<TradeSpecificStockGA>(TradeSpecificStockPerformer);
        ActionSystem.AttachPerformer<ChangeMoneyGA>(ChangeMoneyPerformer);
        ActionSystem.AttachPerformer<UseStockMaterialGA>(UseStockMaterialPerformer);

        ActionSystem.SubscribeReaction<NextRoundTurnGA>(NextRoundTurnPostReaction, ReactionTiming.POST);

    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ChangeStockPriceGA>();
        ActionSystem.DetachPerformer<TradeSpecificStockGA>();
        ActionSystem.DetachPerformer<ChangeMoneyGA>();
        ActionSystem.DetachPerformer<UseStockMaterialGA>();

        ActionSystem.UnsubscribeReaction<NextRoundTurnGA>(NextRoundTurnPostReaction, ReactionTiming.POST);
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
        UpdateUI();
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

        UpdateUI();
        UpdateKLineDisplays();

        if (showDebugInfo)
        {
            Debug.Log("所有股票价格已刷新");
        }
    }

    #endregion

    #region GameAction Performers

    /// <summary>
    /// 处理股票价格上涨
    /// </summary>
    private IEnumerator ChangeStockPricePerformer(ChangeStockPriceGA action)
    {
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

        bool success = false;
        float cost = 0f;

        if (action.Amount > 0) // 买入
        {
            cost = action.Amount * market.currentPrice;
            if (cost <= currentMoney)
            {
                success = market.BuyStock(action.Amount, currentMoney);
                if (success)
                {
                    currentMoney -= cost;
                }
            }
        }
        else if (action.Amount < 0) // 卖出
        {
            int sellAmount = -action.Amount;
            success = market.SellStock(sellAmount);
            if (success)
            {
                currentMoney += sellAmount * market.currentPrice;
            }
        }

        if (success)
        {
            OnStockTraded?.Invoke(action.StockType, action.Amount, action.Amount > 0);
            UpdateUI();

            if (showDebugInfo)
            {
                string operation = action.Amount > 0 ? "买入" : "卖出";
                Debug.Log($"{operation} {market.stockName} {math.abs(action.Amount)}股，" +
                         $"价格: {market.currentPrice:F2}，总额: {cost:F2}");
            }
        }

        yield return null;
    }

    /// <summary>
    /// 处理金币变化
    /// </summary>
    private IEnumerator ChangeMoneyPerformer(ChangeMoneyGA action)
    {
        currentMoney += action.Amount;
        UpdateUI();

        if (showDebugInfo)
        {
            string operation = action.Amount > 0 ? "获得" : "失去";
            Debug.Log($"{operation} {math.abs(action.Amount):F2} 金币，当前: {currentMoney:F2}");
        }

        yield return null;
    }

    /// <summary>
    /// 处理股票材料使用
    /// </summary>
    private IEnumerator UseStockMaterialPerformer(UseStockMaterialGA action)
    {
        var market = GetStockMarket(action.StockType);
        if (market == null || market.playerHoldings < action.Amount)
        {
            Debug.LogWarning($"材料不足: {action.StockType} 需要{action.Amount}，拥有{market?.playerHoldings ?? 0}");
            yield break;
        }

        market.SellStock(action.Amount);
        UpdateUI();

        if (showDebugInfo)
        {
            Debug.Log($"使用材料: {market.stockName} x{action.Amount}");
        }

        yield return null;
    }
    private void NextRoundTurnPostReaction(NextRoundTurnGA action)
    {
        RefreshAllStockPrices();
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

    /// <summary>
    /// 检查是否有足够的股票材料
    /// </summary>
    public bool HasEnoughMaterial(EStockType stockType, int amount)
    {
        return GetStockHoldings(stockType) >= amount;
    }

    /// <summary>
    /// 检查是否有足够的金币买入
    /// </summary>
    public bool CanAffordStock(EStockType stockType, int amount)
    {
        var market = GetStockMarket(stockType);
        if (market == null) return false;

        float cost = amount * market.currentPrice;
        return cost <= currentMoney;
    }

    #endregion

    #region UI Updates

    /// <summary>
    /// 更新UI显示
    /// </summary>
    private void UpdateUI()
    {
        if (moneyUI != null)
        {
            moneyUI.UpdateMoneyText(currentMoney);

            // 更新总资产显示
            float totalAssets = GetTotalAssetValue();
            // moneyUI.UpdateTotalAssetsText(totalAssets); // 如果有这个方法的话
        }
    }

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

    #region Debug Methods

    /// <summary>
    /// 打印所有股市状态
    /// </summary>
    [ContextMenu("打印股市状态")]
    public void PrintAllStockStatus()
    {
        Debug.Log("=== 股市状态 ===");
        Debug.Log($"当前金币: {currentMoney:F2}");
        Debug.Log($"总资产: {GetTotalAssetValue():F2}");

        foreach (var market in stockMarkets)
        {
            Debug.Log($"{market.stockName}: 价格{market.currentPrice:F2}, " +
                     $"持有{market.playerHoldings}股, 价值{market.totalValue:F2}, " +
                     $"变化{market.GetPriceChangePercent():+F1;-F1}%");
        }
    }
    #endregion
}
