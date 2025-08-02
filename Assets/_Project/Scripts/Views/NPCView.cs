using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCView : MonoBehaviour
{
    [Header("卡牌管理")]
    public List<Card> Deck = new List<Card>();
    public List<Card> Hand = new List<Card>();
    public List<Card> DiscardPile = new List<Card>();

    [Header("游戏设置")]
    public int MaxHandSize = 3;
    public int InitialDrawCount = 3;

    /// <summary>
    /// 初始化NPC的卡牌系统
    /// </summary>
    /// <param name="npcData">NPC数据</param>
    public void Setup(NPCData npcData)
    {
        // 复制卡牌数据到牌堆
        foreach (CardData cardData in npcData.CardDataList)
        {
            Deck.Add(new Card(cardData));
        }
        InitialDrawCount = npcData.initialDrawCount;
        MaxHandSize = npcData.maxHandSize;
        Hand.Clear();
        DiscardPile.Clear();

        // 初始洗牌
        ShuffleDeck();

        // 初始抽牌
        DrawInitialCards();
    }

    /// <summary>
    /// 初始抽牌
    /// </summary>
    private void DrawInitialCards()
    {
        int drawCount = Mathf.Min(InitialDrawCount, MaxHandSize);
        for (int i = 0; i < drawCount; i++)
        {
            TryDrawCard();
        }
    }

    /// <summary>
    /// 出牌
    /// </summary>
    /// <param name="card">要出的卡牌</param>
    /// <param name="immediateEffect">是否立即触发效果</param>
    /// <returns>是否成功出牌</returns>
    public bool PlayCard(Card card)
    {
        // 检查手牌中是否有这张卡
        if (!Hand.Contains(card))
        {
            Debug.LogWarning($"尝试出牌失败：手牌中没有卡牌 {card.Title}");
            return false;
        }

        // 从手牌移除
        Hand.Remove(card);

        // 添加到弃牌堆
        DiscardPile.Add(card);

        // 立即触发卡牌效果
        PlayCardGA playCardGA = new(card);
        ActionSystem.Instance.Perform(playCardGA);

        // 尝试抽一张新卡
        TryDrawCard();

        Debug.Log($"成功出牌：{card.Title}");
        return true;
    }

    /// <summary>
    /// 尝试抽牌
    /// </summary>
    /// <returns>是否成功抽牌</returns>
    public bool TryDrawCard()
    {
        // 检查手牌是否已满
        if (Hand.Count >= MaxHandSize)
        {
            Debug.Log("手牌已满，无法抽牌");
            return false;
        }

        // 如果牌堆为空，尝试从弃牌堆重新洗牌
        if (Deck.Count == 0)
        {
            if (!RefillDeckFromDiscard())
            {
                Debug.Log("牌堆和弃牌堆都为空，无法抽牌");
                return false;
            }
        }

        // 从牌堆顶部抽牌
        Card drawnCard = Deck[0];
        Deck.RemoveAt(0);
        Hand.Add(drawnCard);

        Debug.Log($"成功抽牌：{drawnCard.Title}");
        return true;
    }

    /// <summary>
    /// 从弃牌堆重新填充牌堆
    /// </summary>
    /// <returns>是否成功重新填充</returns>
    public bool RefillDeckFromDiscard()
    {
        if (DiscardPile.Count == 0)
        {
            return false;
        }

        // 将弃牌堆的所有卡牌移回牌堆
        Deck.AddRange(DiscardPile);
        DiscardPile.Clear();

        // 洗牌
        ShuffleDeck();

        Debug.Log($"从弃牌堆重新填充牌堆，共 {Deck.Count} 张卡牌");
        return true;
    }

    /// <summary>
    /// 洗牌（Fisher-Yates算法）
    /// </summary>
    public void ShuffleDeck()
    {
        if (Deck.Count <= 1) return;

        // Fisher-Yates洗牌算法
        for (int i = Deck.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);

            // 交换卡牌位置
            Card temp = Deck[i];
            Deck[i] = Deck[randomIndex];
            Deck[randomIndex] = temp;
        }

        Debug.Log($"牌堆洗牌完成，共 {Deck.Count} 张卡牌");
    }

    /// <summary>
    /// 强制抽指定数量的卡牌
    /// </summary>
    /// <param name="count">要抽的卡牌数量</param>
    /// <returns>实际抽到的卡牌数量</returns>
    public int DrawCards(int count)
    {
        int drawnCount = 0;
        for (int i = 0; i < count; i++)
        {
            if (TryDrawCard())
            {
                drawnCount++;
            }
            else
            {
                break; // 无法继续抽牌
            }
        }
        return drawnCount;
    }

    /// <summary>
    /// 丢弃手牌中的指定卡牌
    /// </summary>
    /// <param name="cardData">要丢弃的卡牌</param>
    /// <returns>是否成功丢弃</returns>
    public bool DiscardCard(Card card)
    {
        if (!Hand.Contains(card))
        {
            Debug.LogWarning($"尝试丢弃卡牌失败：手牌中没有卡牌 {card.Title}");
            return false;
        }

        Hand.Remove(card);
        DiscardPile.Add(card);

        Debug.Log($"成功丢弃卡牌：{card.Title}");
        return true;
    }

    /// <summary>
    /// 丢弃所有手牌
    /// </summary>
    public void DiscardAllHand()
    {
        int discardedCount = Hand.Count;
        DiscardPile.AddRange(Hand);
        Hand.Clear();

        Debug.Log($"丢弃所有手牌，共 {discardedCount} 张卡牌");
    }

    /// <summary>
    /// 获取牌堆信息
    /// </summary>
    /// <returns>牌堆信息字符串</returns>
    public string GetDeckInfo()
    {
        return $"牌堆: {Deck.Count}张, 手牌: {Hand.Count}/{MaxHandSize}张, 弃牌堆: {DiscardPile.Count}张";
    }

    /// <summary>
    /// 随机出牌
    /// </summary>
    /// <param name="targetHandSize">目标手牌数量（出牌后保持的手牌数量）</param>
    /// <returns>出牌的卡牌，如果没有可出的卡牌则返回null</returns>
    public Card PlayRandomCard(int targetHandSize = 0)
    {
        // 如果目标手牌数量为0，则使用默认的最大手牌数量
        if (targetHandSize <= 0)
        {
            targetHandSize = MaxHandSize;
        }

        // 确保手牌数量达到目标数量
        EnsureHandSize(targetHandSize);

        // 如果手牌为空，无法出牌
        if (Hand.Count == 0)
        {
            Debug.LogWarning("手牌为空，无法随机出牌");
            return null;
        }

        // 随机选择一张手牌
        int randomIndex = UnityEngine.Random.Range(0, Hand.Count);
        Card cardToPlay = Hand[randomIndex];

        // 出牌
        bool success = PlayCard(cardToPlay);
        if (success)
        {
            Debug.Log($"随机出牌成功：{cardToPlay.Title}");
            return cardToPlay;
        }
        else
        {
            Debug.LogError($"随机出牌失败：{cardToPlay.Title}");
            return null;
        }
    }

    /// <summary>
    /// 确保手牌数量达到指定数量
    /// </summary>
    /// <param name="targetSize">目标手牌数量</param>
    /// <returns>实际达到的手牌数量</returns>
    public int EnsureHandSize(int targetSize)
    {
        int currentSize = Hand.Count;

        // 如果当前手牌数量已经达到或超过目标数量，直接返回
        if (currentSize >= targetSize)
        {
            return currentSize;
        }

        int needToDraw = targetSize - currentSize;
        int actuallyDrawn = 0;

        Debug.Log($"手牌数量不足，需要抽取 {needToDraw} 张卡牌");

        // 尝试抽取所需数量的卡牌
        for (int i = 0; i < needToDraw; i++)
        {
            if (TryDrawCard())
            {
                actuallyDrawn++;
            }
            else
            {
                // 如果无法抽牌，尝试从弃牌堆重新洗牌
                if (RefillDeckFromDiscard())
                {
                    // 重新洗牌后再次尝试抽牌
                    if (TryDrawCard())
                    {
                        actuallyDrawn++;
                    }
                    else
                    {
                        // 即使重新洗牌后也无法抽牌，说明所有卡牌都在手牌中
                        break;
                    }
                }
                else
                {
                    // 弃牌堆也为空，无法继续抽牌
                    Debug.LogWarning("牌堆和弃牌堆都为空，无法继续抽牌");
                    break;
                }
            }
        }

        Debug.Log($"手牌补充完成，实际抽取了 {actuallyDrawn} 张卡牌，当前手牌数量：{Hand.Count}");
        return Hand.Count;
    }

    /// <summary>
    /// 批量随机出牌
    /// </summary>
    /// <param name="count">要出的卡牌数量</param>
    /// <param name="maintainHandSize">是否保持手牌数量（出牌后自动补充）</param>
    /// <returns>实际出牌的卡牌列表</returns>
    public List<Card> PlayRandomCards(int count, bool maintainHandSize = true)
    {
        List<Card> playedCards = new List<Card>();

        for (int i = 0; i < count; i++)
        {
            // 如果选择保持手牌数量，则每次出牌后都确保手牌数量
            int targetHandSize = maintainHandSize ? MaxHandSize : 0;

            Card playedCard = PlayRandomCard(targetHandSize);
            if (playedCard != null)
            {
                playedCards.Add(playedCard);
            }
            else
            {
                // 无法继续出牌，退出循环
                Debug.LogWarning($"无法继续出牌，已出 {playedCards.Count} 张卡牌");
                break;
            }
        }

        Debug.Log($"批量随机出牌完成，共出牌 {playedCards.Count} 张");
        return playedCards;
    }

    /// <summary>
    /// 智能随机出牌（考虑手牌管理）
    /// </summary>
    /// <param name="preferredHandSize">期望保持的手牌数量</param>
    /// <param name="maxPlayCount">最大出牌数量</param>
    /// <returns>实际出牌的卡牌列表</returns>
    public List<Card> SmartRandomPlay(int preferredHandSize = 0, int maxPlayCount = 3)
    {
        if (preferredHandSize <= 0)
        {
            preferredHandSize = MaxHandSize;
        }

        List<Card> playedCards = new List<Card>();
        int availableCards = Hand.Count;

        // 计算可以出的卡牌数量（保留期望的手牌数量）
        int canPlayCount = Mathf.Max(0, availableCards - preferredHandSize);
        int actualPlayCount = Mathf.Min(canPlayCount, maxPlayCount);

        if (actualPlayCount <= 0)
        {
            Debug.Log($"无法出牌：手牌数量 {availableCards}，期望保持 {preferredHandSize} 张");
            return playedCards;
        }

        // 执行随机出牌
        for (int i = 0; i < actualPlayCount; i++)
        {
            Card playedCard = PlayRandomCard(preferredHandSize);
            if (playedCard != null)
            {
                playedCards.Add(playedCard);
            }
            else
            {
                break;
            }
        }

        Debug.Log($"智能随机出牌完成，出牌 {playedCards.Count} 张，保持手牌 {Hand.Count} 张");
        return playedCards;
    }
}