public class RemoveExtraStockAttrCMD : LevelCommand
{
    public int stockId;
    public int num;
    public RemoveExtraStockAttrCMD(int stockId, int num)
    {
        this.stockId = stockId;
        this.num = num;
    }
}