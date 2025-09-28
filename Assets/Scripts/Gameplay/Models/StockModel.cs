using System;
using System.Collections.Generic;
using GameConfig;

public class StockModel : IEffectReceiver
{
    public int stockId;
    public float price = 100;
    public int holding = 200;//持股数量
    public float volatility = 1f; //当前波动性
    public List<float> priceHistory = new();
    public float tempPrice;
    public StockConfigItem Cfg => Config.StockConfig.Get(stockId);
    public float minPrice = 1;
    public float maxPrice = 1000;
    public StockModel(int stockId)
    {
        this.stockId = stockId;
        price = Cfg.OriginPrice;
        volatility = Cfg.BaseVolatility;
        holding = 200;
        priceHistory = new() { price };
    }
    public void GrowPrice(float newPrice)
    {
        price = Math.Clamp(newPrice, 1, 1000);
        // 更新历史记录
        priceHistory.Add(price);
        if (priceHistory.Count > Global.Ins.maxPriceHistoryCnt)
        {
            priceHistory.RemoveAt(0);
        }
    }
}