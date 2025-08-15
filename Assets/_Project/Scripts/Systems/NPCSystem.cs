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

    private int currentNPCIndex = 0; // 当前出牌的NPC索引

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        // 停止NPC出牌计时器
        StopNPCAutoPlay();
    }

    public void Setup(List<NPCData> npcDataList)
    {
        foreach (NPCData npcData in npcDataList)
        {
            npcBoardView.AddNPC(npcData);

        }

        // 启动NPC自动出牌
        StartNPCAutoPlay();

        StartCoroutine(DelayedFirstPlay());
    }

    /// <summary>
    /// 延迟执行首次NPC出牌（可选方法）
    /// </summary>
    private IEnumerator DelayedFirstPlay()
    {
        yield return new WaitForSeconds(1f); // 等待1秒确保初始化完成
        PlayNextNPCCard();
    }


    /// <summary>
    /// 启动NPC自动出牌
    /// </summary>
    public void StartNPCAutoPlay()
    {
        if (npcPlayTimer != null && npcPlayTimer.IsRunning)
        {
            return; // 已经在运行中
        }

        // 如果没有Timer组件，创建一个
        if (npcPlayTimer == null)
        {
            npcPlayTimer = gameObject.AddComponent<Timer>();
        }

        // 设置Timer参数
        npcPlayTimer.SetDuration(npcPlayInterval);
        npcPlayTimer.SetCountdown(true); // 倒计时
        npcPlayTimer.SetLoop(true); // 循环
        npcPlayTimer.OnTimerComplete += PlayNextNPCCard;
        npcPlayTimer.StartTimer();
        Debug.Log("NPC自动出牌已启动");
    }

    /// <summary>
    /// 停止NPC自动出牌
    /// </summary>
    public void StopNPCAutoPlay()
    {
        if (npcPlayTimer != null)
        {
            npcPlayTimer.StopTimer();
            npcPlayTimer.OnTimerComplete -= PlayNextNPCCard;
        }
        Debug.Log("NPC自动出牌已停止");
    }

    /// <summary>
    /// 暂停NPC自动出牌
    /// </summary>
    public void PauseNPCAutoPlay()
    {
        if (npcPlayTimer != null)
        {
            npcPlayTimer.PauseTimer();
            Debug.Log("NPC自动出牌已暂停");
        }
    }

    /// <summary>
    /// 恢复NPC自动出牌
    /// </summary>
    public void ResumeNPCAutoPlay()
    {
        if (npcPlayTimer != null && npcBoardView.NPCViews.Count > 0)
        {
            npcPlayTimer.ResumeTimer();
            Debug.Log("NPC自动出牌已恢复");
        }
    }

    /// <summary>
    /// 让下一个NPC出牌（计时器回调方法）
    /// </summary>
    private void PlayNextNPCCard()
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
                PlayCardGA playCardGA = new(cardToPlay, npc);
                ActionSystem.Instance.Perform(playCardGA);
            }
            else
            {
                Debug.Log($"NPC {npc.name} 补牌");
                DrawCardsGA drawCardsGA = new(npc.MaxHandSize, npc);
                ActionSystem.Instance.Perform(drawCardsGA);
            }
        });

    }
}