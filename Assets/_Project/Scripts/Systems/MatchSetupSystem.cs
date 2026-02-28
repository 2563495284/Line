using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSetupSystem : Singleton<MatchSetupSystem>
{
    [SerializeField] private PlayerGameData playerData;
    [SerializeField] private PerkData perkData;
    [SerializeField] private List<NPCData> npcDataList;

    private void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        MultiStockSystem.Instance.InitializeStockMarkets();

        NPCSystem.Instance.Setup(npcDataList);

        PlayerAttributeSystem.Instance.Setup(playerData);

        DrawCardsGA drawCardsGA = new(playerData.initialDrawCount, PlayerAttributeSystem.Instance.playerView);
        ActionSystem.Instance.Perform(drawCardsGA);
    }

    /// <summary>
    /// 获取玩家数据（供重启时使用）
    /// </summary>
    public PlayerGameData GetPlayerData()
    {
        return playerData;
    }

    /// <summary>
    /// 获取NPC数据列表（供重启时使用）
    /// </summary>
    public List<NPCData> GetNPCDataList()
    {
        return npcDataList;
    }
}