/// <summary>
/// 增量改变股票价格（玩家影响）
/// </summary>
public class ChangePriceIncrementCMD : LevelCommand
{
    public int stockId;
    public float increment;
    public bool isFromPlayer;
    public bool isMarkPoint;

    public ChangePriceIncrementCMD(float amount, int stockId, bool isFromPlayer, bool isMarkPoint)
    {
        increment = amount;
        this.stockId = stockId;
        this.isFromPlayer = isFromPlayer;
        this.isMarkPoint = isMarkPoint;
    }
}
