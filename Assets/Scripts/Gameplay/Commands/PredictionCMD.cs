
public class PredictionCMD : LevelCommand
{
    public EPredictionType PredictionType { get; set; }
    public float RewardMoneyAmount { get; set; }
    public int RewardStockAmount { get; set; }
    public float PenaltyMoneyAmount { get; set; }
    public int PenaltyStockAmount { get; set; }
    public int DelayRounds { get; set; }

    public EStockType StockType { get; set; }

    public PredictionCMD(
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

