using System.Collections;
using UnityEngine.Rendering;

public class SEffect_AttrPurify : StockAttrEffect, IAutoEffect_RoundEnd
{
    public int num;
    public SEffect_AttrPurify(string effectKey, string[] args, float probablity = 1, int growId = 0) : base(effectKey, args, probablity, growId)
    {
        num = int.Parse(args[0]);
    }

    public IEnumerator ApplyInRoundEnd(IEffectSource source)
    {
        int stockid = (source as StockAttrData).StockId;
        yield return ExeCMD(new RemoveExtraStockAttrCMD(stockid, num));
    }
}