using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CardSystem : Singleton<CardSystem>
{
    [SerializeField] public PlayerView playerView;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ChangeStrategyGA>(ChangeStrategyPerformer);
        ActionSystem.AttachPerformer<DrawCardsGA>(DrawCardsPerformer);
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
        // ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        // ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ChangeStrategyGA>();
        ActionSystem.DetachPerformer<DrawCardsGA>();
        ActionSystem.DetachPerformer<DiscardAllCardsGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
        // ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        // ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    public void Setup(PlayerData playerData)
    {
        playerView.Setup(playerData);
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
            TipsSystem.Instance.ShowError("手牌已满！");
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
            TipsSystem.Instance.ShowInfo("手牌已满!");
        }
    }

    private IEnumerator DiscardAllCardsPerformer(DiscardAllCardsGA discardAllCardsGA)
    {
        CharacterView characterView = discardAllCardsGA.CharacterView;
        foreach (Card card in characterView.hand)
        {
            yield return characterView.RemoveCard(card);

        }

        characterView.hand.Clear();
    }

    private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
    {
        CharacterView characterView = playCardGA.CharacterView;
        characterView.hand.Remove(playCardGA.Card);
        yield return characterView.RemoveCard(playCardGA.Card);

        characterView.DoManualTargetEffect(playCardGA);
        characterView.DoAutoTargetEffect(playCardGA);

        Debug.Log($"玩家 {characterView.name} 出牌: {playCardGA.Card.Title}");
    }
    #endregion

    #region Reactions
    // private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    // {
    //     DiscardAllCardsGA discardAllCardsGA = new();
    //     ActionSystem.Instance.AddReaction(discardAllCardsGA);
    // }

    // private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    // {
    //     DrawCardsGA drawCardsGA = new(enemyDrawCardsAmount);
    //     ActionSystem.Instance.AddReaction(drawCardsGA);
    // }
    #endregion
}