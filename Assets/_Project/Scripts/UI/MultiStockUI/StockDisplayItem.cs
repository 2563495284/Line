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
            backgroundImage.color = Color.white;
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
            Color textColor = changePercent > 0 ? Color.green :
                             changePercent < 0 ? Color.red : Color.white;
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

    #endregion

    #region Button Actions

    /// <summary>
    /// 买入股票
    /// </summary>
    private void BuyStock(int amount)
    {
        if (MultiStockSystem.Ins == null) return;

        var tradeGA = new TradeSpecificStockGA(marketData.stockType, amount);
        ActionSystem.Ins.Perform(tradeGA);
    }

    /// <summary>
    /// 卖出股票
    /// </summary>
    private void SellStock(int amount)
    {
        if (MultiStockSystem.Ins == null) return;

        var tradeGA = new TradeSpecificStockGA(marketData.stockType, -amount);
        ActionSystem.Ins.Perform(tradeGA);
    }




    #endregion
}
