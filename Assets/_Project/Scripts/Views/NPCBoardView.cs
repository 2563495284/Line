using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBoardView : MonoBehaviour
{
    public List<NPCView> NPCViews { get; private set; } = new();

    [SerializeField] private Transform slot;
    [SerializeField] private float removeNPCScaleDuration = 0.25f;

    public void AddNPC(NPCData npcData)
    {

        NPCView npcView = NPCViewCreator.Ins.CreateNPCView(npcData, slot.position, slot.rotation);
        npcView.transform.parent = slot;
        //初始手牌
        DrawCardsGA drawCardsGA = new(npcData.initialDrawCount, npcView);
        ActionSystem.Ins.Perform(drawCardsGA);

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