public class TradeSpecificStockCMD : LevelCommand
{
    public int StockId { get; set; }
    public int Amount { get; set; } // 正数买入，负数卖出

    public TradeSpecificStockCMD(int stockId, int amount)
    {
        StockId = stockId;
        Amount = amount;
    }
}
