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
        MultiStockSystem.Instance.InitializeStockMarkets();

        NPCSystem.Instance.Setup(npcDataList);

        PlayerAttributeSystem.Instance.Setup(playerData);

        PerkSystem.Instance.AddPerk(new Perk(perkData));


        DrawCardsGA drawCardsGA = new(playerData.initialDrawCount, PlayerAttributeSystem.Instance.playerView);

        ActionSystem.Instance.Perform(drawCardsGA);
    }
}