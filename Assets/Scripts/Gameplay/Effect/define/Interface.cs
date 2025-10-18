using System;
using System.Collections;
using System.Collections.Generic;

public interface IEffectTargetToStock
{
    IEnumerator ApplyToStock(IEffectSource source, StockModel stock);
}
public interface IReleaseEffect
{
    IEnumerator Apply(IEffectSource source);
}
public interface IAutoEffect_RoundStart
{
    IEnumerator ApplyInRoundStart(IEffectSource source);
}
public interface IAutoEffect_BeforeCalc
{
    IEnumerator ApplyBeforeCalc(IEffectSource source);
}
public interface IAutoEffect_RoundEnd
{
    IEnumerator ApplyInRoundEnd(IEffectSource source);
}
public interface IGrowTarget
{
    void GrowingVal(IEffectSource source, Func<int, float, float> func);
}