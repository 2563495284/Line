using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;
using GameConfig;

/// <summary>
/// 多股市管理系统
/// </summary>
class PriceChangeFactor
{
    //比例变化值
    public float percent = 0;
    //增量变化值
    public float increment = 0;
}
public class StockSystem : LevelSystem
{

    private bool IsShowTradeDebugLogs => Cfg.showDebugInfo;


    private Dictionary<int, PriceChangeFactor> stockPriceFactorMap = new();
    // 当前资金状态
    private float currentMoney;

    public StockSystem(LevelController ctrl) : base(ctrl)
    {
        stockPriceFactorMap = new();
        Config.StockConfig.list.ToList().ForEach(e => stockPriceFactorMap.Add(e.Id, new()));
    }
    public override void EnableSystem()
    {
        currentMoney = 200000f;
        Ctrl.BindProcessor<ChangePricePercentCMD>(ChangePricePercentProcessor);
        Ctrl.BindProcessor<ChangePriceIncrementCMD>(ChangePriceIncrementProcessor);
        Ctrl.BindProcessor<TradeSpecificStockCMD>(TradeSpecificStockProcessor);
        Ctrl.BindProcessor<ChangeMoneyCMD>(ChangeMoneyProcessor);
        Ctrl.BindProcessor<ChangeHoldingCMD>(ChangeStockHoldingProcessor);
        Ctrl.AddRection<NextRoundTurnCMD>(NextRoundTurnPostReaction, ReactionTiming.POST);
        Ctrl.Notify(NotifyConst.UpdateStockChart);
    }
    public override void DisableSystem()
    {
        Ctrl.UnbindPerformer<ChangePricePercentCMD>();
        Ctrl.UnbindPerformer<ChangePriceIncrementCMD>();
        Ctrl.UnbindPerformer<TradeSpecificStockCMD>();
        Ctrl.UnbindPerformer<ChangeMoneyCMD>();
        Ctrl.UnbindPerformer<ChangeHoldingCMD>();
        Ctrl.RemoveRection<NextRoundTurnCMD>(NextRoundTurnPostReaction, ReactionTiming.POST);
    }



    #region GameAction Performers
    [TraceableCoroutine("ChangePricePercent")]
    private IEnumerator ChangePricePercentProcessor(ChangePricePercentCMD cmd)
    {
        stockPriceFactorMap[cmd.stockId].percent += cmd.percent * (cmd.isFromPlayer ? Data.PlayerInfluenceToPrice : Data.EnvInfluenceToPrice);
        if (cmd.isMarkPoint)
            Ctrl.Notify(NotifyConst.SetPointState, new SetPointStateArgs()
            {
                stockId = cmd.stockId,
                state = cmd.percent > 0 ? PointState.Bullish : PointState.Bearish
            });
        yield break;
    }
    [TraceableCoroutine("ChangePriceIncrement")]
    private IEnumerator ChangePriceIncrementProcessor(ChangePriceIncrementCMD cmd)
    {
        stockPriceFactorMap[cmd.stockId].increment += cmd.increment * (cmd.isFromPlayer ? Data.PlayerInfluenceToPrice : Data.EnvInfluenceToPrice);
        if (cmd.isMarkPoint)
            Ctrl.Notify(NotifyConst.SetPointState, new SetPointStateArgs()
            {
                stockId = cmd.stockId,
                state = cmd.increment > 0 ? PointState.Bullish : PointState.Bearish
            });
        yield break;
    }
    /// <summary>
    /// 处理特定股票交易（支持尽可能多地买卖模式）
    /// </summary>
    [TraceableCoroutine("TradeStock")]
    private IEnumerator TradeSpecificStockProcessor(TradeSpecificStockCMD cmd)
    {
        var market = Data.GetStockModel(cmd.StockId);
        if (market == null)
        {
            Debug.LogError($"未找到股票类型: {cmd.StockId}");
            yield break;
        }

        int amount = (int)math.floor(cmd.Amount * PlayerAttributeSystem.Ins.GetStockCourageBonus());
        int actualTradeAmount = 0;
        // 尽可能多地买卖模式
        if (amount > 0) // 买入意向
        {
            // 计算能买入的最大数量
            int maxBuyAmount = (int)Math.Floor(currentMoney / market.price);
            actualTradeAmount = Math.Min(amount, maxBuyAmount);

            if (actualTradeAmount <= 0)
            {
                if (IsShowTradeDebugLogs)
                    Debug.LogWarning($"[MultiStockSystem] 资金不足，无法买入 {cmd.StockId} 股票。当前资金: {currentMoney:F2}，股价: {market.price:F2}");
                TipsSystem.Ins.ShowTip("资金不足，无法买入股票");
                Utils.ShakeCamera();
                yield break;
            }

            if (IsShowTradeDebugLogs)
                Debug.Log($"[MultiStockSystem] 买入 {cmd.StockId}: 请求 {amount} 股，实际买入 {actualTradeAmount} 股 (最大化模式)");
        }
        else // 卖出意向
        {
            // 计算能卖出的最大数量
            int maxSellAmount = market.holding;
            actualTradeAmount = Math.Max(amount, -maxSellAmount); // amount是负数

            if (actualTradeAmount >= 0)
            {
                if (IsShowTradeDebugLogs)
                    Debug.LogWarning($"[MultiStockSystem] 持有量不足，无法卖出 {cmd.StockId} 股票。当前持有: {market.holding} 股");
                TipsSystem.Ins.ShowTip("持有量不足，无法卖出股票");
                Utils.ShakeCamera();
                yield break;
            }

            if (IsShowTradeDebugLogs)
                Debug.Log($"[MultiStockSystem] 卖出 {cmd.StockId}: 请求卖出 {-amount} 股，实际卖出 {-actualTradeAmount} 股 (最大化模式)");
        }
        // // 传统固定数量交易模式
        // if (!CanTradeStock(action.StockType, action.Amount))
        // {
        //     if (showTradeDebugLogs)
        //         Debug.LogWarning($"[MultiStockSystem] 无法交易 {action.Amount} 股 {action.StockType}");
        //     Utils.ShakeCamera();
        //     yield break;
        // }

        // actualTradeAmount = action.Amount;
        // if (showTradeDebugLogs)
        //     Debug.Log($"[MultiStockSystem] 交易 {action.StockType}: {actualTradeAmount} 股 (固定数量模式)");

        // 执行交易
        ChangeHoldingCMD changeStockCMD = new ChangeHoldingCMD(actualTradeAmount, cmd.StockId);
        Ctrl.ExeCMD(changeStockCMD);

        ChangeMoneyCMD changeMoneyCMD = new ChangeMoneyCMD(-actualTradeAmount * market.price);
        Ctrl.ExeCMD(changeMoneyCMD);

        Ctrl.Notify(NotifyConst.SetPointState, new SetPointStateArgs()
        {
            stockId = cmd.StockId,
            state = actualTradeAmount > 0 ? PointState.Buy : PointState.Sell
        });

        yield return null;
    }

    private void NextRoundTurnPostReaction(NextRoundTurnCMD action)
    {
        foreach (var stock in Data.stocks)
        {
            int stockId = stock.stockId;
            float price = stock.price;
            PriceChangeFactor factor = stockPriceFactorMap[stockId];
            float newPrice = (price + factor.increment) * (1 + factor.percent);
            stock.GrowPrice(newPrice);
        }
        Ctrl.Notify(NotifyConst.UpdateStockChart);
    }
    [TraceableCoroutine("ChangeMoney")]
    private IEnumerator ChangeMoneyProcessor(ChangeMoneyCMD action)
    {
        currentMoney += action.Amount;
        currentMoney = Mathf.Max(0, currentMoney);
        yield return null;
    }

    [TraceableCoroutine("ChangeHolding")]
    private IEnumerator ChangeStockHoldingProcessor(ChangeHoldingCMD cmd)
    {
        var market = Data.GetStockModel(cmd.StockId);
        if (market == null)
        {
            Debug.LogError($"未找到股票类型: {cmd.StockId}");
            yield break;
        }
        market.holding = Mathf.Max(0, market.holding + cmd.Amount);
        Ctrl.ExeCMD(new ChangePricePercentCMD(Math.Sign(cmd.Amount) * 0.3f, cmd.StockId, true, false));
        yield return null;
    }

    #endregion
}
