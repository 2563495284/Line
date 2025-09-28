using System.Collections;
using UnityEngine;
//type # rewardMoney # rewardStock # penaltyMoney # penaltyStock # delayRounds
public class Effect_Predict : EffectWithTarget
{

    private EPredictionType predictionType;

    private float rewardMoneyAmount = 100f;

    private int rewardStockAmount = 100;

    private float penaltyMoneyAmount = 50f;

    private int penaltyStockAmount = 50;

    private int delayRounds = 5;

    public Effect_Predict(string[] args) : base(args)
    {
        predictionType = (EPredictionType)int.Parse(args[0]);
        rewardMoneyAmount = float.Parse(args[1]);
        rewardStockAmount = int.Parse(args[2]);
        penaltyMoneyAmount = float.Parse(args[3]);
        penaltyStockAmount = int.Parse(args[4]);
        delayRounds = int.Parse(args[5]);
    }

    public override IEnumerator Run(IEffectEmitter from, IEffectReceiver to)
    {
        if (!(to is StockModel))
            yield break;
        StockModel target = to as StockModel;
        yield return ExeCMD(new PredictionCMD(predictionType, rewardMoneyAmount, rewardStockAmount, penaltyMoneyAmount, penaltyStockAmount, target.stockId, delayRounds));


    }
}