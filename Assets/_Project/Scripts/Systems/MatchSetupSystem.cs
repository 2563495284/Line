using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSetupSystem : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private PerkData perkData;
    [SerializeField] private List<NPCData> npcDataList;

    private void Start()
    {

        NPCSystem.Instance.Setup(npcDataList);

        CardSystem.Instance.Setup(playerData);

        PerkSystem.Instance.AddPerk(new Perk(perkData));


        DrawCardsGA drawCardsGA = new(playerData.initialDrawCount, CardSystem.Instance.playerView);

        ActionSystem.Instance.Perform(drawCardsGA);
    }
}