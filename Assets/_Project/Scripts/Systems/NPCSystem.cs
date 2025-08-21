using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSystem : Singleton<NPCSystem>
{
    public List<NPCView> NPCs => npcBoardView.NPCViews;

    [Header("NPC出牌设置")]
    [SerializeField] private NPCBoardView npcBoardView;
    [SerializeField] private float npcPlayInterval = 3f; // NPC出牌间隔（秒）
    [SerializeField] private Timer npcPlayTimer; // NPC出牌计时器

    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<NextRoundTurnGA>(NextRoundTurnPreReaction, ReactionTiming.PRE);
    }

    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<NextRoundTurnGA>(NextRoundTurnPreReaction, ReactionTiming.PRE);
    }

    public void Setup(List<NPCData> npcDataList)
    {
        foreach (NPCData npcData in npcDataList)
        {
            npcBoardView.AddNPC(npcData);

        }
    }



    /// <summary>
    /// 让下一个NPC出牌（计时器回调方法）
    /// </summary>
    private void NextRoundTurnPreReaction(NextRoundTurnGA action)
    {
        // 检查是否有NPC可以出牌
        if (npcBoardView.NPCViews.Count == 0)
        {
            return;
        }

        // 找到有手牌的NPC
        npcBoardView.NPCViews.ForEach(npc =>
        {
            Debug.Log($"NPC {npc.name} 策略: {npc.StrategyType} 手牌数量: {npc.hand.Count}");

            if (npc.hand.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, npc.hand.Count);
                Card cardToPlay = npc.hand[randomIndex];
                PlayCardGA playCardGA = new(cardToPlay, npc, MultiStockSystem.Instance.GetLineViewRandom());
                ActionSystem.Instance.AddReaction(playCardGA);
            }
            else
            {
                Debug.Log($"NPC {npc.name} 补牌");
                DrawCardsGA drawCardsGA = new(npc.MaxHandSize, npc);
                ActionSystem.Instance.AddReaction(drawCardsGA);
            }
        });

    }
}