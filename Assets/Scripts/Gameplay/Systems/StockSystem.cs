using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameConfig;
using UnityEngine;

public class StockSystem : LevelSystem
{
    public override void OnEnter()
    {
        base.OnEnter();
        BindProcessor<ApplyIndustryPowerCMD>(ApplyIndustryPowerProcessor);
        BindProcessor<ApplyStockAttrCMD>(ApplyStockAttrProcessor);
        BindProcessor<CalcPerformanceCMD>(CalcPerformanceProcessor);
        BindProcessor<SettlementStockCMD>(SettlementStockProcessor);
        BindProcessor<UpdateStockStrategyCMD>(UpdateStockStrategyProcessor);

        BindProcessor<AddExtraStockAttrCMD>(AddExtraStockAttrProcessor);
        BindProcessor<AddPerformanceCMD>(AddPerformanceProcessor);
        BindProcessor<RemoveExtraStockAttrCMD>(RemoveExtraStockAttrProcessor);
        BindProcessor<ScalePerformanceCMD>(ScalePerformanceProcessor);
        BindProcessor<StockAttrGrownCMD>(StockAttrGrownProcessor);
        BindProcessor<TransferStockFactorCMD>(TransferStockFactorProcessor);
    }
    public override void OnExit()
    {
        base.OnExit();
        UnbindProcessor<ApplyIndustryPowerCMD>();
        UnbindProcessor<ApplyStockAttrCMD>();
        UnbindProcessor<CalcPerformanceCMD>();
        UnbindProcessor<SettlementStockCMD>();
        UnbindProcessor<UpdateStockStrategyCMD>();
        UnbindProcessor<AddExtraStockAttrCMD>();
        UnbindProcessor<AddPerformanceCMD>();
        UnbindProcessor<RemoveExtraStockAttrCMD>();
        UnbindProcessor<ScalePerformanceCMD>();
        UnbindProcessor<StockAttrGrownCMD>();
        UnbindProcessor<TransferStockFactorCMD>();
    }
    private IEnumerator ApplyIndustryPowerProcessor(ApplyIndustryPowerCMD cmd)
    {
        yield return 0;
    }
    private IEnumerator ApplyStockAttrProcessor(ApplyStockAttrCMD cmd)
    {
        List<StockModel> stocks = new(Model.stocks.List);
        for (int i = 0; i < stocks.Count; i++)
        {
            StockModel stock = stocks[i];
            for (int j = 0; j < stock.attrs.Count; j++)
            {
                StockAttrData attr = stock.attrs[j];
                if (attr.delay <= 0 && attr.HasEffectReady(cmd.timing))
                {
                    bool hasPerformEffect = false;
                    for (int k = 0; k < attr.Effects.Count; k++)
                    {
                        EffectBase effect = attr.Effects[k];
                        if (effect.probablity >= 1 || RNG.Rand() < effect.probablity)
                        {
                            if (!hasPerformEffect)
                            {
                                yield return Perform(EventConst.StartStockAttrEffect, new StartStockAttrEffectArgs(stock.stockId, attr.Id));
                                hasPerformEffect = true;
                            }
                            switch (cmd.timing)
                            {
                                case EStockAttrApplyTiming.RoundStart:
                                    if (effect is IAutoEffect_RoundStart roundStartEff)
                                        yield return roundStartEff.ApplyInRoundStart(attr);
                                    break;
                                case EStockAttrApplyTiming.BeforeSettlement:
                                    if (effect is IAutoEffect_BeforeCalc beforeCalcEff)
                                        yield return beforeCalcEff.ApplyBeforeCalc(attr);
                                    break;
                                case EStockAttrApplyTiming.AfterSettlement:
                                    if (effect is IAutoEffect_RoundEnd roundEndEff)
                                        yield return roundEndEff.ApplyInRoundEnd(attr);
                                    break;
                            }
                        }
                    }
                    if (hasPerformEffect)
                        yield return Perform(EventConst.ExitStockAttrEffect, new ExitStockAttrEffectArgs(stock.stockId, attr.Id));
                }
            }
        }
    }
    private IEnumerator CalcPerformanceProcessor(CalcPerformanceCMD cmd)
    {
        yield return 0;
    }
    private IEnumerator SettlementStockProcessor(SettlementStockCMD cmd)
    {
        yield return 0;
    }
    private IEnumerator UpdateStockStrategyProcessor(UpdateStockStrategyCMD cmd)
    {
        List<StockModel> stocks = Model.stocks.List;
        List<(int, int)> changedInfo = new();
        for (int i = 0; i < stocks.Count; i++)
        {
            if (stocks[i].remainRound <= 0)
            {
                int newStrategy = GetStockStrategy(stocks[i].CurStrategyId, stocks[i].cfg);
                changedInfo.Add((stocks[i].stockId, newStrategy));
                stocks[i].NextStrategy(newStrategy);
            }
        }
        for (int i = 0; i < changedInfo.Count; i++)
        {
            (int stockId, int strategyId) = changedInfo[i];
            yield return Perform(EventConst.StockStrategyChange, new StockStrategyChangeArgs(stockId, strategyId));
        }
        yield return 0;
    }
    private int GetStockStrategy(int lastStrategyId, StockConfig cfg)
    {
        int lastStrategySerial = Config.StockStrategyConfig.Get(lastStrategyId).SerialId;
        int maxRound = cfg.maxRoundInStrategy;
        List<float> wei = new() { cfg.neutralStrategyWeight, cfg.positiveStrategyWeight, cfg.negativeStrategyWeight };
        List<int> sign = new() { 0, 1, 2 };
        int targetStrategyEmotion = sign.WeiRand(wei);
        int result = Config.StockStrategyConfig.list
            .Where(e => e.SerialId != lastStrategyId &&
                e.Round <= maxRound &&
                Config.StockStrategySerialConfig.Get(e.SerialId).Emotion == targetStrategyEmotion)
            .Select(e => e.Id)
            .ToList().Rand();
        return result;
    }

    #region  Attr Effect CMD

    private IEnumerator AddExtraStockAttrProcessor(AddExtraStockAttrCMD cmd)
    {
        StockModel stock = Model.stocks[cmd.stockId];
        StockAttrData attrData = new(stock.stockId, cmd.attrType, cmd.round, 0, true);
        stock.attrs.Add(attrData);
        yield return Perform(EventConst.AddExtraStockAttr, new AddExtraStockAttrArgs(stock.stockId, attrData.Id));
    }
    private IEnumerator AddPerformanceProcessor(AddPerformanceCMD cmd)
    {
        StockModel stock = Model.stocks[cmd.stockId];
        stock.calcFactor.performanceAdd += cmd.addVal;
        yield return Perform(EventConst.StockPerformanceAdded, new StockPerformaceAddedArgs(stock.stockId, cmd.addVal));
    }
    private IEnumerator RemoveExtraStockAttrProcessor(RemoveExtraStockAttrCMD cmd)
    {
        StockModel stock = Model.stocks[cmd.stockId];
        List<StockAttrData> extraDatas = stock.attrs.Where(e => e.isExtra).ToList();
        List<StockAttrData> removeTargets = extraDatas.WeiRandMul(extraDatas.Select(e => 1f).ToList(), cmd.num, false);
        if (removeTargets.Count > 0)
        {
            removeTargets.ForEach(e => stock.attrs.Remove(e));
            yield return Perform(EventConst.RemoveExtraStockAttr, new RemoveExtraStockAttrArgs(stock.stockId, removeTargets.Select(e => e.Id).ToList()));
        }
    }
    private IEnumerator ScalePerformanceProcessor(ScalePerformanceCMD cmd)
    {
        StockModel stock = Model.stocks[cmd.stockId];
        stock.calcFactor.performanceMul *= cmd.mul;
        yield return Perform(EventConst.StockPerformanceMuled, new StockPerformanceMuledArgs(stock.stockId, cmd.mul));
    }
    private IEnumerator StockAttrGrownProcessor(StockAttrGrownCMD cmd)
    {
        yield return Perform(EventConst.StockAttrGrown, new StockAttrGrownArgs(cmd.stockId, cmd.attrId));
    }
    private IEnumerator TransferStockFactorProcessor(TransferStockFactorCMD cmd)
    {
        StockModel stock = Model.stocks[cmd.stockId];
        List<Func<float>> factorGetter = new()
        {
            ()=>stock.Info.factor_strength,
            ()=>stock.Info.factor_senity,
            ()=>stock.Info.factor_industry
        };
        List<Action<float>> factorSetter = new()
        {
            v=>stock.Info.factor_strength = v,
            v=>stock.Info.factor_senity=v,
            v=>stock.Info.factor_industry=v
        };
        List<float> oldFactors = new List<int>() { 0, 1, 2 }.Select(e => factorGetter[e].Invoke()).ToList();
        float amount = cmd.volume * factorGetter[cmd.fromFactorId - 1].Invoke();
        factorSetter[cmd.toFactorId - 1].Invoke(factorGetter[cmd.toFactorId - 1].Invoke() + amount);
        factorSetter[cmd.fromFactorId - 1].Invoke(factorGetter[cmd.fromFactorId - 1].Invoke() - amount);
        List<float> newFactors = new List<int>() { 0, 1, 2 }.Select(e => factorGetter[e].Invoke()).ToList();
        yield return Perform(EventConst.TransferStockFactor, new TransferStockFactorArgs(stock.stockId, oldFactors, newFactors));
    }
    #endregion
}