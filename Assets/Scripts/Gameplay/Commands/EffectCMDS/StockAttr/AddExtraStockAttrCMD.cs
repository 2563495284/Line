using GameConfig;

public class AddExtraStockAttrCMD : LevelCommand
{
    public int stockId;
    public StockAttrType attrType;
    public int round;
    public AddExtraStockAttrCMD(int stockId, StockAttrType attrType, int round = -1)
    {
        this.stockId = stockId;
        this.attrType = attrType;
        this.round = round;
    }
}