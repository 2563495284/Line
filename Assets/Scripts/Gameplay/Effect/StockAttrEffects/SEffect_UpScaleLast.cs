using System.Collections;

public class SEffect_UpScaleLast : StockAttrEffect, IAutoEffect_BeforeCalc
{
    public float mul;
    public SEffect_UpScaleLast(string effectKey, string[] args, float probablity = 1, int growId = 0) : base(effectKey, args, probablity, growId)
    {
        mul = float.Parse(args[0]);
    }

    public IEnumerator ApplyBeforeCalc(IEffectSource source)
    {
        StockModel stock = GM.LevelData.stocks[(source as StockAttrData).StockId];
        if (stock.roundInfoHistory.Count > 1 && stock.roundInfoHistory.Index(-2).performance > 0)
            yield return ExeCMD(new ScalePerformanceCMD(mul, stock.stockId));
        else
            yield break;
    }
}