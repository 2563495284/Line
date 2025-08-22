using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBoardView : MonoBehaviour
{
    public List<NPCView> NPCViews { get; private set; } = new();

    [SerializeField] private List<Transform> slots;
    [SerializeField] private float removeNPCScaleDuration = 0.25f;

    public void AddNPC(NPCData npcData)
    {
        // 检查是否有可用的slot
        if (NPCViews.Count >= slots.Count)
        {
            Debug.LogError($"无法添加更多NPC，已达到最大数量限制: {slots.Count}");
            return;
        }

        Transform slot = slots[NPCViews.Count];
        NPCView npcView = NPCViewCreator.Instance.CreateNPCView(npcData, slot.position, slot.rotation);
        npcView.transform.parent = slot;
        //初始手牌
        DrawCardsGA drawCardsGA = new(npcData.initialDrawCount, npcView);
        ActionSystem.Instance.Perform(drawCardsGA);

        NPCViews.Add(npcView);
    }

    public IEnumerator RemoveNPC(NPCView npcView)
    {
        NPCViews.Remove(npcView);
        Tween tween = npcView.transform.DOScale(Vector3.zero, removeNPCScaleDuration);
        yield return tween.WaitForCompletion();
        Destroy(npcView.gameObject);
    }

    /// <summary>
    /// 清空所有NPC（用于重置）
    /// </summary>
    public void ClearAllNPCs()
    {
        // 销毁所有现有的NPC
        var npcsToDestroy = new List<NPCView>(NPCViews);
        foreach (var npc in npcsToDestroy)
        {
            if (npc != null)
            {
                Destroy(npc.gameObject);
            }
        }

        // 清空NPC列表
        NPCViews.Clear();
    }
}