using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCView : CharacterView
{
    public override IEnumerator DrawCardAnimation(Card card)
    {
        yield break;
    }

    public override IEnumerator RemoveCardAnimation(Card card)
    {
        yield break;
    }

    /// <summary>
    /// 初始化NPC的卡牌系统
    /// </summary>
    /// <param name="npcData">NPC数据</param>
    public void Setup(NPCData npcData)
    {
        base.Setup(npcData);
    }
}