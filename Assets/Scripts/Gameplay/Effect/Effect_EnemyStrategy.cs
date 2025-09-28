using System.Collections;
using UnityEngine;

public class Effect_EnemyStrategy : Effect
{
    private EStrategyType strategyType;

    public Effect_EnemyStrategy(string[] args) : base(args)
    {
        strategyType = (EStrategyType)int.Parse(args[0]);
    }

    public override IEnumerator Run(IEffectEmitter from)
    {
        if (from is not EnemyModel)
            yield return null;
        yield return ExeCMD(new ChangeStrategyCMD(from as EnemyModel, strategyType));
    }
}