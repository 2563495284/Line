using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;

/// <summary>
/// 多股市管理系统
/// </summary>
public class StockSystem : LevelSystem
{

    private bool IsShowTradeDebugLogs => Cfg.showDebugInfo;



    // 当前资金状态
    private float currentMoney;

    public StockSystem(LevelController ctrl) : base(ctrl)
    {
    }
    public override void EnableSystem()
    {
        if (Cfg.financialData != null)
            currentMoney = 200000f;
        else
            currentMoney = Cfg.financialData.InitialMoney;
        Ctrl.BindPerformer<ChangeStockPriceCMD>(ChangeStockPricePerformer);
        Ctrl.BindPerformer<TradeSpecificStockCMD>(TradeSpecificStockPerformer);

        //改变金币
        Ctrl.BindPerformer<ChangeMoneyCMD>(ChangeMoneyPerformer);
        //改变股票数量
        Ctrl.BindPerformer<ChangeStockCMD>(ChangeStockPerformer);

        Ctrl.AddRection<NextRoundTurnCMD>(NextRoundTurnPostReaction, ReactionTiming.POST);


        Ctrl.Notify(NotifyConst.UpdateStockChart);
    }
    public override void DisableSystem()
    {

        Ctrl.UnbindPerformer<ChangeStockPriceCMD>();
        Ctrl.UnbindPerformer<TradeSpecificStockCMD>();

        //改变金币
        Ctrl.UnbindPerformer<ChangeMoneyCMD>();
        //改变股票数量
        Ctrl.UnbindPerformer<ChangeStockCMD>();

        Ctrl.RemoveRection<NextRoundTurnCMD>(NextRoundTurnPostReaction, ReactionTiming.POST);
    }


    #region Price Management

    /// <summary>
    /// 刷新所有股票价格
    /// </summary>
    private void RefreshAllStockPrices()
    {
        foreach (var stock in Data.stocks)
        {
            stock.MarkPrice();
        }
        Ctrl.Notify(NotifyConst.UpdateStockChart);
    }

    #endregion

    #region GameAction Performers

    /// <summary>
    /// 处理股票价格上涨
    /// </summary>
    private IEnumerator ChangeStockPricePerformer(ChangeStockPriceCMD cmd)
    {
        if (cmd.characterView.CharacterType == ECharacterType.Player)
        {
            // 使用ToList()避免在遍历时修改集合的异常
            foreach (var item in cmd.ChangePriceDictionary.ToList())
            {
                cmd.ChangePriceDictionary[item.Key] = item.Value * PlayerAttributeSystem.Ins.GetStockInfluenceBonus();
            }
            // 使用ToList()避免在遍历时修改集合的异常
            foreach (var item in cmd.ChangePricePersentDictionary.ToList())
            {
                cmd.ChangePricePersentDictionary[item.Key] = item.Value * PlayerAttributeSystem.Ins.GetStockInfluenceBonus();
            }
        }
        else if (cmd.characterView.CharacterType == ECharacterType.NPC)
        {
            // 使用ToList()避免在遍历时修改集合的异常
            foreach (var item in cmd.ChangePriceDictionary.ToList())
            {
                cmd.ChangePriceDictionary[item.Key] = item.Value * PlayerAttributeSystem.Ins.GetStockEnvironmentBonus();
            }
            // 使用ToList()避免在遍历时修改集合的异常
            foreach (var item in cmd.ChangePricePersentDictionary.ToList())
            {
                cmd.ChangePricePersentDictionary[item.Key] = item.Value * PlayerAttributeSystem.Ins.GetStockEnvironmentBonus();
            }
        }
        var market = GetStockMarket(cmd.stockType);
        if (market == null)
        {
            Debug.LogError($"未找到股票类型: {cmd.stockType}");
            yield break;
        }

        float tempPrice = market.tempPrice;
        EStrategyType characterStrategyType = cmd.characterView.StrategyType;
        cmd.ChangePriceDictionary.TryGetValue(characterStrategyType, out float changeStockPrice);
        cmd.ChangePricePersentDictionary.TryGetValue(characterStrategyType, out float changeStockPersentPrice);
        // 计算新的价格
        float priceIncrease = changeStockPrice;
        priceIncrease += Mathf.Round(tempPrice * changeStockPersentPrice) / 100f;

        tempPrice = Mathf.Round((tempPrice + priceIncrease) * 100f) / 100f;

        // 限制价格范围并保留两位小数
        tempPrice = Mathf.Round(Mathf.Clamp(tempPrice, market.minPrice, market.maxPrice) * 100f) / 100f;

        // 更新价格历史
        market.UpdatePrice(tempPrice);
        if (cmd.MarkPoint)
        {
            Ctrl.Notify(NotifyConst.SetPointState, new SetPointStateArgs()
            {
                stockType = cmd.stockType,
                state = tempPrice > market.price ? PointState.Bullish : PointState.Bearish
            });
        }
        Debug.Log($"NPC {cmd.characterView.name} 投资策略: {characterStrategyType} 价格: {tempPrice:F2} -> {tempPrice:F2} ({priceIncrease:F2})");
        yield return null;
    }
    /// <summary>
    /// 处理特定股票交易（支持尽可能多地买卖模式）
    /// </summary>
    private IEnumerator TradeSpecificStockPerformer(TradeSpecificStockCMD cmd)
    {
        var market = GetStockMarket(cmd.StockType);
        if (market == null)
        {
            Debug.LogError($"未找到股票类型: {cmd.StockType}");
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
                    Debug.LogWarning($"[MultiStockSystem] 资金不足，无法买入 {cmd.StockType} 股票。当前资金: {currentMoney:F2}，股价: {market.price:F2}");
                TipsSystem.Ins.ShowTip("资金不足，无法买入股票");
                Utils.ShakeCamera();
                yield break;
            }

            if (IsShowTradeDebugLogs)
                Debug.Log($"[MultiStockSystem] 买入 {cmd.StockType}: 请求 {amount} 股，实际买入 {actualTradeAmount} 股 (最大化模式)");
        }
        else // 卖出意向
        {
            // 计算能卖出的最大数量
            int maxSellAmount = market.holding;
            actualTradeAmount = Math.Max(amount, -maxSellAmount); // amount是负数

            if (actualTradeAmount >= 0)
            {
                if (IsShowTradeDebugLogs)
                    Debug.LogWarning($"[MultiStockSystem] 持有量不足，无法卖出 {cmd.StockType} 股票。当前持有: {market.holding} 股");
                TipsSystem.Ins.ShowTip("持有量不足，无法卖出股票");
                Utils.ShakeCamera();
                yield break;
            }

            if (IsShowTradeDebugLogs)
                Debug.Log($"[MultiStockSystem] 卖出 {cmd.StockType}: 请求卖出 {-amount} 股，实际卖出 {-actualTradeAmount} 股 (最大化模式)");
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
        ChangeStockCMD changeStockCMD = new ChangeStockCMD(actualTradeAmount, cmd.StockType);
        Ctrl.ExeCMD(changeStockCMD);

        ChangeMoneyCMD changeMoneyCMD = new ChangeMoneyCMD(-actualTradeAmount * market.price);
        Ctrl.ExeCMD(changeMoneyCMD);

        Ctrl.Notify(NotifyConst.SetPointState, new SetPointStateArgs()
        {
            stockType = cmd.StockType,
            state = actualTradeAmount > 0 ? PointState.Buy : PointState.Sell
        });

        yield return null;
    }
    private void UpdateStockPrice()
    {
        RefreshAllStockPrices();
        Ctrl.Notify(NotifyConst.UpdateStockChart);
    }

    private void NextRoundTurnPostReaction(NextRoundTurnCMD action)
    {
        if (!MadeInHeavenSystem.Ins.IsMadeInHeavenActive)
        {
            UpdateStockPrice();
        }
    }
    private IEnumerator ChangeMoneyPerformer(ChangeMoneyCMD action)
    {
        currentMoney += action.Amount;
        currentMoney = Mathf.Max(0, currentMoney);
        yield return null;
    }

    private IEnumerator ChangeStockPerformer(ChangeStockCMD cmd)
    {
        var market = GetStockMarket(cmd.StockType);
        if (market == null)
        {
            Debug.LogError($"未找到股票类型: {cmd.StockType}");
            yield break;
        }
        market.holding = Mathf.Max(0, market.holding + cmd.Amount);
        ChangeStockPriceCMD changeStockPriceCMD;
        if (cmd.Amount > 0)
        {
            changeStockPriceCMD = new ChangeStockPriceCMD(PlayerAttributeSystem.Ins.playerView, cmd.StockType, PlayerAttributeSystem.Ins.ChangePriceDictionaryWhenBuy, PlayerAttributeSystem.Ins.ChangePricePersentDictionaryWhenBuy);
        }
        else
        {
            changeStockPriceCMD = new ChangeStockPriceCMD(PlayerAttributeSystem.Ins.playerView, cmd.StockType, PlayerAttributeSystem.Ins.ChangePriceDictionaryWhenSell, PlayerAttributeSystem.Ins.ChangePricePersentDictionaryWhenSell);
        }
        Ctrl.AddCMD(changeStockPriceCMD);
        yield return null;
    }

    #endregion

    #region Public Interface

    /// <summary>
    /// 获取指定股市数据
    /// </summary>
    public StockModel GetStockMarket(EStockType stockType)
    {
        return Data.stocks.Find(e => e.type == stockType);
    }


    /// <summary>
    /// 获取当前金币
    /// </summary>
    public float GetCurrentMoney()
    {
        return currentMoney;
    }


    /// <summary>
    /// 检查是否有足够的资金
    /// </summary>
    public bool HasEnoughMoney(float requiredAmount)
    {
        return currentMoney >= requiredAmount;
    }

    /// <summary>
    /// 格式化金钱显示
    /// </summary>
    public string FormatMoney(float amount)
    {
        if (Cfg.financialData != null)
        {
            return Cfg.financialData.FormatMoney(amount);
        }
        return amount.ToString("N0");
    }

    /// <summary>
    /// 格式化当前金钱显示
    /// </summary>
    public string FormatCurrentMoney()
    {
        return FormatMoney(currentMoney);
    }




    #endregion

}
