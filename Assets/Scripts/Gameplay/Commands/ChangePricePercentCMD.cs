/// <summary>
/// 比例改变股票价格（玩家影响）
/// </summary>
public class ChangePricePercentCMD : LevelCommand
{
    public int stockId;
    public float percent;
    public bool isFromPlayer;
    public bool isMarkPoint;

    public ChangePricePercentCMD(float amount, int stockId, bool isFromPlayer, bool isMarkPoint)
    {
        percent = amount;
        this.stockId = stockId;
        this.isFromPlayer = isFromPlayer;
        this.isMarkPoint = isMarkPoint;
    }
}
