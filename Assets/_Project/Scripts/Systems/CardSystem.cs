using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CardSystem : SingletonCom<CardSystem>
{

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ChangeStrategyGA>(ChangeStrategyPerformer);
        ActionSystem.AttachPerformer<DrawCardsGA>(DrawCardsPerformer);
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);

    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ChangeStrategyGA>();
        ActionSystem.DetachPerformer<DrawCardsGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
    }

    #region Performers
    private IEnumerator ChangeStrategyPerformer(ChangeStrategyGA changeStrategyGA)
    {
        CharacterView characterView = changeStrategyGA.CharacterView;
        characterView.ChangeStrategy(changeStrategyGA.StrategyType);
        yield return null;
    }

    private IEnumerator DrawCardsPerformer(DrawCardsGA drawCardsGA)
    {
        CharacterView characterView = drawCardsGA.CharacterView;

        int handCapacity = characterView.MaxHandSize - characterView.hand.Count;
        if (handCapacity <= 0 && characterView.CharacterType == ECharacterType.Player)
        {
            TipsSystem.Ins.ShowError("手牌已满！");
            yield break;
        }
        bool handFull = drawCardsGA.Amount > handCapacity;
        int amount = math.min(drawCardsGA.Amount, handCapacity);


        int actualAmount = Mathf.Min(amount, characterView.drawPile.Count);
        int notDrawnAmount = amount - actualAmount;

        for (int i = 0; i < actualAmount; i++)
        {
            yield return characterView.DrawCard();
        }

        if (notDrawnAmount > 0)
        {
            characterView.RefillDeck();

            for (int i = 0; i < notDrawnAmount; i++)
            {
                yield return characterView.DrawCard();
            }
        }
        if (handFull && characterView.CharacterType == ECharacterType.Player)
        {
            TipsSystem.Ins.ShowInfo("手牌已满!");
        }
    }

    private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
    {
        CharacterView characterView = playCardGA.CharacterView;
        characterView.hand.Remove(playCardGA.Card);
        yield return characterView.RemoveCard(playCardGA.Card);

        characterView.DoManualTargetEffect(playCardGA);
        characterView.DoAutoTargetEffect(playCardGA);

    }
    #endregion
}