using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 预测系统 - 管理股价预测和延迟结算
/// </summary>
public class PredictionSystem : Singleton<PredictionSystem>
{
    [Header("预测管理")]
    [SerializeField] private List<PredictionData> activePredictions = new List<PredictionData>();

    [Header("调试信息")]
    [SerializeField] private bool showDebugInfo = true;

    public List<PredictionData> ActivePredictions => new List<PredictionData>(activePredictions);
    public int ActivePredictionCount => activePredictions.Count;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<PredictionGA>(PredictionPerformer);

    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<PredictionGA>();
    }

    #region Performers

    /// <summary>
    /// 处理新的预测
    /// </summary>
    private IEnumerator PredictionPerformer(PredictionGA predictionGA)
    {
        float currentPrice = StockSystem.Instance.CurrentStockPrice;

        PredictionData newPrediction = new PredictionData(
            predictionGA.PredictionType,
            currentPrice,
            predictionGA.RewardMoneyAmount,
            predictionGA.RewardStockAmount,
            predictionGA.PenaltyMoneyAmount,
            predictionGA.PenaltyStockAmount,
            predictionGA.DelayRounds
        );

        activePredictions.Add(newPrediction);
        yield return null;
    }

    #endregion

    #region Event Handlers

    #endregion

    #region Prediction Management

    /// <summary>
    /// 更新所有预测的倒计时
    /// </summary>
    public void UpdatePredictionCountdown()
    {
        List<PredictionData> toResolve = new List<PredictionData>();

        foreach (PredictionData prediction in activePredictions)
        {
            if (!prediction.isResolved)
            {
                prediction.remainingRounds--;

                if (prediction.remainingRounds <= 0)
                {
                    toResolve.Add(prediction);
                }
            }
        }

        // 结算到期的预测
        foreach (PredictionData prediction in toResolve)
        {
            ResolvePrediction(prediction);
        }
        for (int i = activePredictions.Count - 1; i >= 0; i--)
        {
            PredictionData prediction = activePredictions[i];
            if (prediction.isResolved)
            {
                activePredictions.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 结算单个预测
    /// </summary>
    private void ResolvePrediction(PredictionData prediction)
    {
        if (prediction.isResolved) return;

        float currentPrice = StockSystem.Instance.CurrentStockPrice;
        bool wasCorrect = prediction.CheckPrediction(currentPrice);

        prediction.isResolved = true;
        prediction.wasCorrect = wasCorrect;

        // 计算奖励或惩罚
        float moneyChange = wasCorrect ? prediction.rewardMoneyAmount : -prediction.penaltyMoneyAmount;
        int stockChange = wasCorrect ? prediction.rewardStockAmount : -prediction.penaltyStockAmount;
        // 执行金币变化
        if (moneyChange != 0)
        {
            ChangeMoneyGA changeMoneyGA = new ChangeMoneyGA(moneyChange);
            ActionSystem.Instance.Perform(changeMoneyGA);
        }

        // 执行股票变化
        if (stockChange != 0)
        {
            ChangeStockGA changeStockGA = new ChangeStockGA(stockChange);
            ActionSystem.Instance.Perform(changeStockGA);
        }

        string result = wasCorrect ? "正确" : "错误";
        string direction = prediction.predictionType == EPredictionType.Rise ? "上涨" : "下跌";
        string moneyAction = wasCorrect ? "获得" : "失去";
        string stockAction = wasCorrect ? "获得" : "失去";

        NewsSystem.Instance.BroadcastPrediction(
            $"预测{direction} - {result}!",
            $"初始价格: {prediction.initialPrice:F2} -> 当前价格: {currentPrice:F2} | " +
            (moneyChange == 0 ? "" : $"{moneyAction} {Mathf.Abs(moneyChange):F2} 金币 | ") +
            (stockChange == 0 ? "" : $"{stockAction} {Mathf.Abs(stockChange):F2} 股票")
        );
    }

    /// <summary>
    /// 手动结算所有预测（调试用）
    /// </summary>
    [ContextMenu("结算所有预测")]
    public void ResolveAllPredictions()
    {
        List<PredictionData> toResolve = new List<PredictionData>();

        foreach (PredictionData prediction in activePredictions)
        {
            if (!prediction.isResolved)
            {
                toResolve.Add(prediction);
            }
        }

        foreach (PredictionData prediction in toResolve)
        {
            ResolvePrediction(prediction);
        }
    }

    /// <summary>
    /// 清除已结算的预测
    /// </summary>
    [ContextMenu("清除已结算预测")]
    public void ClearResolvedPredictions()
    {
        activePredictions.RemoveAll(p => p.isResolved);

        if (showDebugInfo)
        {
            Debug.Log("已清除所有已结算的预测");
        }
    }

    /// <summary>
    /// 获取预测统计信息
    /// </summary>
    public (int total, int resolved, int correct, int remaining) GetPredictionStats()
    {
        int total = activePredictions.Count;
        int resolved = 0;
        int correct = 0;
        int remaining = 0;

        foreach (PredictionData prediction in activePredictions)
        {
            if (prediction.isResolved)
            {
                resolved++;
                if (prediction.wasCorrect)
                {
                    correct++;
                }
            }
            else
            {
                remaining++;
            }
        }

        return (total, resolved, correct, remaining);
    }

    #endregion

    #region Debug Methods

    /// <summary>
    /// 打印当前所有预测状态
    /// </summary>
    [ContextMenu("打印预测状态")]
    public void PrintPredictionStatus()
    {
        if (activePredictions.Count == 0)
        {
            Debug.Log("当前没有活跃的预测");
            return;
        }

        Debug.Log($"=== 预测状态 (共{activePredictions.Count}个) ===");

        for (int i = 0; i < activePredictions.Count; i++)
        {
            PredictionData prediction = activePredictions[i];
            string status = prediction.isResolved ?
                (prediction.wasCorrect ? "正确✓" : "错误✗") :
                $"进行中 ({prediction.remainingRounds}回合剩余)";

            Debug.Log($"{i + 1}. {prediction.GetPredictionDescription()} - {status}");
        }
    }

    #endregion
}
