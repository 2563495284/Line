public class TradeSpecificStockCMD : LevelCommand
{
    public EStockType StockType { get; set; }
    public int Amount { get; set; } // 正数买入，负数卖出

    public TradeSpecificStockCMD(EStockType stockType, int amount)
    {
        StockType = stockType;
        Amount = amount;
    }
}
