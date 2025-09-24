public class ChangeStockCMD : LevelCommand
{
    public int Amount { get; set; }

    public EStockType StockType { get; set; }


    public ChangeStockCMD(int amount, EStockType stockType)
    {
        Amount = amount;
        StockType = stockType;
    }
}
