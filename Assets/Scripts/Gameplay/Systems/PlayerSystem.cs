
using System;
using System.Collections;
using UnityEngine;

public class PlayerSystem : LevelSystem
{
    public PlayerSystem(LevelController ctrl) : base(ctrl)
    {
    }
    public override void EnableSystem()
    {
        Ctrl.BindProcessor<DiscardAllCardsCMD>(DiscardAllCardsProcessor);
        //改变属性
        Ctrl.BindProcessor<ChangeAttributeCMD>(ChangeAttributeProcessor);
        //监听 回合前后
        Ctrl.AddRection<NextRoundTurnCMD>(NextRoundTurnPreReaction, ReactionTiming.PRE);
        Ctrl.AddRection<NextRoundTurnCMD>(NextRoundTurnPostReaction, ReactionTiming.POST);

        Ctrl.BindProcessor<ChangeManaCMD>(ChangeManaProcessor);
        Ctrl.BindProcessor<RefillManaCMD>(RefillManaProcessor);

        //改变金币
        Ctrl.AddRection<ChangeMoneyCMD>(ChangeMoneyPostReaction, ReactionTiming.POST);
        //改变股票数量
        Ctrl.AddRection<ChangeHoldingCMD>(ChangeStockPostReaction, ReactionTiming.POST);

        //监听属性变化
        Ctrl.AddRection<ChangeAttributeCMD>(ChangeAttributePostReaction, ReactionTiming.POST);
    }
    public override void DisableSystem()
    {
        Ctrl.UnbindPerformer<DiscardAllCardsCMD>();
        Ctrl.UnbindPerformer<ChangeAttributeCMD>();
        Ctrl.UnbindPerformer<ChangeManaCMD>();
        Ctrl.UnbindPerformer<RefillManaCMD>();
        Ctrl.RemoveRection<NextRoundTurnCMD>(NextRoundTurnPreReaction, ReactionTiming.PRE);
        Ctrl.RemoveRection<NextRoundTurnCMD>(NextRoundTurnPostReaction, ReactionTiming.POST);
        Ctrl.RemoveRection<ChangeMoneyCMD>(ChangeMoneyPostReaction, ReactionTiming.POST);
        Ctrl.RemoveRection<ChangeHoldingCMD>(ChangeStockPostReaction, ReactionTiming.POST);
        Ctrl.RemoveRection<ChangeAttributeCMD>(ChangeAttributePostReaction, ReactionTiming.POST);

    }


    [TraceableCoroutine("DiscardAll")]
    private IEnumerator DiscardAllCardsProcessor(DiscardAllCardsCMD discardAllCardsCMD)
    {

        Data.handCards.ForEach(e => Data.discardCards.Enqueue(e));
        Data.handCards.Clear();
        yield return Ctrl.RequestPerform(PerformRequest.DiscardCardAll);
    }
    [TraceableCoroutine("ChangeAttr")]
    private IEnumerator ChangeAttributeProcessor(ChangeAttributeCMD cmd)
    {
        Data.ChangeAttrValue(cmd.type, cmd.val);
        yield return null;
    }
    private void ChangeAttributePostReaction(ChangeAttributeCMD action)
    {
        EC.Send(NotifyConst.UpdatePlayerAttr);
    }

    private void ChangeMoneyPostReaction(ChangeMoneyCMD action)
    {
        EC.Send(NotifyConst.UpdateMoneyUI);
    }

    private void ChangeStockPostReaction(ChangeHoldingCMD action)
    {
        EC.Send(NotifyConst.UpdateStockChart);
    }
    private void NextRoundTurnPreReaction(NextRoundTurnCMD nextRoundTurnCMD)
    {
        Data.mana = 0;
        Ctrl.Notify(NotifyConst.UpdateManaUI);
    }

    private void NextRoundTurnPostReaction(NextRoundTurnCMD nextRoundTurnCMD)
    {

        //刷新信息
        UpdateInfo();
    }
    private void UpdateInfo()
    {
        EC.Send(NotifyConst.UpdatePlayerAttr);
        EC.Send(NotifyConst.UpdateStockChart);
        EC.Send(NotifyConst.UpdateMoneyUI);
    }
    [TraceableCoroutine("ChangeMana")]
    private IEnumerator ChangeManaProcessor(ChangeManaCMD changeManaCMD)
    {
        Data.mana = Mathf.Clamp(Data.mana + changeManaCMD.Amount, 0, Cfg.maxMana);
        EC.Send(NotifyConst.UpdateManaUI);
        yield return null;
    }
    [TraceableCoroutine("RefillMana")]
    private IEnumerator RefillManaProcessor(RefillManaCMD refillManaCMD)
    {
        Data.mana = Data.ManaPerTurn;
        EC.Send(NotifyConst.UpdateManaUI);
        yield return null;
    }


}