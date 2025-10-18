using System.Collections;
using GameConfig;

public class SEffect_AttrAdd : StockAttrEffect, IAutoEffect_RoundEnd
{
    public int attrId;
    public int round;
    public SEffect_AttrAdd(string effectKey, string[] args, float probablity = 1, int growId = 0) : base(effectKey, args, probablity, growId)
    {
        attrId = int.Parse(args[0]);
        round = int.Parse(args[1]);
    }

    public IEnumerator ApplyInRoundEnd(IEffectSource source)
    {
        int stockId = (source as StockAttrData).StockId;
        yield return ExeCMD(new AddExtraStockAttrCMD(stockId, (StockAttrType)attrId, round));
    }
}