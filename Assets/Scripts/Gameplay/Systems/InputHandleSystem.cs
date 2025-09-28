using System.Collections;
using GameConfig;
using UnityEngine;

public class InputHandleSystem : LevelSystem
{
    public InputHandleSystem(LevelController ctrl) : base(ctrl)
    {
    }
    private bool allowInput = true;
    private CardModel selectCard;
    private CardModel previewCard;
    private ICardEffectTarget cardTarget = null;
    public override void EnableSystem()
    {
        Ctrl.BindProcessor<SelectCardCMD>(SelectCardProcessor);
        Ctrl.BindProcessor<DragCardCMD>(DragCardProcessor);
        Ctrl.BindProcessor<ReleaseCardCMD>(ReleaseCardProcessor);
        Ctrl.BindProcessor<PreviewCardCMD>(PreviewCardProcessor);
        Ctrl.BindProcessor<CancelPreviewCardCMD>(CancelPreviewCardProcessor);
        Ctrl.BindProcessor<SelectCardTargetCMD>(SelectTargetProcessor);
        Ctrl.BindProcessor<CancelSelectTargetCMD>(CancelSelectTargetProcessor);
    }
    public override void DisableSystem()
    {
        Ctrl.UnbindPerformer<SelectCardCMD>();
        Ctrl.UnbindPerformer<DragCardCMD>();
        Ctrl.UnbindPerformer<ReleaseCardCMD>();
        Ctrl.UnbindPerformer<PreviewCardCMD>();
        Ctrl.UnbindPerformer<CancelPreviewCardCMD>();
        Ctrl.UnbindPerformer<SelectCardTargetCMD>();
        Ctrl.UnbindPerformer<CancelSelectTargetCMD>();

    }
    [TraceableCoroutine("SelectCard")]
    private IEnumerator SelectCardProcessor(SelectCardCMD cmd)
    {
        if (!allowInput)
            yield break;
        ECardSelectState state = cmd.card.GetCardStateInRound();
        switch (state)
        {
            case ECardSelectState.Ready:
                selectCard = cmd.card;
                if (previewCard != null)
                    yield return Ctrl.ExeCMD(new CancelPreviewCardCMD(previewCard));
                Ctrl.Notify(NotifyConst.StartDragCard, new DragCardArgs
                {
                    card = cmd.card,
                    worldPos = MouseUtils.GetMouseWp()
                });
                break;
            case ECardSelectState.LackMana:
                Ctrl.Notify(NotifyConst.PopupTips, new PopupTipsArgs() { message = "能量不足" });
                break;
        }
        yield return null;
    }
    [TraceableCoroutine("DragCard")]
    private IEnumerator DragCardProcessor(DragCardCMD cmd)
    {
        if (!allowInput)
            yield break;
        if (selectCard == null)
            yield break;
        Ctrl.Notify(NotifyConst.DraggingCard, new DragCardArgs
        {
            card = cmd.card,
            worldPos = MouseUtils.GetMouseWp()
        });
        yield return null;
    }
    [TraceableCoroutine("ReleaseCard")]
    private IEnumerator ReleaseCardProcessor(ReleaseCardCMD cmd)
    {
        if (selectCard == null || !allowInput)
            yield break;
        Ctrl.Notify(NotifyConst.CancelSelectCard, new DragCardArgs { card = selectCard });
        if (cmd.card.Cfg.ReleaseMode == ReleaseMode.TargetToStock && cardTarget == null)
        {
            //TODO lackTarget
        }
        else
        {
            if (cmd.card.Cfg.ReleaseMode == ReleaseMode.NoTarget)
            {
                yield return Ctrl.ExeCMD(new PlayCardCMD(cmd.card, null));
            }
            else
            {
                Ctrl.Notify(NotifyConst.CancelSelectCardTarget, new CardTargetArgs { card = selectCard, target = cardTarget });
                yield return Ctrl.ExeCMD(new PlayCardCMD(cmd.card, cardTarget.GetReceiver()));
            }
        }
        selectCard = null;
        yield return null;
    }
    [TraceableCoroutine("PreviewCard")]
    private IEnumerator PreviewCardProcessor(PreviewCardCMD cmd)
    {
        if (!allowInput)
            yield break;
        if (selectCard != null)
            yield break;
        if (previewCard != null)
        {
            Ctrl.Notify(NotifyConst.CancelPreviewCard, new PreviewCardArgs
            {
                card = previewCard
            });
        }
        previewCard = cmd.card;
        Ctrl.Notify(NotifyConst.PreviewCard, new PreviewCardArgs
        {
            card = cmd.card
        });
        yield return null;
    }
    [TraceableCoroutine("CancelPreviewCard")]
    private IEnumerator CancelPreviewCardProcessor(CancelPreviewCardCMD cmd)
    {
        if (previewCard != null && cmd.card != previewCard || !allowInput)
            yield break;
        previewCard = null;
        Ctrl.Notify(NotifyConst.CancelPreviewCard, new PreviewCardArgs
        {
            card = cmd.card
        });

        yield return null;
    }
    [TraceableCoroutine("SelectTarget")]
    private IEnumerator SelectTargetProcessor(SelectCardTargetCMD cmd)
    {
        if (selectCard == null || !allowInput)
            yield break;
        if (cardTarget != null)
        {
            Ctrl.Notify(NotifyConst.CancelSelectCardTarget, new CardTargetArgs
            {
                card = selectCard,
                target = cardTarget
            });
            cardTarget.CancelPreviewEffect();
        }
        cardTarget = cmd.target;
        Ctrl.Notify(NotifyConst.SelectCardTarget, new CardTargetArgs
        {
            card = selectCard,
            target = cmd.target
        });
        cardTarget.PreviewEffect();
        yield return null;
    }

    [TraceableCoroutine("CancelSelectTarget")]
    private IEnumerator CancelSelectTargetProcessor(CancelSelectTargetCMD cmd)
    {
        if (selectCard == null || !allowInput)
            yield break;
        if (cardTarget != null && cmd.target != cardTarget)
            yield break;
        cardTarget = null;
        Ctrl.Notify(NotifyConst.CancelSelectCardTarget, new CardTargetArgs
        {
            card = selectCard,
            target = cmd.target
        });
        yield return null;
    }
}