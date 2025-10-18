using System.Collections;

public class SEffect_UpScale : StockAttrEffect, IAutoEffect_BeforeCalc
{
    public float mul = 1;
    public SEffect_UpScale(string effectKey, string[] args, float probablity = 1, int growId = 0) : base(effectKey, args, probablity, growId)
    {
        mul = float.Parse(args[0]);
    }

    public IEnumerator ApplyBeforeCalc(IEffectSource source)
    {
        StockModel stock = GM.LevelData.stocks[(source as StockAttrData).StockId];
        if (stock.Info.performance > 0)
            yield return ExeCMD(new ScalePerformanceCMD(mul, stock.stockId));
        else
            yield break;
    }
}