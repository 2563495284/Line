using System.Collections;

public class SEffect_Transfer : StockAttrEffect, IAutoEffect_RoundStart
{
    public int fromFactorId;
    public int toFactorId;
    public float volume;
    public SEffect_Transfer(string effectKey, string[] args, float probablity = 1, int growId = 0) : base(effectKey, args, probablity, growId)
    {
        fromFactorId = int.Parse(args[0]);
        toFactorId = int.Parse(args[1]);
        volume = float.Parse(args[2]);
    }

    public IEnumerator ApplyInRoundStart(IEffectSource source)
    {
        int stockId = (source as StockAttrData).StockId;
        yield return ExeCMD(new TransferStockFactorCMD(stockId, fromFactorId, toFactorId, volume));
    }
}