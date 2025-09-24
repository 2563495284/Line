public class UseStockMaterialCMD : LevelCommand
{
    public EStockType StockType { get; set; }
    public int Amount { get; set; }

    public UseStockMaterialCMD(EStockType stockType, int amount)
    {
        StockType = stockType;
        Amount = amount;
    }
}
