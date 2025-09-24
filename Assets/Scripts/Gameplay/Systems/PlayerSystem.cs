
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
        Ctrl.BindPerformer<DiscardAllCardsCMD>(DiscardAllCardsPerformer);
        //改变属性
        Ctrl.BindPerformer<ChangeAttributeCMD>(ChangeAttributePerformer);
        Ctrl.BindPerformer<CheckAndConsumeResourceCMD>(CheckAndConsumeResourcePerformer);
        //监听 回合前后
        Ctrl.AddRection<NextRoundTurnCMD>(NextRoundTurnPreReaction, ReactionTiming.PRE);
        Ctrl.AddRection<NextRoundTurnCMD>(NextRoundTurnPostReaction, ReactionTiming.POST);

        Ctrl.BindPerformer<ChangeManaCMD>(ChangeManaPerformer);
        Ctrl.BindPerformer<RefillManaCMD>(RefillManaPerformer);

        //改变金币
        Ctrl.AddRection<ChangeMoneyCMD>(ChangeMoneyPostReaction, ReactionTiming.POST);
        //改变股票数量
        Ctrl.AddRection<ChangeStockCMD>(ChangeStockPostReaction, ReactionTiming.POST);

        //监听属性变化
        Ctrl.AddRection<ChangeAttributeCMD>(ChangeAttributePostReaction, ReactionTiming.POST);
    }
    public override void DisableSystem()
    {
        Ctrl.UnbindPerformer<DiscardAllCardsCMD>();
        Ctrl.UnbindPerformer<ChangeAttributeCMD>();
        Ctrl.UnbindPerformer<ChangeManaCMD>();
        Ctrl.UnbindPerformer<RefillManaCMD>();
        Ctrl.UnbindPerformer<CheckAndConsumeResourceCMD>();
        Ctrl.RemoveRection<NextRoundTurnCMD>(NextRoundTurnPreReaction, ReactionTiming.PRE);
        Ctrl.RemoveRection<NextRoundTurnCMD>(NextRoundTurnPostReaction, ReactionTiming.POST);
        Ctrl.RemoveRection<ChangeMoneyCMD>(ChangeMoneyPostReaction, ReactionTiming.POST);
        Ctrl.RemoveRection<ChangeStockCMD>(ChangeStockPostReaction, ReactionTiming.POST);
        Ctrl.RemoveRection<ChangeAttributeCMD>(ChangeAttributePostReaction, ReactionTiming.POST);

    }


    private IEnumerator DiscardAllCardsPerformer(DiscardAllCardsCMD discardAllCardsCMD)
    {

        Data.handCards.ForEach(e => Data.discardCards.Enqueue(e));
        Data.handCards.Clear();
        yield return Ctrl.RequestPerform(PerformRequest.DiscardCardAll);
    }
    private IEnumerator ChangeAttributePerformer(ChangeAttributeCMD cmd)
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

    private void ChangeStockPostReaction(ChangeStockCMD action)
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
    private IEnumerator ChangeManaPerformer(ChangeManaCMD changeManaCMD)
    {
        Data.mana = Mathf.Clamp(Data.mana + changeManaCMD.Amount, 0, Cfg.maxMana);
        EC.Send(NotifyConst.UpdateManaUI);
        yield return null;
    }
    private IEnumerator RefillManaPerformer(RefillManaCMD refillManaCMD)
    {
        Data.mana = Data.ManaPerTurn;
        EC.Send(NotifyConst.UpdateManaUI);
        yield return null;
    }

    /// <summary>
    /// 处理条件消耗资源的逻辑
    /// </summary>
    private IEnumerator CheckAndConsumeResourcePerformer(CheckAndConsumeResourceCMD action)
    {
        // 检查所有资源是否足够
        foreach (var cost in action.ResourceCosts)
        {
            if (!cost.CanAfford())
            {
                // 资源不足，震动相机提示
                Utils.ShakeCamera();
                GM.Tips("资源不足");
                Debug.Log($"资源不足: {cost.GetDescription()}");
                yield break;
            }
        }

        // 消耗所有资源
        foreach (var cost in action.ResourceCosts)
        {
            var consumeAction = cost.GetConsumeAction();
            if (consumeAction != null)
            {
                // ctrl.ExeCMD(consumeAction);
            }
        }

        // 执行成功后的效果
        foreach (var effect in action.SuccessEffects)
        {
            effect.SetCharacterView(action.CharacterView);
            effect.SetTargetLineView(action.TargetLineView);
            // ctrl.ExeCMD(successAction);
        }

        yield return null;
    }

}