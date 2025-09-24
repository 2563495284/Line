using System.Collections;
using UnityEngine;

public class Effect_ChangeStrategy : Effect
{
    [SerializeField] private EStrategyType strategyType;


    public override IEnumerator Run(IEffectEmitter from)
    {
        if (from is not EnemyModel)
            yield return null;
        yield return ExeCMD(new ChangeStrategyCMD(from as EnemyModel, strategyType));
    }
}