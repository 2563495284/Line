using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSetupSystem : SingletonCom<MatchSetupSystem>
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private List<NPCData> npcDataList;

    private void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        MultiStockSystem.Ins.InitializeStockMarkets();

        NPCSystem.Ins.Setup(npcDataList);

        PlayerAttributeSystem.Ins.Setup(playerData);

        DrawCardsGA drawCardsGA = new(playerData.initialDrawCount, PlayerAttributeSystem.Ins.playerView);
        ActionSystem.Ins.Perform(drawCardsGA);
    }

    /// <summary>
    /// 获取玩家数据（供重启时使用）
    /// </summary>
    public PlayerData GetPlayerData()
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