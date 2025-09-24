using System;
using System.Collections.Generic;

public class StockModel : IEffectReceiver
{
    public EStockType type;
    public float price = 100;
    public int holding = 200;//持股数量
    public float volatility = 1f; //当前波动性
    public List<float> priceHistory = new();
    public float tempPrice;
    private readonly int maxPriceHistory;
    public StockConfigItem Cfg => Global.Ins.GetStockCfg(type);
    public float minPrice = 1;
    public float maxPrice = 1000;
    public StockModel(LevelConfig cfg, EStockType type)
    {
        this.type = type;
        maxPriceHistory = cfg.maxPriceHistoryCnt;
        price = Cfg.initialPrice;
        volatility = Cfg.baseVolatility;
        holding = 200;
        priceHistory = new() { price };
        tempPrice = price;

    }
    public void UpdatePrice(float price)
    {
        tempPrice = Math.Clamp(price, minPrice, maxPrice);
    }
    public void MarkPrice()
    {
        price = tempPrice;

        // 更新历史记录
        priceHistory.Add(price);
        if (priceHistory.Count > maxPriceHistory)
        {
            priceHistory.RemoveAt(0);
        }
    }
    public float PriceChangePercent
    {
        get
        {
            if (priceHistory.Count < 2) return 0f;

            float previousPrice = priceHistory[priceHistory.Count - 2];
            if (previousPrice == 0) return 0f;

            return (price - previousPrice) / previousPrice * 100f;

        }
    }
}