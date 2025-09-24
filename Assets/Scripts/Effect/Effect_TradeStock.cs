using System;
using System.Collections;
using UnityEngine;

public class Effect_TradeStock : EffectWithTarget
{
    [SerializeField]
    private int tradeAmount; // 正数买入，负数卖出
    public override IEnumerator Run(IEffectEmitter from, IEffectReceiver to)
    {
        if (to is not StockModel)
            yield break;
        StockModel stock = to as StockModel;
        yield return ExeCMD(new TradeSpecificStockCMD(stock.type, tradeAmount));
    }
}