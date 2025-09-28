using System;
using System.Collections;
using UnityEngine;

public class Effect_Trade : EffectWithTarget
{
    private int tradeAmount; // 正数买入，负数卖出

    public Effect_Trade(string[] args) : base(args)
    {
        tradeAmount = int.Parse(args[0]);
    }

    public override IEnumerator Run(IEffectEmitter from, IEffectReceiver to)
    {
        if (to is not StockModel)
            yield break;
        StockModel stock = to as StockModel;
        yield return ExeCMD(new TradeSpecificStockCMD(stock.stockId, tradeAmount));
    }
}