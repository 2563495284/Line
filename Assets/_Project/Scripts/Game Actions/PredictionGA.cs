using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictionGA : GameAction
{
    public EPredictionType PredictionType { get; set; }
    public float RewardMoneyAmount { get; set; }
    public int RewardStockAmount { get; set; }
    public float PenaltyMoneyAmount { get; set; }
    public int PenaltyStockAmount { get; set; }
    public int DelayRounds { get; set; }

    public PredictionGA(
        EPredictionType predictionType,
        float rewardMoneyAmount,
        int rewardStockAmount,
        float penaltyMoneyAmount,
        int penaltyStockAmount,
        int delayRounds = 5
    )
    {
        PredictionType = predictionType;
        RewardMoneyAmount = rewardMoneyAmount;
        RewardStockAmount = rewardStockAmount;
        PenaltyMoneyAmount = penaltyMoneyAmount;
        PenaltyStockAmount = penaltyStockAmount;
        DelayRounds = delayRounds;
    }
}

public enum EPredictionType
{
    Rise, // 预测上涨
    Fall, // 预测下跌
}
