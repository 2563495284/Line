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

    public EStockType StockType { get; set; }

    public PredictionGA(
        EPredictionType predictionType,
        float rewardMoneyAmount,
        int rewardStockAmount,
        float penaltyMoneyAmount,
        int penaltyStockAmount,
        EStockType stockType,
        int delayRounds
    )
    {
        PredictionType = predictionType;
        RewardMoneyAmount = rewardMoneyAmount;
        RewardStockAmount = rewardStockAmount;
        PenaltyMoneyAmount = penaltyMoneyAmount;
        PenaltyStockAmount = penaltyStockAmount;
        DelayRounds = delayRounds;
        StockType = stockType;
    }
}

