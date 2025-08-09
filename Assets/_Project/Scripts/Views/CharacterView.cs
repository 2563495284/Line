using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
public enum ECharacterType
{
    Player,
    NPC
}
public abstract class CharacterView : MonoBehaviour
{
    [Header("卡牌管理")]
    public List<Card> drawPile = new List<Card>();
    public List<Card> hand = new List<Card>();
    public List<Card> DiscardPile = new List<Card>();

    [Header("游戏设置")]
    public int MaxHandSize = 3;
    public int InitialDrawCount = 3;

    public ECharacterType CharacterType { get; private set; }

    public ECharacterStrategyType StrategyType { get; private set; }

    /// <summary>
    /// 初始化NPC的卡牌系统
    /// </summary>
    /// <param name="npcData">NPC数据</param>
    public void Setup(CharacterData characterData)
    {
        // 复制卡牌数据到牌堆
        foreach (CardData cardData in characterData.Deck)
        {
            drawPile.Add(new Card(cardData));
        }
        drawPile.Shuffle();
        InitialDrawCount = characterData.initialDrawCount;
        MaxHandSize = characterData.maxHandSize;
        hand.Clear();
        DiscardPile.Clear();

        CharacterType = characterData.CharacterType;
        StrategyType = characterData.StrategyType;
    }
    public IEnumerator RemoveCard(Card card)
    {
        DiscardPile.Add(card);
        yield return StartCoroutine(RemoveCardAnimation(card));
    }

    public abstract IEnumerator RemoveCardAnimation(Card card);

    public virtual IEnumerator DrawCard()
    {
        Card card = drawPile.Draw();
        if (card == null)
        {
            TipsSystem.Instance.ShowError("抽卡失败，牌堆为空！");
            yield break;
        }

        hand.Add(card);
        yield return StartCoroutine(DrawCardAnimation(card));
    }
    public abstract IEnumerator DrawCardAnimation(Card card);

    public virtual void RefillDeck()
    {
        drawPile.AddRange(DiscardPile);
        DiscardPile.Clear();
    }

    public virtual void DoManualTargetEffect(PlayCardGA playCardGA)
    {
        if (playCardGA.Card.ManualTargetEffect != null)
        {
            PerformEffectGA performEffectGA = new(playCardGA.Card.ManualTargetEffect, this);
            ActionSystem.Instance.AddReaction(performEffectGA);
        }
    }

    public virtual void DoAutoTargetEffect(PlayCardGA playCardGA)
    {
        foreach (AutoTargetEffect effectWrapper in playCardGA.Card.OtherEffects)
        {
            PerformEffectGA performEffectGA = new(effectWrapper.Effect, this);
            ActionSystem.Instance.AddReaction(performEffectGA);
        }
    }

    public virtual void ChangeStrategy(ECharacterStrategyType strategyType)
    {
        StrategyType = strategyType;
    }
}