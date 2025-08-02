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
    [SerializeField] private float npcPlayInterval = 0.5f; // NPC出牌间隔（秒）

    private Coroutine npcPlayCoroutine;
    private int currentNPCIndex = 0; // 当前出牌的NPC索引


    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        // 停止NPC出牌协程
        if (npcPlayCoroutine != null)
        {
            StopCoroutine(npcPlayCoroutine);
            npcPlayCoroutine = null;
        }
    }

    public void Setup(List<NPCData> npcDataList)
    {
        foreach (NPCData npcData in npcDataList)
        {
            npcBoardView.AddNPC(npcData);
        }

        // 启动NPC自动出牌
        StartNPCAutoPlay();
    }

    /// <summary>
    /// 启动NPC自动出牌
    /// </summary>
    public void StartNPCAutoPlay()
    {
        if (npcPlayCoroutine != null)
        {
            StopCoroutine(npcPlayCoroutine);
        }

        npcPlayCoroutine = StartCoroutine(NPCAutoPlayCoroutine());
        Debug.Log("NPC自动出牌已启动");
    }

    /// <summary>
    /// 停止NPC自动出牌
    /// </summary>
    public void StopNPCAutoPlay()
    {
        if (npcPlayCoroutine != null)
        {
            StopCoroutine(npcPlayCoroutine);
            npcPlayCoroutine = null;
        }

        Debug.Log("NPC自动出牌已停止");
    }

    /// <summary>
    /// NPC自动出牌协程
    /// </summary>
    private IEnumerator NPCAutoPlayCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(npcPlayInterval);

            // 检查是否有NPC可以出牌
            if (npcBoardView.NPCViews.Count > 0)
            {
                PlayNextNPCCard();
            }
        }
    }

    /// <summary>
    /// 让下一个NPC出牌
    /// </summary>
    private void PlayNextNPCCard()
    {
        // 找到有手牌的NPC
        int attempts = 0;
        int maxAttempts = npcBoardView.NPCViews.Count;

        while (attempts < maxAttempts)
        {
            NPCView npc = npcBoardView.NPCViews[currentNPCIndex];

            if (npc != null && npc.Hand.Count > 0)
            {
                // 随机选择一张手牌并出牌
                int randomIndex = UnityEngine.Random.Range(0, npc.Hand.Count);
                Card cardToPlay = npc.Hand[randomIndex];

                bool success = npc.PlayCard(cardToPlay);

                if (success)
                {
                    Debug.Log($"NPC {npc.name} 出牌: {cardToPlay.Title}");
                }
                else
                {
                    Debug.LogWarning($"NPC {npc.name} 出牌失败: {cardToPlay.Title}");
                }

                // 移动到下一个NPC
                currentNPCIndex = (currentNPCIndex + 1) % npcBoardView.NPCViews.Count;
                break;
            }
            else
            {
                // 当前NPC没有手牌，移动到下一个
                currentNPCIndex = (currentNPCIndex + 1) % npcBoardView.NPCViews.Count;
                attempts++;
            }
        }

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("所有NPC都没有手牌可出");
        }
    }
}