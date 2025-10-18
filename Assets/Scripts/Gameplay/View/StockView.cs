using System.Collections;
using UnityEngine;

public class StockView : LevelView
{
    public int stockId = 0;
    public override void OnEnter()
    {
        base.OnEnter();
    }
    public override void OnExit()
    {
        base.OnExit();
    }
    private IEnumerator StockStrategyChangePerformer(StockStrategyChangeArgs args)
    {
        GM.Ins.Level.Notify(EventConst.PopupTips, new PopupTipsArgs($"{args.stockId} 切换策略: ${args.strategyId}"));
        yield return new WaitForSeconds(2);
    }
}