using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 单个股票显示项
/// </summary>
public class StockDisplayItem : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private TextMeshProUGUI stockNameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI changeText;
    [SerializeField] private TextMeshProUGUI holdingsText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button sellButton;

    [Header("颜色设置")]
    [SerializeField] private Color positiveColor = Color.green;
    [SerializeField] private Color negativeColor = Color.red;
    [SerializeField] private Color neutralColor = Color.white;

    private SingleStockMarketData marketData;

    #region Initialization

    /// <summary>
    /// 初始化显示项
    /// </summary>
    public void Initialize(SingleStockMarketData data)
    {
        marketData = data;

        // 设置基础信息
        if (stockNameText != null)
        {
            stockNameText.text = $"{marketData.stockName} ({marketData.stockSymbol})";
        }

        // 设置主题颜色
        if (backgroundImage != null)
        {
            backgroundImage.color = marketData.themeColor;
        }

        // 设置按钮事件
        SetupButtons();

        // 初始更新
        UpdateDisplay();
    }

    /// <summary>
    /// 设置按钮事件
    /// </summary>
    private void SetupButtons()
    {
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => BuyStock(1));
        }

        if (sellButton != null)
        {
            sellButton.onClick.RemoveAllListeners();
            sellButton.onClick.AddListener(() => SellStock(1));
        }
    }

    #endregion

    #region Display Update

    /// <summary>
    /// 更新显示
    /// </summary>
    public void UpdateDisplay()
    {
        if (marketData == null) return;

        // 更新价格
        UpdatePriceDisplay();

        // 更新持有信息
        UpdateHoldingsDisplay();

        // 更新按钮状态
        UpdateButtonStates();
    }

    /// <summary>
    /// 更新价格显示
    /// </summary>
    private void UpdatePriceDisplay()
    {
        if (priceText != null)
        {
            priceText.text = $"¥{marketData.currentPrice:F2}";
        }

        if (changeText != null)
        {
            float changePercent = marketData.GetPriceChangePercent();
            string changeSymbol = changePercent > 0 ? "+" : "";
            changeText.text = $"{changeSymbol}{changePercent:F1}%";

            // 设置颜色
            Color textColor = changePercent > 0 ? positiveColor :
                             changePercent < 0 ? negativeColor : neutralColor;
            changeText.color = textColor;
        }
    }

    /// <summary>
    /// 更新持有信息显示
    /// </summary>
    private void UpdateHoldingsDisplay()
    {
        if (holdingsText != null)
        {
            holdingsText.text = $"持有: {marketData.playerHoldings}";
        }

        if (valueText != null)
        {
            valueText.text = $"价值: ¥{marketData.totalValue:F2}";
        }
    }

    /// <summary>
    /// 更新按钮状态
    /// </summary>
    private void UpdateButtonStates()
    {
        if (MultiStockSystem.Instance == null) return;

        // 更新买入按钮
        if (buyButton != null)
        {
            bool canBuy = MultiStockSystem.Instance.CanAffordStock(marketData.stockType, 1);
            buyButton.interactable = canBuy;
        }

        // 更新卖出按钮
        if (sellButton != null)
        {
            bool canSell = marketData.playerHoldings > 0;
            sellButton.interactable = canSell;
        }
    }

    #endregion

    #region Button Actions

    /// <summary>
    /// 买入股票
    /// </summary>
    private void BuyStock(int amount)
    {
        if (MultiStockSystem.Instance == null) return;

        var tradeGA = new TradeSpecificStockGA(marketData.stockType, amount);
        ActionSystem.Instance.Perform(tradeGA);
    }

    /// <summary>
    /// 卖出股票
    /// </summary>
    private void SellStock(int amount)
    {
        if (MultiStockSystem.Instance == null) return;

        var tradeGA = new TradeSpecificStockGA(marketData.stockType, -amount);
        ActionSystem.Instance.Perform(tradeGA);
    }

    /// <summary>
    /// 买入10股
    /// </summary>
    [ContextMenu("买入10股")]
    public void BuyStock10()
    {
        BuyStock(10);
    }

    /// <summary>
    /// 卖出10股
    /// </summary>
    [ContextMenu("卖出10股")]
    public void SellStock10()
    {
        SellStock(10);
    }

    /// <summary>
    /// 全部买入
    /// </summary>
    [ContextMenu("全部买入")]
    public void BuyAllAffordable()
    {
        if (MultiStockSystem.Instance == null) return;

        float availableMoney = MultiStockSystem.Instance.GetCurrentMoney();
        int maxBuyAmount = Mathf.FloorToInt(availableMoney / marketData.currentPrice);

        if (maxBuyAmount > 0)
        {
            BuyStock(maxBuyAmount);
        }
    }

    /// <summary>
    /// 全部卖出
    /// </summary>
    [ContextMenu("全部卖出")]
    public void SellAll()
    {
        if (marketData.playerHoldings > 0)
        {
            SellStock(marketData.playerHoldings);
        }
    }

    #endregion
}
