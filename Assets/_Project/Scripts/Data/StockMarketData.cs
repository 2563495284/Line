using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 股票市场类型
/// </summary>
public enum EStockType
{
    Oil,      // 石油
    Steel,    // 钢铁
    Cotton    // 棉花
}

/// <summary>
/// 单个股市的数据
/// </summary>
[System.Serializable]
public class SingleStockMarketData
{
    [Header("基础信息")]
    public EStockType stockType;
    public string stockName;
    public string stockSymbol;
    public Color themeColor = Color.white;

    [Header("价格信息")]
    public float currentPrice;
    public float initialPrice = 100f;
    public float minPrice = 1f;
    public float maxPrice = 1000f;

    [Header("持有信息")]
    public int playerHoldings; // 玩家持有数量
    public float totalValue;   // 总价值

    [Header("历史数据")]
    public List<float> priceHistory = new List<float>();
    public int maxHistoryCount = 40;

    [Header("波动设置")]
    public float baseVolatility = 1f; // 基础波动性
    public float currentVolatility = 1f; // 当前波动性

    public SingleStockMarketData(EStockType type)
    {
        stockType = type;
        SetupStockInfo();
        currentPrice = initialPrice;
        priceHistory.Add(currentPrice);
        UpdateTotalValue();
    }

    /// <summary>
    /// 设置股票基础信息
    /// </summary>
    private void SetupStockInfo()
    {
        switch (stockType)
        {
            case EStockType.Oil:
                stockName = "石油";
                stockSymbol = "OIL";
                themeColor = new Color(0.2f, 0.2f, 0.2f); // 黑色
                baseVolatility = 1.2f; // 石油波动较大
                break;
            case EStockType.Steel:
                stockName = "钢铁";
                stockSymbol = "STL";
                themeColor = new Color(0.7f, 0.7f, 0.7f); // 银灰色
                baseVolatility = 0.8f; // 钢铁相对稳定
                break;
            case EStockType.Cotton:
                stockName = "棉花";
                stockSymbol = "CTN";
                themeColor = new Color(0.9f, 0.9f, 0.8f); // 米白色
                baseVolatility = 1.0f; // 棉花中等波动
                break;
        }
        currentVolatility = baseVolatility;
    }

    /// <summary>
    /// 更新价格
    /// </summary>
    public void UpdatePrice(float newPrice)
    {
        currentPrice = Mathf.Clamp(newPrice, minPrice, maxPrice);

        // 更新历史记录
        priceHistory.Add(currentPrice);
        if (priceHistory.Count > maxHistoryCount)
        {
            priceHistory.RemoveAt(0);
        }

        UpdateTotalValue();
    }

    /// <summary>
    /// 更新总价值
    /// </summary>
    public void UpdateTotalValue()
    {
        totalValue = playerHoldings * currentPrice;
    }

    /// <summary>
    /// 买入股票
    /// </summary>
    public bool BuyStock(int amount, float availableMoney)
    {
        float cost = amount * currentPrice;
        if (cost <= availableMoney)
        {
            playerHoldings += amount;
            UpdateTotalValue();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 卖出股票
    /// </summary>
    public bool SellStock(int amount)
    {
        if (amount <= playerHoldings)
        {
            playerHoldings -= amount;
            UpdateTotalValue();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取价格变化百分比
    /// </summary>
    public float GetPriceChangePercent()
    {
        if (priceHistory.Count < 2) return 0f;

        float previousPrice = priceHistory[priceHistory.Count - 2];
        if (previousPrice == 0) return 0f;

        return ((currentPrice - previousPrice) / previousPrice) * 100f;
    }

    /// <summary>
    /// 获取相对于初始价格的变化百分比
    /// </summary>
    public float GetTotalChangePercent()
    {
        if (initialPrice == 0) return 0f;
        return ((currentPrice - initialPrice) / initialPrice) * 100f;
    }

    /// <summary>
    /// 获取显示字符串
    /// </summary>
    public string GetDisplayString()
    {
        float changePercent = GetPriceChangePercent();
        string changeSymbol = changePercent > 0 ? "+" : "";
        return $"{stockName} ({stockSymbol}) ¥{currentPrice:F2} {changeSymbol}{changePercent:F1}%";
    }
}
