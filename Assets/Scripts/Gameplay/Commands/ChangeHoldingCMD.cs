public class ChangeHoldingCMD : LevelCommand
{
    public int Amount { get; set; }

    public int StockId { get; set; }


    public ChangeHoldingCMD(int amount, int stockId)
    {
        Amount = amount;
        StockId = stockId;
    }
}
