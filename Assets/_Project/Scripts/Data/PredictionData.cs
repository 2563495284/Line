using System;
using UnityEngine;

[System.Serializable]
public class PredictionData
{
    [Header("预测信息")]
    public EPredictionType predictionType;
    public float initialPrice;
    public float rewardMoneyAmount;
    public int rewardStockAmount;
    public float penaltyMoneyAmount;
    public int penaltyStockAmount;

    [Header("时间信息")]
    public int remainingRounds;
    public DateTime createdTime;

    [Header("状态")]
    public bool isResolved;
    public bool wasCorrect;

    public PredictionData(
        EPredictionType type,
        float price,
        float rewardMoneyAmount,
        int rewardStockAmount,
        float penaltyMoneyAmount,
        int penaltyStockAmount,
        int rounds
    )
    {
        predictionType = type;
        initialPrice = price;
        this.rewardMoneyAmount = rewardMoneyAmount;
        this.rewardStockAmount = rewardStockAmount;
        this.penaltyMoneyAmount = penaltyMoneyAmount;
        this.penaltyStockAmount = penaltyStockAmount;
        remainingRounds = rounds;
        createdTime = DateTime.Now;
        isResolved = false;
        wasCorrect = false;
    }

    /// <summary>
    /// 检查预测是否正确
    /// </summary>
    public bool CheckPrediction(float currentPrice)
    {
        bool priceRose = currentPrice > initialPrice;

        switch (predictionType)
        {
            case EPredictionType.Rise:
                return priceRose;
            case EPredictionType.Fall:
                return !priceRose;
            default:
                return false;
        }
    }

    /// <summary>
    /// 获取预测描述
    /// </summary>
    public string GetPredictionDescription()
    {
        string direction = predictionType == EPredictionType.Rise ? "上涨" : "下跌";
        return $"预测股价{direction} (初始价格: {initialPrice:F2}, 剩余回合: {remainingRounds})";
    }
}
