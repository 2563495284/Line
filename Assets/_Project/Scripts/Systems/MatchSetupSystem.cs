using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSetupSystem : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private PerkData perkData;
    [SerializeField] private List<NPCData> npcDataList;
    [SerializeField] private int startingHandSize = 5;

    private void Start()
    {

        NPCSystem.Instance.Setup(npcDataList);

        CardSystem.Instance.Setup(playerData.Deck);

        PerkSystem.Instance.AddPerk(new Perk(perkData));

        DrawCardsGA drawCardsGA = new(startingHandSize);

        ActionSystem.Instance.Perform(drawCardsGA);
    }
}