using System;
using System.Collections;
using Unity.Collections;

public class SEffect_Add : StockAttrEffect, IAutoEffect_BeforeCalc, IGrowTarget
{
    public float performanceAdd = 0;
    public SEffect_Add(string effectKey, string[] args, float probablity = 1, int growId = 0) : base(effectKey, args, probablity, growId)
    {
        performanceAdd = float.Parse(args[0]);
    }

    public IEnumerator ApplyBeforeCalc(IEffectSource source)
    {
        StockAttrData from = source as StockAttrData;
        yield return ExeCMD(new AddPerformanceCMD(performanceAdd, from.StockId));
    }
    public void GrowingVal(IEffectSource source, Func<int, float, float> func)
    {
        performanceAdd = func(growId, performanceAdd);
    }

}