using System.Collections;
//仅针对玩家的价格修改效果
//对价格的影响默认都是比例变化，标明increment才是增量
public class Effect_Price : EffectWithTarget
{
    private float pricePercent;
    public Effect_Price(string[] args) : base(args)
    {
        pricePercent = float.Parse(args[0]);
    }

    public override IEnumerator Run(IEffectEmitter from, IEffectReceiver to)
    {
        if (to is StockModel stock)
        {
            yield return ExeCMD(new ChangePricePercentCMD(pricePercent, stock.stockId, true, true));
        }
    }
}