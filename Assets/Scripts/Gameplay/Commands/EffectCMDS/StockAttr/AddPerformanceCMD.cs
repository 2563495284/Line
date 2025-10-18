public class AddPerformanceCMD : LevelCommand
{
    public float addVal = 0;
    public int stockId = 0;
    public AddPerformanceCMD(float addVal, int stockid)
    {
        this.addVal = addVal;
        this.stockId = stockid;
    }
}