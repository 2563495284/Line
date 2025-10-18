
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectGroup : EffectBase, IEffectTargetToStock, IReleaseEffect, IAutoEffect_BeforeCalc, IAutoEffect_RoundEnd, IAutoEffect_RoundStart, IGrowTarget
{
    public readonly List<EffectBase> effects = new();
    public readonly List<float> weight = new();


    public EffectGroup(List<EffectBase> effects, List<float> weights) : base("", null, 1, 0)
    {
        if (effects.Count != weights.Count)
        {
            Debug.LogError("EffectGroup Generate Error");
            return;
        }
        this.effects = effects;
        this.weight = weights;
    }

    public IEnumerator ApplyToStock(IEffectSource source, StockModel stock)
    {
        IEffectTargetToStock eff = GetEffect<IEffectTargetToStock>();
        if (eff != null)
            yield return eff.ApplyToStock(source, stock);
        else
            yield break;
    }

    public IEnumerator Apply(IEffectSource source)
    {
        IReleaseEffect eff = GetEffect<IReleaseEffect>();
        if (eff != null)
            yield return eff.Apply(source);
        else
            yield break;
    }

    public IEnumerator ApplyBeforeCalc(IEffectSource source)
    {
        IAutoEffect_BeforeCalc eff = GetEffect<IAutoEffect_BeforeCalc>();
        if (eff != null)
            yield return eff.ApplyBeforeCalc(source);
        else
            yield break;
    }


    public IEnumerator ApplyInRoundEnd(IEffectSource source)
    {
        IAutoEffect_RoundEnd eff = GetEffect<IAutoEffect_RoundEnd>();
        if (eff != null)
            yield return eff.ApplyInRoundEnd(source);
        else
            yield break;
    }

    public IEnumerator ApplyInRoundStart(IEffectSource source)
    {
        IAutoEffect_RoundStart eff = GetEffect<IAutoEffect_RoundStart>();
        if (eff != null)
            yield return eff.ApplyInRoundStart(source);
        else
            yield break;
    }

    private T GetEffect<T>() where T : class
    {
        return effects.WeiRand(weight) as T;
    }

    public void GrowingVal(IEffectSource source, Func<int, float, float> func)
    {
        effects.ForEach(e =>
        {
            if (e is IGrowTarget t)
                t.GrowingVal(source, func);
        });
    }

}