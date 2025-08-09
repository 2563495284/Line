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
}