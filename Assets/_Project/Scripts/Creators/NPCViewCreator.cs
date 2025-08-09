using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCViewCreator : Singleton<NPCViewCreator>
{
    public int NPCViewCount { get; private set; } = 0;
    [SerializeField] private NPCView npcViewPrefab;

    public NPCView CreateNPCView(NPCData npcData, Vector3 position, Quaternion rotation)
    {
        NPCView npcView = Instantiate(npcViewPrefab, position, rotation);
        npcView.Setup(npcData);
        NPCViewCount++;
        npcView.name = $"NPC {NPCViewCount}";
        return npcView;
    }
}