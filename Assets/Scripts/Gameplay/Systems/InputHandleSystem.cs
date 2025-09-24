using System.Collections;

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
        Ctrl.BindPerformer<SelectCardCMD>(PerformSelectCard);
        Ctrl.BindPerformer<DragCardCMD>(PerformDragCard);
        Ctrl.BindPerformer<ReleaseCardCMD>(PerformReleaseCard);
        Ctrl.BindPerformer<PreviewCardCMD>(PerformPreviewCard);
        Ctrl.BindPerformer<CancelPreviewCardCMD>(PerformCancelPreviewCard);
        Ctrl.BindPerformer<SelectCardTargetCMD>(PerformSelectTarget);
        Ctrl.BindPerformer<CancelSelectTargetCMD>(PerformCancelSelectTarget);
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
    private IEnumerator PerformSelectCard(SelectCardCMD cmd)
    {
        if (!allowInput)
            yield break;
        ECardSelectState state = cmd.card.GetCardStateInRound();
        switch (state)
        {
            case ECardSelectState.Ready:
                selectCard = cmd.card;
                if (previewCard != null)
                {
                    Ctrl.Notify(NotifyConst.CancelPreviewCard, new PreviewCardArgs { card = cmd.card, });
                    previewCard = null;
                }
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
    private IEnumerator PerformDragCard(DragCardCMD cmd)
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
    private IEnumerator PerformReleaseCard(ReleaseCardCMD cmd)
    {
        if (selectCard == null || !allowInput)
            yield break;
        if (cmd.card.Cfg.releaseType == ECardReleaseType.TargetToStock && cardTarget == null)
        {
            Ctrl.Notify(NotifyConst.TipsCardReleaseFail, new TipsCardReleaseFailArgs { card = cmd.card, target = null, failState = ECardReleaseState.LackTarget });
        }
        else
        {
            if (cmd.card.Cfg.releaseType == ECardReleaseType.NoTarget)
            {
                yield return Ctrl.ExeCMD(new PlayCardCMD(cmd.card, null));
            }
            else
            {
                yield return Ctrl.ExeCMD(new PlayCardCMD(cmd.card, cardTarget.GetReceiver()));
            }
        }
        CancelSelectCurrent();
        yield return null;
    }
    private void CancelSelectCurrent()
    {
        if (selectCard == null)
            return;
        Ctrl.Notify(NotifyConst.CancelSelectCard, new DragCardArgs { card = selectCard });
        selectCard = null;
    }
    private IEnumerator PerformPreviewCard(PreviewCardCMD cmd)
    {
        if (!allowInput)
            yield break;
        if (selectCard != null)
            yield break;
        if (previewCard != null)
        {
            Ctrl.Notify(NotifyConst.CancelPreviewCard, new PreviewCardArgs
            {
                card = cmd.card
            });
        }

        previewCard = cmd.card;
        Ctrl.Notify(NotifyConst.PreviewCard, new PreviewCardArgs
        {
            card = cmd.card
        });
        yield return null;
    }
    private IEnumerator PerformCancelPreviewCard(CancelPreviewCardCMD cmd)
    {
        if (previewCard != null && cmd.card != previewCard)
            yield break;
        previewCard = null;
        Ctrl.Notify(NotifyConst.CancelPreviewCard, new PreviewCardArgs
        {
            card = cmd.card
        });

        yield return null;
    }
    private IEnumerator PerformSelectTarget(SelectCardTargetCMD cmd)
    {
        if (selectCard == null || !allowInput)
            yield break;
        if (cardTarget != null)
            Ctrl.Notify(NotifyConst.CancelSelectCardTarget, new CardTargetArgs
            {
                card = selectCard,
                target = cmd.target
            });
        cardTarget = cmd.target;
        Ctrl.Notify(NotifyConst.SelectCardTarget, new CardTargetArgs
        {
            card = selectCard,
            target = cmd.target
        });
        yield return null;
    }

    private IEnumerator PerformCancelSelectTarget(CancelSelectTargetCMD cmd)
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