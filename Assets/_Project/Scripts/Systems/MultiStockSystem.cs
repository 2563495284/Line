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

    [Header("价格刷新设置")]
    [SerializeField] private float priceRefreshInterval = 3f;
    [SerializeField] private Timer priceRefreshTimer;

    [Header("UI引用")]
    [SerializeField] private MoneyUI moneyUI;

    [Header("调试")]
    [SerializeField] private bool showDebugInfo = true;

    // 玩家资金
    private float currentMoney = 10000f;

    // 事件
    public event Action<EStockType, float, float> OnStockPriceChanged;
    public event Action<EStockType, int, bool> OnStockTraded; // 股票类型，数量，是否买入

    protected override void Awake()
    {
        base.Awake();
        InitializeStockMarkets();
    }

    private void Start()
    {
        StartPriceRefreshTimer();
        UpdateUI();
    }

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<TradeSpecificStockGA>(TradeSpecificStockPerformer);
        ActionSystem.AttachPerformer<ChangeMoneyGA>(ChangeMoneyPerformer);
        ActionSystem.AttachPerformer<UseStockMaterialGA>(UseStockMaterialPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<TradeSpecificStockGA>();
        ActionSystem.DetachPerformer<ChangeMoneyGA>();
        ActionSystem.DetachPerformer<UseStockMaterialGA>();

        StopPriceRefreshTimer();
    }

    #region Initialization

    /// <summary>
    /// 初始化股市
    /// </summary>
    private void InitializeStockMarkets()
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
    }

    #endregion

    #region Price Management

    /// <summary>
    /// 启动价格刷新定时器
    /// </summary>
    private void StartPriceRefreshTimer()
    {
        if (priceRefreshTimer == null)
        {
            priceRefreshTimer = gameObject.AddComponent<Timer>();
        }

        priceRefreshTimer.SetDuration(priceRefreshInterval);
        priceRefreshTimer.SetCountdown(true);
        priceRefreshTimer.SetLoop(true);
        priceRefreshTimer.OnTimerComplete += RefreshAllStockPrices;
        priceRefreshTimer.StartTimer();
    }

    /// <summary>
    /// 停止价格刷新定时器
    /// </summary>
    private void StopPriceRefreshTimer()
    {
        if (priceRefreshTimer != null)
        {
            priceRefreshTimer.StopTimer();
        }
    }

    /// <summary>
    /// 刷新所有股票价格
    /// </summary>
    private void RefreshAllStockPrices()
    {
        foreach (var market in stockMarkets)
        {
            RefreshStockPrice(market);
        }

        UpdateUI();

        if (showDebugInfo)
        {
            Debug.Log("所有股票价格已刷新");
        }
    }

    /// <summary>
    /// 刷新单个股票价格
    /// </summary>
    private void RefreshStockPrice(SingleStockMarketData market)
    {
        float oldPrice = market.currentPrice;

        // 基础随机波动
        float randomChange = UnityEngine.Random.Range(-5f, 5f);

        // 应用波动性
        randomChange *= market.currentVolatility;

        // 应用玩家魅力属性影响
        if (PlayerAttributeSystem.Instance != null)
        {
            float charismaBonus = PlayerAttributeSystem.Instance.GetAttributeValue(EPlayerAttributeType.Charisma);
            randomChange *= (1f + charismaBonus / 100f);
        }

        float newPrice = oldPrice + randomChange;
        market.UpdatePrice(newPrice);

        // 触发价格变化事件
        OnStockPriceChanged?.Invoke(market.stockType, oldPrice, market.currentPrice);

        if (showDebugInfo)
        {
            Debug.Log($"{market.stockName} 价格: {oldPrice:F2} -> {market.currentPrice:F2} ({randomChange:+F2;-F2})");
        }
    }

    #endregion

    #region GameAction Performers

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

    /// <summary>
    /// 测试买入股票
    /// </summary>
    [ContextMenu("测试买入石油")]
    public void TestBuyOil()
    {
        var tradeGA = new TradeSpecificStockGA(EStockType.Oil, 10);
        ActionSystem.Instance.Perform(tradeGA);
    }

    /// <summary>
    /// 测试卖出股票
    /// </summary>
    [ContextMenu("测试卖出石油")]
    public void TestSellOil()
    {
        var tradeGA = new TradeSpecificStockGA(EStockType.Oil, -5);
        ActionSystem.Instance.Perform(tradeGA);
    }

    /// <summary>
    /// 测试使用材料
    /// </summary>
    [ContextMenu("测试使用石油材料")]
    public void TestUseOilMaterial()
    {
        var useGA = new UseStockMaterialGA(EStockType.Oil, 2);
        ActionSystem.Instance.Perform(useGA);
    }

    #endregion
}
