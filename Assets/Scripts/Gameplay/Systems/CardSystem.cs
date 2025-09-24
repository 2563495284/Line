using DG.Tweening;
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
        Ctrl.BindPerformer<DrawCardsCMD>(DrawCardsPerformer);
        Ctrl.BindPerformer<PlayCardCMD>(PlayCardPerformer);
        Ctrl.BindPerformer<EnemyPlayCardCMD>(EnemyPlayCardPerformer);

    }
    public override void DisableSystem()
    {
        Ctrl.UnbindPerformer<DrawCardsCMD>();
        Ctrl.UnbindPerformer<PlayCardCMD>();
        Ctrl.UnbindPerformer<EnemyPlayCardCMD>();

    }

    #region Performers

    private IEnumerator DrawCardsPerformer(DrawCardsCMD cmd)
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

    private IEnumerator PlayCardPerformer(PlayCardCMD cmd)
    {
        CardModel card = cmd.card;


        yield return Ctrl.RequestPerform(PerformRequest.Play_PresentCard, new PlayCardArgs(card, cmd.receiver));
        yield return Ctrl.RequestPerform(PerformRequest.Play_EffectCard, new PlayCardArgs(card, cmd.receiver));
        foreach (CardEffectWithTarget effectWrapper in card.Cfg.effectsWithTarget)
        {
            yield return effectWrapper.effect.Run(card, cmd.receiver);
        }
        foreach (CardEffect effectWrapper in card.Cfg.effects)
        {
            yield return effectWrapper.effect.Run(card);
        }
        yield return Ctrl.RequestPerform(PerformRequest.Play_DiscardCard, new PlayCardArgs(card, cmd.receiver));
        Data.handCards.Remove(card);
        Data.discardCards.Enqueue(card);

    }
    private IEnumerator EnemyPlayCardPerformer(EnemyPlayCardCMD cmd)
    {
        CardModel card = cmd.card;
        foreach (CardEffect effectWrapper in card.Cfg.effects)
        {
            yield return effectWrapper.effect.Run(card);
        }
    }
    #endregion
}