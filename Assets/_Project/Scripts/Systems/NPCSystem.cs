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
        // 检查NPCBoardView是否存在
        if (npcBoardView == null || npcBoardView.NPCViews == null || npcBoardView.NPCViews.Count == 0)
        {
            return;
        }

        // 找到有手牌的NPC，添加空引用检查
        foreach (var npc in npcBoardView.NPCViews)
        {
            // 检查NPC是否为空或已被销毁
            if (npc == null || npc.gameObject == null)
            {
                continue;
            }

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
        }
    }

    /// <summary>
    /// 重置NPC系统到初始状态
    /// </summary>
    public void ResetSystem()
    {
        if (npcBoardView == null) return;

        // 使用NPCBoardView的清空方法
        npcBoardView.ClearAllNPCs();
    }
}