public class ScalePerformanceCMD : LevelCommand
{
    public float mul = 1;
    public int stockId = 1;
    public ScalePerformanceCMD(float mul, int stockId)
    {
        this.mul = mul;
        this.stockId = stockId;
    }
}