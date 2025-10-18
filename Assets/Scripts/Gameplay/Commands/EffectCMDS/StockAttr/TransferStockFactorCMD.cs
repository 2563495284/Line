public class TransferStockFactorCMD : LevelCommand
{
    public int stockId;
    public int fromFactorId;
    public int toFactorId;
    public float volume;
    public TransferStockFactorCMD(int stockId, int fromFactorId, int toFactorId, float volume)
    {
        this.stockId = stockId;
        this.fromFactorId = fromFactorId;
        this.toFactorId = toFactorId;
        this.volume = volume;
    }
}