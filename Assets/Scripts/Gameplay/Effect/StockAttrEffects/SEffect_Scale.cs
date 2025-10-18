using System;
using System.Collections;

public class SEffect_Scale : StockAttrEffect, IAutoEffect_BeforeCalc, IGrowTarget
{
    public float mul = 1;

    public SEffect_Scale(string effectKey, string[] args, float probablity = 1, int growId = 0) : base(effectKey, args, probablity, growId)
    {
        mul = float.Parse(args[0]);
    }

    public IEnumerator ApplyBeforeCalc(IEffectSource source)
    {
        yield return ExeCMD(new ScalePerformanceCMD(mul, (source as StockAttrData).StockId));
    }

    public void GrowingVal(IEffectSource source, Func<int, float, float> func)
    {
        mul = func(growId, mul);
    }
}