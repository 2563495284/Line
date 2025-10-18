using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SEffect_GrowAdd : StockAttrEffect, IAutoEffect_RoundEnd
{
    public int targetGrowId = 1;
    public float val = 0;

    public SEffect_GrowAdd(string effectKey, string[] args, float probablity = 1, int growId = 0) : base(effectKey, args, probablity, growId)
    {
        targetGrowId = int.Parse(args[0]);
        val = float.Parse(args[1]);
    }


    public IEnumerator ApplyInRoundEnd(IEffectSource source)
    {
        StockAttrData fromData = source as StockAttrData;
        StockModel stock = GM.LevelData.stocks[fromData.StockId];
        List<IGrowTarget> targets = stock.attrs.Where(e => e is IGrowTarget g).Select(e => e as IGrowTarget).ToList();
        targets.ForEach(e => e.GrowingVal(source, (growId, targetVal) => growId == targetGrowId ? targetVal + val : targetVal));
        yield return ExeCMD(new StockAttrGrownCMD(fromData.Id, stock.stockId));
    }
}