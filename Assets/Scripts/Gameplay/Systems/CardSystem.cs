using DG.Tweening;
using GameConfig;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Card_System : LevelSystem
{
    public Card_System(LevelController ctrl) : base(ctrl)
    {
    }
    public override void EnableSystem()
    {
        Ctrl.BindProcessor<DrawCardsCMD>(DrawCardsProcessor);
        Ctrl.BindProcessor<PlayCardCMD>(PlayCardProcessor);
        Ctrl.BindProcessor<EnemyPlayCardCMD>(EnemyPlayCardProcessor);

    }
    public override void DisableSystem()
    {
        Ctrl.UnbindPerformer<DrawCardsCMD>();
        Ctrl.UnbindPerformer<PlayCardCMD>();
        Ctrl.UnbindPerformer<EnemyPlayCardCMD>();

    }

    #region Performers
    [TraceableCoroutine("DrawCards")]
    private IEnumerator DrawCardsProcessor(DrawCardsCMD cmd)
    {
        Func<int> GetNumInDraws = () => Data.drawCards.Count;
        int drawRemain = cmd.num;
        int max = Cfg.maxHandNum;
        while (drawRemain > 0)
        {
            int readyToDraw = Math.Min(drawRemain, GetNumInDraws());
            bool fullTips = readyToDraw + Data.handCards.Count > max;
            bool isRefill = drawRemain > GetNumInDraws();
            int actualDraw = Math.Min(readyToDraw, max - Data.handCards.Count);
            drawRemain -= actualDraw;
            List<CardModel> targetCards = new();
            while (actualDraw-- > 0) targetCards.Add(Data.drawCards.Dequeue());
            Data.handCards = Data.handCards.Concat(targetCards).ToList();
            yield return Ctrl.RequestPerform(PerformRequest.DrawCards, new DeckOPArgs(targetCards));
            if (fullTips)
            {
                Ctrl.Notify(NotifyConst.PopupTips, new PopupTipsArgs
                {
                    message = "手牌已满"
                });
                yield break;
            }
            if (isRefill)
            {
                yield return Ctrl.RequestPerform(PerformRequest.RefillCards);
                List<CardModel> newDeck = Data.discardCards.ToList();
                newDeck.Shuffle();
                foreach (CardModel card in newDeck)
                    Data.drawCards.Enqueue(card);
            }

        }

    }

    [TraceableCoroutine("PlayCard")]
    private IEnumerator PlayCardProcessor(PlayCardCMD cmd)
    {
        CardModel card = cmd.card;


        yield return Ctrl.RequestPerform(PerformRequest.Play_PresentCard, new PlayCardArgs(card, cmd.receiver));
        yield return Ctrl.RequestPerform(PerformRequest.Play_EffectCard, new PlayCardArgs(card, cmd.receiver));
        if (card.Cfg.ReleaseMode == ReleaseMode.NoTarget)
            yield return card.Execute(cmd.card);
        else
            yield return card.Execute(cmd.card, cmd.receiver);
        yield return Ctrl.ExeCMD(new ChangeManaCMD(-cmd.card.Cfg.ManaCost));
        yield return Ctrl.RequestPerform(PerformRequest.Play_DiscardCard, new PlayCardArgs(card, cmd.receiver));
        Data.handCards.Remove(card);
        Data.discardCards.Enqueue(card);

    }
    [TraceableCoroutine("EnemyDrawCard")]
    private IEnumerator EnemyPlayCardProcessor(EnemyPlayCardCMD cmd)
    {
        CardModel card = cmd.card;
        yield return card.Execute(cmd.enemy);
    }
    #endregion
}