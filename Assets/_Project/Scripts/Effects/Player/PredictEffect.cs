using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictEffect : Effect
{
    [Header("预测设置")]
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

    public override GameAction GetGameAction()
    {
        PredictionGA predictionGA = new(predictionType, rewardMoneyAmount, rewardStockAmount, penaltyMoneyAmount, penaltyStockAmount, delayRounds);
        return predictionGA;
    }
}
