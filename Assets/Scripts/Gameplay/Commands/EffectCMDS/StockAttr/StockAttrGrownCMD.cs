public class StockAttrGrownCMD : LevelCommand
{
    public int attrId;
    public int stockId;
    public StockAttrGrownCMD(int attrId, int stockId)
    {
        this.attrId = attrId;
        this.stockId = stockId;
    }
}