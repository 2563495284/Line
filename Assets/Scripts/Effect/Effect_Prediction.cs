using System.Collections;
using UnityEngine;

public class Effect_Prediction : EffectWithTarget
{

    [SerializeField]
    private EPredictionType predictionType;

    [SerializeField]
    private float rewardMoneyAmount = 100f;

    [SerializeField]
    private int rewardStockAmount = 100;

    [SerializeField]
    private float penaltyMoneyAmount = 50f;

    [SerializeField]
    private int penaltyStockAmount = 50;

    [SerializeField]
    private int delayRounds = 5;
    public override IEnumerator Run(IEffectEmitter from, IEffectReceiver to)
    {
        if (!(to is StockModel))
            yield break;
        StockModel target = to as StockModel;
        yield return ExeCMD(new PredictionCMD(predictionType, rewardMoneyAmount, rewardStockAmount, penaltyMoneyAmount, penaltyStockAmount, target.type, delayRounds));


    }
}